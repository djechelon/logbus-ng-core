﻿/*
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

using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System;

namespace It.Unina.Dis.Logbus.Utils
{
    /// <summary>
    /// The blocking FIFO queue holds elements the same as regular FIFO queue, but allows clients to wait until a new item is available.
    /// The class has a special destructor that allows to empty the queue
    /// </summary>
    /// <typeparam name="T">Type of object the queue will hold</typeparam>
    public sealed class BlockingFifoQueue<T>
        : IDisposable
    {
        private Queue<T> the_queue;
        private Semaphore sema;
        private volatile bool disposed_value;

        private bool Disposed
        {
            get
            {
                return disposed_value;
            }
            set
            {
                disposed_value = value;
            }
        }

        #region Constructor/Destructor

        /// <summary>
        /// Intializes the blocking FIFO queue
        /// </summary>
        public BlockingFifoQueue()
        {
            disposed_value = false;

            the_queue = new Queue<T>();
            sema = new Semaphore(0, int.MaxValue);
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
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            lock (the_queue)
                the_queue.Enqueue(item);
            sema.Release();
        }

        /// <summary>
        /// Dequeues an object from the queue as soon as it's available
        /// </summary>
        /// <returns>The first object enqueued</returns>
        public T Dequeue()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            sema.WaitOne();
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            lock (the_queue) return the_queue.Dequeue();
        }

        /// <summary>
        /// Counts how many objects are available
        /// </summary>
        public int Count
        {
            get { return the_queue.Count; }
        }

        /// <summary>
        /// Extracts all the objects from the queue
        /// </summary>
        /// <returns></returns>
        public T[] Flush()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            lock (the_queue)
            {
                T[] ret = the_queue.ToArray();
                the_queue.Clear();
                //Note: we will need to change the behaviour soon
                sema = new Semaphore(0, int.MaxValue);
                return ret;
            }
        }

        /// <summary>
        /// Extracts all the objects from the queue and disposes of the queue
        /// </summary>
        /// <returns></returns>
        public T[] FlushAndDispose()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            try
            {
                lock (the_queue)
                {
                    T[] ret = the_queue.ToArray();
                    the_queue.Clear();
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

            sema.Close();
            if (disposing)
            {
            }

            Disposed = true;
            GC.ReRegisterForFinalize(this);
        }

        /// <remarks/>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
