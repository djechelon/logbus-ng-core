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

using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;
using System;
using It.Unina.Dis.Logbus.Utils;
using It.Unina.Dis.Logbus.Loggers;
using System.ComponentModel;

namespace It.Unina.Dis.Logbus.OutChannels
{
    /// <summary>
    /// Simple implementation of IOutboundChannel
    /// </summary>
    internal sealed class SimpleOutChannel
        : IOutboundChannel, IAsyncRunnable, ILogSupport
    {

        private readonly Dictionary<string, IOutboundTransport> _transports = new Dictionary<string, IOutboundTransport>();
        private Timer _coalescenceTimer;
        private Thread _workerThread;
        private readonly IFifoQueue<SyslogMessage> _messageQueue = new BlockingFifoQueue<SyslogMessage>();

        private volatile bool _withinCoalescenceWindow, _running;

        /// <summary>
        /// Flag that blocks messages until the first subscription.
        /// Theorically, channels might block all messages when no clients
        /// are subscribed, but checking could add overhead (class needs to
        /// inspect transports' list of clients, involving some locks)
        /// </summary>
        private volatile bool _block = true;


        #region Constructor/Destructor

        public SimpleOutChannel()
        {
            _startDelegate = new ThreadStart(Start);
            _stopDelegate = new ThreadStart(Stop);
        }

        ~SimpleOutChannel()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (_running)
                Stop();


            if (disposing)
            {
                foreach (KeyValuePair<string, IOutboundTransport> trans in _transports) trans.Value.Dispose();
                _messageQueue.Dispose();
            }

            Disposed = true;
        }

        private bool Disposed
        {
            get;
            set;
        }
        #endregion

        #region IOutboundChannel Membri di

        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (_block || _withinCoalescenceWindow || !_running) return; //Discard message
            _messageQueue.Enqueue(message);
        }

        int IOutboundChannel.SubscribedClients
        {
            get
            {
                int ret = 0;
                foreach (KeyValuePair<string, IOutboundTransport> kvp in _transports) ret += kvp.Value.SubscribedClients;
                return ret;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (_running) throw new InvalidOperationException("Channel is already started");

            try
            {
                Log.Info("Channel {0} starting", ID);
                if (Starting != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Starting(this, e);
                    if (e.Cancel) return;
                }

                _workerThread = new Thread(RunnerLoop) { IsBackground = true };
                _workerThread.Start();

                _running = true;

                if (Started != null) Started(this, EventArgs.Empty);
                Log.Info("Channel {0} started", ID);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to start channel {0}", ID);
                Log.Error("Error details: {0}", ex);
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (!_running) throw new InvalidOperationException("Channel is not running");

            try
            {
                Log.Info("Channel {0} stopping", ID);
                if (Stopping != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Stopping(this, e);
                    if (e.Cancel) return;
                }

                //Tell the thread to stop, the good way
                _running = false;

                _workerThread.Interrupt();
                _workerThread.Join(5000); //Giving it all the time it needs

                if (Stopped != null) Stopped(this, EventArgs.Empty);
                Log.Info("Channel {0} stopped", ID);
            }
            catch (Exception ex)
            {
                Log.Error("Unable to stop channel {0}", ID);
                Log.Error("Error details: {0}", ex);
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        public Filters.IFilter Filter
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public ulong CoalescenceWindowMillis
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public ITransportFactoryHelper TransportFactoryHelper
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            private get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string SubscribeClient(string transportId, IEnumerable<KeyValuePair<string, string>> inputInstructions, out IEnumerable<KeyValuePair<string, string>> outputInstructions)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(transportId)) throw new ArgumentNullException("transportId", "Transport ID cannot be null");

            try
            {
                Log.Info("New client subscribing on channel {0}", ID);
                IOutboundTransport toSubscribe;
                if (_transports.ContainsKey(transportId))
                {
                    toSubscribe = _transports[transportId];
                }
                else
                {
                    try
                    {
                        toSubscribe = TransportFactoryHelper.GetFactory(transportId).CreateTransport();
                    }
                    catch (NotSupportedException e)
                    {
                        throw new LogbusException("Given transport is not supported on this node", e);
                    }
                    _transports.Add(transportId, toSubscribe);
                }

                try
                {
                    _block = false;
                    string clientId = string.Format("{0}:{1}", transportId,
                                         toSubscribe.SubscribeClient(inputInstructions, out outputInstructions));
                    Log.Info("New client subscribed on channel {0} with ID {1}", ID, clientId);
                    return clientId;
                }
                catch (LogbusException ex)
                {
                    ex.Data.Add("transportId", transportId);
                    throw;
                }
                catch (Exception e)
                {
                    LogbusException ex = new LogbusException("Could not subscribe transport", e);
                    ex.Data.Add("transportId", transportId);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to subscribe new client on channel {0}", ID);
                Log.Debug("Error details: {0}", ex.Message);
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RefreshClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId", "Client ID must not be null");
            int indexof = clientId.IndexOf(':');
            if (indexof < 0)
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }

            string transportId = clientId.Substring(0, indexof), transportClientId = clientId.Substring(indexof + 1);
            if (string.IsNullOrEmpty(transportId))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }


            //First find the transport
            if (!_transports.ContainsKey(transportId))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }
            IOutboundTransport trans = _transports[transportId];

            try
            {
                try
                {
                    trans.RefreshClient(transportClientId);
                }
                catch (NotSupportedException ex)
                {
                    ex.Data.Add("client-channel", clientId);
                    throw;
                }
                catch (LogbusException ex)
                {
                    ex.Data.Add("client-channel", clientId);
                    throw;
                }
                catch (Exception e)
                {
                    LogbusException ex = new LogbusException("Unable to refresh client", e);
                    ex.Data.Add("client-channel", clientId);
                    throw;
                }
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UnsubscribeClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId", "Client ID must not be null");
            int indexof = clientId.IndexOf(':');
            if (indexof < 0)
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }

            string transportId = clientId.Substring(0, indexof), transportClientId = clientId.Substring(indexof + 1);
            if (string.IsNullOrEmpty(transportId))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }


            //First find the transport
            if (!_transports.ContainsKey(transportId))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }
            IOutboundTransport trans = _transports[transportId];

            try
            {
                try
                {
                    trans.UnsubscribeClient(transportClientId);

                    Log.Info("Client {0} unsubscribed from channel {1}", clientId, ID);
                    if (trans.SubscribedClients == 0)
                    {
                        _transports.Remove(transportId);
                        trans.Dispose();
                    }
                }
                catch (LogbusException ex)
                {
                    ex.Data.Add("client-channel", clientId);
                    throw;
                }
                catch (Exception e)
                {
                    LogbusException ex = new LogbusException("Unable to unsubscribe client", e);
                    ex.Data.Add("client-channel", clientId);
                    throw;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to unsubscribe client {0} from channel {1}", clientId, ID);
                Log.Debug("Error details: {0}", ex.Message);
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
            finally
            {
                if (((IOutboundChannel)this).SubscribedClients == 0) _block = true;
            }
        }


        #endregion

        #region IDisposable Membri di

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// 
        private void RunnerLoop()
        {
            try
            {
                while (_running)
                {
                    SyslogMessage msg = _messageQueue.Dequeue();
                    if (!_withinCoalescenceWindow)
                        if (Filter.IsMatch(msg))
                        {
                            if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(msg));
                            foreach (KeyValuePair<string, IOutboundTransport> kvp in _transports)
                                kvp.Value.SubmitMessage(msg);
                            if (CoalescenceWindowMillis > 0)
                            {
                                _coalescenceTimer = new Timer(ResetCoalescence, null, (int)CoalescenceWindowMillis, Timeout.Infinite);
                                _withinCoalescenceWindow = true;
                            }
                        }
                }
            }
            catch (ThreadInterruptedException)
            { }
            finally
            {
                if (_coalescenceTimer != null) _coalescenceTimer.Dispose();
                //Someone is telling me to stop
                //Flush and terminate
                IEnumerable<SyslogMessage> leftMessages = _messageQueue.Flush();

                if (!_withinCoalescenceWindow)
                {
                    foreach (SyslogMessage msg in leftMessages)
                        if (Filter.IsMatch(msg))
                        {
                            if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(msg));
                            foreach (KeyValuePair<string, IOutboundTransport> kvp in _transports)
                                kvp.Value.SubmitMessage(msg);
                            if (CoalescenceWindowMillis > 0) break;
                        }
                }
            }
        }

        private void ResetCoalescence(object state)
        {
            _withinCoalescenceWindow = false;
        }

        public ILog Log
        {
            private get;
            set;
        }

        #region IRunnable Membri di

        /// <remarks/>
        public event EventHandler<CancelEventArgs> Starting;

        /// <remarks/>
        public event EventHandler<CancelEventArgs> Stopping;

        /// <remarks/>
        public event EventHandler Started;

        /// <remarks/>
        public event EventHandler Stopped;

        /// <remarks/>
        public event UnhandledExceptionEventHandler Error;

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
    }
}

