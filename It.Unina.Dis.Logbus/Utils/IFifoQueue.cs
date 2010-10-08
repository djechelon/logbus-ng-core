using System;

namespace It.Unina.Dis.Logbus.Utils
{
    /// <summary>
    /// A synchronized FIFO queue
    /// </summary>
    /// <typeparam name="T">Type contained in the queue</typeparam>
    public interface IFifoQueue<T> : IDisposable
    {
        /// <summary>
        /// Enqueues an object in the queue
        /// </summary>
        /// <param name="item">Object to enqueue</param>
        void Enqueue(T item);

        /// <summary>
        /// Dequeues an object from the queue as soon as it's available
        /// </summary>
        /// <returns>The first object enqueued</returns>
        T Dequeue();

        /// <summary>
        /// Counts how many objects are available
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Extracts all the objects from the queue
        /// </summary>
        /// <returns></returns>
        T[] Flush();

        /// <summary>
        /// Extracts all the objects from the queue and disposes of the queue
        /// </summary>
        /// <returns></returns>
        T[] FlushAndDispose();

    }
}