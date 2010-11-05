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
    /// Dynamic queue implementation of IFifoQueue
    /// </summary>
    /// <typeparam name="T">Type of object the queue will hold</typeparam>
    public sealed class BlockingFifoQueue<T>
        : IFifoQueue<T>
    {
        private readonly Queue<T> _theQueue;
        private Semaphore _sema;
        private long _count;

        private volatile bool _disposed;

        #region Constructor/Destructor

        /// <summary>
        /// Intializes the blocking FIFO queue
        /// </summary>
        public BlockingFifoQueue()
        {
            _theQueue = new Queue<T>();
            _sema = new Semaphore(0, int.MaxValue);
        }

        /// <summary>
        /// Initializes the blocking FIFO queue with a starting collection of elements
        /// </summary>
        /// <param name="collection">Collection of elements to insert into the queue</param>
        public BlockingFifoQueue(IEnumerable<T> collection)
        {
            _theQueue = new Queue<T>(collection);
            _sema = new Semaphore(_theQueue.Count, int.MaxValue);
            _count = _theQueue.Count;
        }

        /// <summary>
        /// Initializes the blocking FIFO queue with an initial capacity
        /// </summary>
        /// <param name="capacity">Initial capacity to set</param>
        public BlockingFifoQueue(int capacity)
        {
            _theQueue = new Queue<T>(capacity);
            _sema = new Semaphore(0, capacity);
        }

        /// <remarks/>
        ~BlockingFifoQueue()
        {
            Dispose(false);
        }

        #endregion

        /// <summary>
        /// Enqueues an object in the queue
        /// </summary>
        /// <param name="item">Object to enqueue</param>
        public void Enqueue(T item)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            lock (_theQueue)
                _theQueue.Enqueue(item);
            Interlocked.Increment(ref _count);
            _sema.Release();
        }

        /// <summary>
        /// Dequeues an object from the queue as soon as it's available
        /// </summary>
        /// <returns>The first object enqueued</returns>
        public T Dequeue()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            _sema.WaitOne();
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            Interlocked.Decrement(ref _count);
            lock (_theQueue) return _theQueue.Dequeue();
        }

        /// <summary>
        /// Counts how many objects are available
        /// </summary>
        public int Count
        {
            get { return (int) Interlocked.Read(ref _count); }
        }

        /// <summary>
        /// Extracts all the objects from the queue
        /// </summary>
        /// <returns></returns>
        public T[] Flush()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            lock (_theQueue)
            {
                T[] ret = _theQueue.ToArray();
                _theQueue.Clear();
                Interlocked.Exchange(ref _count, 0);

                //Note: we will need to change the behaviour soon
                _sema = new Semaphore(0, int.MaxValue);
                return ret;
            }
        }

        /// <summary>
        /// Extracts all the objects from the queue and disposes of the queue
        /// </summary>
        /// <returns></returns>
        public T[] FlushAndDispose()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            try
            {
                lock (_theQueue)
                {
                    T[] ret = _theQueue.ToArray();
                    _theQueue.Clear();
                    return ret;
                }
            }
            finally
            {
                Dispose();
            }
        }

        #region IDisposable Membri di

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);

            _sema.Close();
            if (disposing)
            {
            }

            _disposed = true;
        }

        /// <remarks/>
        public void Dispose()
        {
            if (_disposed) return;
            Dispose(true);
        }

        #endregion
    }
}