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
using System.Threading;
using It.Unina.Dis.Logbus.Utils;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
namespace It.Unina.Dis.Logbus.InChannels
{
    /// <summary>
    /// Base class for Inbound channels.
    /// Implements multithreaded message forwarding
    /// </summary>
    public abstract class ReceiverBase
        : IInboundChannel, IAsyncRunnable, ILogSupport
    {

        #region Constructor/Destructor
        /// <summary>
        /// Initializes a new instance of SyslogUdpReceiver
        /// </summary>
        public ReceiverBase()
        {
            startDelegate = new ThreadStart(Start);
            stopDelegate = new ThreadStart(Stop);
        }

        /// <summary>
        /// Default destructor. Releases resources
        /// </summary>
        ~ReceiverBase()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);

            try
            {
                Stop();
            }
            catch (Exception) { }

            if (disposing)
            {
            }
            Disposed = true;
        }

        /// <summary>
        /// Whether the object has been disposed or not
        /// </summary>
        protected bool Disposed
        {
            get;
            private set;
        }
        #endregion

        /// <summary>
        /// Whether the channel facility is running or not
        /// </summary>
        protected bool Running
        {
            get;
            private set;
        }

        private Thread[] running_threads;
        private BlockingFifoQueue<SyslogMessage>[] queues;

        /// <summary>
        /// Number of worker threads concurrently listening for datagrams
        /// </summary>
        public const int WORKER_THREADS = 4;

        #region IInboundChannel Membri di

        /// <summary>
        /// Implements IInboundChannel.Name
        /// </summary>
        public virtual string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Implements IInboundChannel.ParseError
        /// </summary>
        public event System.EventHandler<ParseErrorEventArgs> ParseError;

        #endregion

        #region ILogSource Membri di
        /// <summary>
        /// Implements ILogSource.MessageReceived
        /// </summary>
        public event System.EventHandler<SyslogMessageEventArgs> MessageReceived;

        #endregion

        #region IRunnable Membri di

        /// <summary>
        /// Implements IRunnable.Starting
        /// </summary>
        public event System.EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        /// <summary>
        /// Implements IRunnable.Stopping
        /// </summary>
        public event System.EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        /// <summary>
        /// Implements IRunnable.Started
        /// </summary>
        public event System.EventHandler Started;

        /// <summary>
        /// Implements IRunnable.Stopped
        /// </summary>
        public event System.EventHandler Stopped;

        /// <summary>
        /// Implements IRunnable.Error
        /// </summary>
        public event System.UnhandledExceptionEventHandler Error;

        /// <summary>
        /// Implements IRunnable.Start
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            Log.Info(string.Format("Inbound channel {0} starting", Name));
            try
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                if (Running) throw new InvalidOperationException("Listener is already running");

                if (Starting != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Starting(this, e);
                    if (e.Cancel) return;
                }

                OnStart();

                Running = true;
                queues = new BlockingFifoQueue<SyslogMessage>[WORKER_THREADS];

                running_threads = new Thread[WORKER_THREADS];
                for (int i = 0; i < WORKER_THREADS; i++)
                {
                    queues[i] = new BlockingFifoQueue<SyslogMessage>();
                    running_threads[i] = new Thread(QueueLoop);
                    running_threads[i].IsBackground = true;
                    running_threads[i].Name = string.Format(CultureInfo.InvariantCulture, "ReceiverBase[{1}].QueueLoop[{0}]", i, Name);
                    running_threads[i].Start(i);
                }

                if (Started != null) Started(this, EventArgs.Empty);
                Log.Info(string.Format("Inbound channel {0} started", Name));
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                Log.Error(string.Format("Unable to start inbound channel {0}", Name));
                throw;
            }
        }

        /// <summary>
        /// Implements IRunnable.Stop
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            Log.Info(string.Format("Inbound channel {0} stopping", Name));
            try
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                if (!Running) throw new InvalidOperationException("Listener is not running");

                if (Stopping != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Stopping(this, e);
                    if (e.Cancel) return;
                }

                Running = false;

                for (int i = 0; i < WORKER_THREADS; i++)
                    running_threads[i].Join();
                running_threads = null;

                OnStop();

                if (Stopped != null) Stopped(this, EventArgs.Empty);
                Log.Info(string.Format("Inbound channel {0} stopped", Name));
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                Log.Error(string.Format("Unable to stop inbound channel {0}", Name));
                throw;
            }
        }

        #endregion

        #region IConfigurable Membri di

        /// <summary>
        /// Implements IConfigurable.GetConfigurationParameter
        /// </summary>
        public abstract string GetConfigurationParameter(string key);

        /// <summary>
        /// Implements IConfigurable.SetConfigurationParameter
        /// </summary>
        public abstract void SetConfigurationParameter(string key, string value);

        /// <summary>
        /// Implements IConfigurable.Configuration
        /// </summary>
        public abstract System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Configuration
        { set; }

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


        #region Abstract methods

        /// <summary>
        /// Executes class-specific actions upon start
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Executes class-specific actions upon stop
        /// </summary>
        protected abstract void OnStop();


        #endregion

        #region IAsyncRunnable Membri di

        IAsyncResult IAsyncRunnable.BeginStart()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            return startDelegate.BeginInvoke(null, null);
        }

        void IAsyncRunnable.EndStart(IAsyncResult result)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            startDelegate.EndInvoke(result);
        }

        IAsyncResult IAsyncRunnable.BeginStop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            return stopDelegate.BeginInvoke(null, null);
        }

        void IAsyncRunnable.EndStop(IAsyncResult result)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            stopDelegate.EndInvoke(result);
        }

        private ThreadStart startDelegate, stopDelegate;

        #endregion

        /// <summary>
        /// Enqueues a message for forwarding
        /// </summary>
        /// <param name="msg"></param>
        protected void ForwardMessage(SyslogMessage msg)
        {
            msg.AdjustTimestamp();
            queues[Environment.TickCount % WORKER_THREADS].Enqueue(msg);
        }

        private void QueueLoop(object queue_id)
        {
            int id = (int)queue_id;
            while (Running)
            {
                try
                {
                    SyslogMessage new_message = queues[id].Dequeue();
                    if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(new_message));
                }
                catch (Exception) { } //Really do nothing? Shouldn't we stop the service?
            }
        }

        /// <summary>
        /// Fires ParseError event
        /// </summary>
        /// <param name="e"></param>
        protected void OnParseError(ParseErrorEventArgs e)
        {
            if (ParseError != null) ParseError(this, e);
        }

        #region ILogSupport Membri di

        public It.Unina.Dis.Logbus.Loggers.ILog Log
        {
            get;
            set;
        }

        #endregion
    }
}
