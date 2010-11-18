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
    /// A faster queue that doesn't delay releasing semaphores, improving throughput.
    /// Semaphores are granted to be released in order, and they are not released by
    /// other thread by the first acquiring the semaphore
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ZeroWaitQueue<T>
        : IFifoQueue<T> where T : class
    {
        private int _head, _tail, _lastWritten, _lastRead, _count;
        private readonly int _capacity;
        private bool _disposed;
        private readonly Semaphore _readSema, _writeSema;
        private readonly T[] _array;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of ZeroWaitQueue
        /// </summary>
        public ZeroWaitQueue()
            : this(512)
        {
        }

        /// <summary>
        /// Initializes ZeroWaitQueue with the specified capacity
        /// </summary>
        /// <param name="size">Maximum number of elements to store</param>
        public ZeroWaitQueue(int size)
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
            _lastWritten = int.MinValue;
            _lastRead = int.MinValue;

            _readSema = new Semaphore(0, _capacity);
            _writeSema = new Semaphore(_capacity, _capacity);
        }

        /// <remarks/>
        ~ZeroWaitQueue()
        {
            Dispose();
        }

        #endregion


        #region IFifoQueue<T> Membri di

        void IFifoQueue<T>.Enqueue(T item)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            if (item == null) throw new ArgumentNullException("item");

            _writeSema.WaitOne();
            int index = Interlocked.Increment(ref _head);
            index %= _capacity;
            if (index < 0) index += _capacity;
            _array[index] = item;

            //Did we get the semaphore for first?
            int lw = _lastWritten % _capacity;
            if (lw < 0) lw += _capacity;


            if (index != lw) return; //We are not the first, somebody else will release
            do
            {
                Interlocked.Increment(ref _count);
                Interlocked.Increment(ref _lastWritten);
                index++;
                _readSema.Release();
            } while (_array[index] != null);
        }

        T IFifoQueue<T>.Dequeue()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            _readSema.WaitOne();
            int index = Interlocked.Increment(ref _tail);
            index %= _capacity;
            if (index < 0) index += _capacity;
            T ret=_array[index];
            _array[index] = null;

            int lr = _lastRead%_capacity;
            if (lr < 0) lr += _capacity;

            //Did we get the semaphore for first?

            if (index != lr)
            {
                do
                {
                    Interlocked.Decrement(ref _count);
                    Interlocked.Increment(ref _lastRead);
                    index++;
                    _writeSema.Release();
                } while (_array[index] == null);
            }

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

        /// <remarks/>
        public void Dispose()
        {
            if (_disposed) return;

            GC.SuppressFinalize(this);

            _disposed = true;

            _writeSema.Close();
            _readSema.Close();
        }

        #endregion

        private T[] FlushInternal()
        {
            List<T> ret = new List<T>(_count);
            while (_readSema.WaitOne(0))
            {
                int index = Interlocked.Increment(ref _tail);
                index %= _capacity;
                if (index < 0) index += _capacity;
                Interlocked.Decrement(ref _count);
                Interlocked.Increment(ref _lastRead);
                T item=_array[index];
                ret.Add(item);
                _writeSema.Release();
            }
            return ret.ToArray();
        }
    }
}
