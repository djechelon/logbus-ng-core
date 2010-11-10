/*
 *                  Logbus-ng project
 *    ©2010 Logbus Reasearch Team - Some rights reserved
 *
 *  Created by:
 *      Vittorio Alfieri - vitty85@users.sourceforge.net
 *      Antonio Anzivino - djechelon@users.sourceforge.net
 *
 *  Based on the research project "Logbus" by
 *
 *  Dipartimento di Informatica e Sistemistica
 *  University of Naples "Federico II"
 *  via Claudio, 21
 *  80121 Naples, Italy
 *
 *  Software is distributed under Microsoft Reciprocal License
 *  Documentation under Creative Commons 3.0 BY-SA License
*/

using System;
using System.Collections.Generic;
using System.Threading;

namespace
    It.Unina.Dis.Logbus.Utils
{
    /// <summary>
    /// Implements the FIFO queue with static array and two semaphores for best performance in producers/consumers problem
    /// </summary>
    /// <remarks>Currently unusable. The queue actually LEAKS when concurrently accessed in write mode.
    /// No explanation for this absurde behaviour as all operations on pointers are thread safe</remarks>
    public sealed class
        FastFifoQueue<T>
        : IFifoQueue<T> where T : class
    {
        private T[] _array;
        private int _head, _tail, _count;
        private readonly int _capacity;
        private bool _disposed;

        private readonly Semaphore _readSema, _writeSema;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of FastFifoQueue
        /// </summary>
        public FastFifoQueue()
            : this(512)
        {
        }

        /// <summary>
        /// Initializes FastFifoQueue with the specified capacity
        /// </summary>
        /// <param name="size">Maximum number of elements to store</param>
        public FastFifoQueue(int size)
        {
            //Check if size is power of 2
            //Credit: http://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            if ((size & (size - 1)) != 0)
                throw new ArgumentOutOfRangeException("size", "Size must be a power of 2 for this queue to work");

            _capacity = size;
            _array = new T[size];
            _count = 0;
            _head = int.MinValue;
            _tail = int.MinValue;

            _readSema = new Semaphore(0, _capacity);
            _writeSema = new Semaphore(_capacity, _capacity);
        }

        ~FastFifoQueue()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            GC.SuppressFinalize(this);

            _disposed = true;

            _writeSema.Close();
            _readSema.Close();
            if (disposing) _array = null;
        }
        #endregion

        #region IFifoQueue<T> Membri di

        /* Credit to Dan Tao and Les from stackoverflow.com
         * http://stackoverflow.com/questions/3898204/can-a-c-blocking-fifo-queue-leak-messages-whats-wrong-in-my-code */

        void IFifoQueue<T>.Enqueue(T item)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            if (item == null) throw new ArgumentNullException("item");

            _writeSema.WaitOne();
            int index = Interlocked.Increment(ref _head);
            index %= _capacity;
            if (index < 0) index += _capacity;
            //_array[index] = item;
            while (Interlocked.CompareExchange(ref _array[index], item, null) != null)
                Thread.Sleep(0);

            Interlocked.Increment(ref _count);

            _readSema.Release();
        }

        T IFifoQueue<T>.Dequeue()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            _readSema.WaitOne();
            int index = Interlocked.Increment(ref _tail);
            index %= _capacity;
            if (index < 0) index += _capacity;
            T ret;
            while ((ret = Interlocked.Exchange(ref _array[index], null)) == null)
                Thread.Sleep(0);

            Interlocked.Decrement(ref _count);
            _writeSema.Release();

            return ret;
        }

        int IFifoQueue<T>.Count
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _count;
            }
        }

        T[] IFifoQueue<T>.Flush()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            return FlushInternal();
        }

        T[] IFifoQueue<T>.FlushAndDispose()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            _disposed = true;

            try
            {
                return FlushInternal();
            }
            finally
            {
                Dispose();
            }
        }

        #endregion

        #region IDisposable Membri di

        /// <summary>
        /// Implements IDisposable.Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        private T[] FlushInternal()
        {
            List<T> ret = new List<T>(_count);
            while (_count > 0 && _readSema.WaitOne(1))
            {
                int index = Interlocked.Increment(ref _tail);
                index %= _capacity;
                if (index < 0) index += _capacity;
                Interlocked.Decrement(ref _count);
                ret.Add(_array[index]);
                _array[index] = null;
            }
            return ret.ToArray();
        }
    }
}