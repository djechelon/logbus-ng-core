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
namespace It.Unina.Dis.Logbus.Utils
{
    /// <summary>
    /// Implements the FIFO queue with static array and two semaphores for best performance in producers/consumers problem
    /// </summary>
    public sealed class FastFifoQueue<T>
        : IFifoQueue<T>
    {

        private T[] _array;
        private int _head, _tail, _count;
        private readonly int _capacity;
        private bool _disposed;

        private Semaphore _readSema, _writeSema;

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        public FastFifoQueue()
            : this(512)
        { }

        /// <summary>
        /// Initializes FastFifoQueue with the specified capacity
        /// </summary>
        /// <param name="size"></param>
        public FastFifoQueue(int size)
        {
            //Check if size is power of 2
            //Credit: http://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
            if ((size & (size - 1)) != 0)
                throw new ArgumentOutOfRangeException("size", "Size must be a power of 2 for this queue to work");

            _capacity = size;
            _array = new T[size];
            _count = 0;
            _head = -1;
            _tail = -1;

            _readSema = new Semaphore(0, _capacity);
            _writeSema = new Semaphore(_capacity, _capacity);
        }

        #endregion

        #region IFifoQueue<T> Membri di

        public void Enqueue(T item)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            _writeSema.WaitOne();
            int index = (((Interlocked.Increment(ref _head)) % _capacity) + _capacity) % _capacity;
            _array[index] = item;
            _readSema.Release();
            Interlocked.Increment(ref _count);
        }

        public T Dequeue()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            _readSema.WaitOne();
            int index = (((Interlocked.Increment(ref _tail)) % _capacity) + _capacity) % _capacity;
            T ret = _array[index];

            _array[index] = default(T); //Null
            _writeSema.Release();
            Interlocked.Decrement(ref _count);
            return ret;
        }

        public int Count
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(GetType().FullName);
                return _count;
            }
        }

        public T[] Flush()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            return FlushInternal();
        }

        public T[] FlushAndDispose()
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

        public void Dispose()
        {
            _disposed = true;

            _writeSema.Close();
            _readSema.Close();
            _array = null;
        }

        #endregion

        private T[] FlushInternal()
        {
            List<T> ret = new List<T>(_count);
            while (_readSema.WaitOne(10))
            {
                int index = (((Interlocked.Increment(ref _tail)) % _capacity) + _capacity) % _capacity;
                Interlocked.Decrement(ref _count);
                ret.Add(_array[index]);
                _array[index] = default(T);
            }
            return ret.ToArray();
        }
    }
}
