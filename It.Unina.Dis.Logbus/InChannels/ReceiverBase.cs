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

#if X64
using COUNTER_TYPE = System.Int64;
#else
using COUNTER_TYPE = System.Int32;
#endif

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

        private Thread[] _queueThreads;
        private BlockingFifoQueue<SyslogMessage>[] _queues;
        private COUNTER_TYPE _currentQueue;

        #region Constructor/Destructor
        /// <summary>
        /// Initializes a new instance of SyslogUdpReceiver
        /// </summary>
        protected ReceiverBase()
        {
            _startDelegate = new ThreadStart(Start);
            _stopDelegate = new ThreadStart(Stop);
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
        public event EventHandler<CancelEventArgs> Starting;

        /// <summary>
        /// Implements IRunnable.Stopping
        /// </summary>
        public event EventHandler<CancelEventArgs> Stopping;

        /// <summary>
        /// Implements IRunnable.Started
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Implements IRunnable.Stopped
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Implements IRunnable.Error
        /// </summary>
        public event UnhandledExceptionEventHandler Error;

        /// <summary>
        /// Implements IRunnable.Start
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            Log.Info("Inbound channel {0} starting", Name);
            try
            {
                if (Running) throw new InvalidOperationException("Listener is already running");

                if (Starting != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Starting(this, e);
                    if (e.Cancel)
                    {
                        Log.Notice("Sarting of channel {0} cancelled", Name);
                        return;
                    }
                }

                Running = true;
                _queues = new BlockingFifoQueue<SyslogMessage>[WORKER_THREADS];
                _currentQueue = COUNTER_TYPE.MinValue;

                _queueThreads = new Thread[WORKER_THREADS];
                for (int i = 0; i < WORKER_THREADS; i++)
                {
                    _queues[i] = new BlockingFifoQueue<SyslogMessage>();
                    _queueThreads[i] = new Thread(QueueLoop)
                                           {
                                               IsBackground = true,
                                               Name =
                                                   string.Format(CultureInfo.InvariantCulture,
                                                                 "ReceiverBase[{1}].QueueLoop[{0}]", i, Name)
                                           };
                    _queueThreads[i].Start(i);
                }
                
                OnStart();

                if (Started != null) Started(this, EventArgs.Empty);
                Log.Info("Inbound channel {0} started", Name);
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                Log.Error("Unable to start inbound channel {0}", Name);
                Log.Debug("Eror details: {0}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Implements IRunnable.Stop
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            Log.Info("Inbound channel {0} stopping", Name);
            try
            {
                if (!Running) throw new InvalidOperationException("Listener is not running");

                if (Stopping != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Stopping(this, e);

                    if (e.Cancel)
                    {
                        Log.Notice("Stopping of channel {0} cancelled", Name);
                        return;
                    }
                }

                Running = false;

                OnStop();

                for (int i = 0; i < WORKER_THREADS; i++)
                    _queueThreads[i].Interrupt();

                for (int i = 0; i < WORKER_THREADS; i++)
                    _queueThreads[i].Join();

                _queueThreads = null;

                if (Stopped != null) Stopped(this, EventArgs.Empty);
                Log.Info("Inbound channel {0} stopped", Name);
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                Log.Error("Unable to stop inbound channel {0}", Name);
                Log.Debug("Error details: {0}", ex.Message);
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

            return _startDelegate.BeginInvoke(null, null);
        }

        void IAsyncRunnable.EndStart(IAsyncResult result)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            _startDelegate.EndInvoke(result);
        }

        IAsyncResult IAsyncRunnable.BeginStop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            return _stopDelegate.BeginInvoke(null, null);
        }

        void IAsyncRunnable.EndStop(IAsyncResult result)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            _stopDelegate.EndInvoke(result);
        }

        private readonly ThreadStart _startDelegate, _stopDelegate;

        #endregion

        /// <summary>
        /// Enqueues a message for forwarding
        /// </summary>
        /// <param name="msg"></param>
        protected void ForwardMessage(SyslogMessage msg)
        {
            msg.AdjustTimestamp();
            _queues[(((Interlocked.Increment(ref _currentQueue)) % WORKER_THREADS) + WORKER_THREADS) % WORKER_THREADS].Enqueue(msg);
        }

        private void QueueLoop(object queueId)
        {
            int id = (int)queueId;
            while (Running)
            {
                try
                {
                    SyslogMessage newMessage = _queues[id].Dequeue();
                    if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(newMessage));
                }
                catch (ThreadInterruptedException) { return; } //End thread
                catch (Exception ex)
                {
                    //Most probably the exception was thrown by a malformed event handler, anyway log it!
                    Log.Error("Unable to process Syslog message in channel {0}'s processing queue", Name);
                    Log.Debug("Error details: {0}", ex.Message);
                }
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

        public Loggers.ILog Log
        {
            private get;
            set;
        }

        #endregion
    }
}
