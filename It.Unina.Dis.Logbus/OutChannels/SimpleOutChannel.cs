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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Loggers;
using It.Unina.Dis.Logbus.OutTransports;
using It.Unina.Dis.Logbus.Utils;

namespace It.Unina.Dis.Logbus.OutChannels
{
    /// <summary>
    /// Simple implementation of IOutboundChannel
    /// </summary>
    internal sealed class SimpleOutChannel
        : IOutboundChannel, IAsyncRunnable, ILogSupport
    {
        private readonly Dictionary<string, IOutboundTransport> _transports;

        private Timer _coalescenceTimer;
        private Thread _workerThread;
        private readonly IFifoQueue<SyslogMessage> _messageQueue;
        private readonly ReaderWriterLock _transportLock;
        private const int DEFAULT_JOIN_TIMEOUT = 5000;
        private readonly Timer _statistics;
        private int _processedMessages, _deliveredMessages;

        private volatile bool _withinCoalescenceWindow;

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
            _transportLock = new ReaderWriterLock();
            _transports = new Dictionary<string, IOutboundTransport>();
            _messageQueue = new FastFifoQueue<SyslogMessage>(16384);

            _statistics = new Timer(LogStatistics, null, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));
        }

        ~SimpleOutChannel()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            GC.SuppressFinalize(this);

            try
            {
                Stop();
            }
            catch
            {
            }

            _statistics.Dispose();

            if (disposing)
            {
                _transportLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    foreach (KeyValuePair<string, IOutboundTransport> trans in _transports) trans.Value.Dispose();
                }
                finally
                {
                    _transportLock.ReleaseReaderLock();
                }
                _messageQueue.Dispose();
            }

            Disposed = true;
        }

        private bool Disposed { get; set; }

        #endregion

        #region IOutboundChannel Membri di

        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        public string ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (_block || _withinCoalescenceWindow || !Running) return; //Discard message
            _messageQueue.Enqueue(message);
        }

        public int SubscribedClients
        {
            get
            {
                int ret = 0;
                _transportLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    foreach (KeyValuePair<string, IOutboundTransport> kvp in _transports)
                        try
                        {
                            ret += kvp.Value.SubscribedClients;
                        }
                        catch (ObjectDisposedException) { }
                }
                finally
                {
                    _transportLock.ReleaseReaderLock();
                }

                if (MessageReceived != null)
                    ret += MessageReceived.GetInvocationList().Length;

                return ret;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (Running) throw new InvalidOperationException("Channel is already started");

            try
            {
                Log.Info("Channel {0} starting", ID);
                if (Starting != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Starting(this, e);
                    if (e.Cancel) return;
                }

                Running = true;
                _workerThread = new Thread(RunnerLoop)
                                    {
                                        Name = "SimpleOutChannel.RunnerLoop",
                                        IsBackground = true
                                    };
                _workerThread.Start();

                if (Started != null) Started(this, EventArgs.Empty);
                Log.Info("Channel {0} started", ID);

                _block = SubscribedClients == 0;
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
            if (!Running) throw new InvalidOperationException("Channel is not running");

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
                Running = false;

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

        public IFilter Filter
        {
            get;
            set;
        }

        public ulong CoalescenceWindowMillis
        {
            get;
            set;
        }

        public ITransportFactoryHelper TransportFactoryHelper
        {
            private get;
            set;
        }

        public string SubscribeClient(string transportId, IEnumerable<KeyValuePair<string, string>> inputInstructions,
                                      out IEnumerable<KeyValuePair<string, string>> outputInstructions)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(transportId))
                throw new ArgumentNullException("transportId", "Transport ID cannot be null");

            try
            {
                Log.Info("New client subscribing on channel {0}", ID);
                IOutboundTransport toSubscribe;
                _transportLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    if (_transports.ContainsKey(transportId))
                    {
                        toSubscribe = _transports[transportId];
                    }
                    else
                    {
                        try
                        {
                            toSubscribe = TransportFactoryHelper.GetFactory(transportId).CreateTransport();
                            Log.Debug("Created new instance of transport type {0} for channel {1}, ID {2}", transportId, Name, toSubscribe.GetHashCode());
                        }
                        catch (NotSupportedException e)
                        {
                            throw new LogbusException("Given transport is not supported on this node", e);
                        }

                        LockCookie ck = _transportLock.UpgradeToWriterLock(DEFAULT_JOIN_TIMEOUT);
                        try
                        {
                            _transports.Add(transportId, toSubscribe);
                        }
                        finally
                        {
                            _transportLock.DowngradeFromWriterLock(ref ck);
                        }
                    }
                }
                finally
                {
                    _transportLock.ReleaseReaderLock();
                }

                try
                {
                    _block = false;
                    string clientId = string.Format("{0}:{1}", transportId,
                                                    toSubscribe.SubscribeClient(inputInstructions,
                                                                                out outputInstructions));
                    Log.Info("New client subscribed on channel {0} with ID {1}", ID, clientId);
                    return clientId;
                }
                catch (ObjectDisposedException)
                {
                    _transportLock.AcquireWriterLock(DEFAULT_JOIN_TIMEOUT);
                    try
                    {
                        _transports.Remove(transportId);
                    }
                    finally
                    {
                        _transportLock.ReleaseWriterLock();
                    }

                    return SubscribeClient(transportId, inputInstructions, out outputInstructions);
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

        public void RefreshClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException("clientId", "Client ID must not be null");
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
            _transportLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            IOutboundTransport trans;
            try
            {
                if (!_transports.ContainsKey(transportId))
                {
                    ArgumentException ex = new ArgumentException("Invalid client ID");
                    ex.Data.Add("clientId-channel", clientId);
                    throw ex;
                }
                trans = _transports[transportId];
            }
            finally
            {
                _transportLock.ReleaseReaderLock();
            }


            try
            {
                try
                {
                    trans.RefreshClient(transportClientId);
                }
                catch (ObjectDisposedException)
                {
                    _transportLock.AcquireWriterLock(DEFAULT_JOIN_TIMEOUT);
                    try
                    {
                        _transports.Remove(transportId);
                    }
                    finally
                    {
                        _transportLock.ReleaseWriterLock();
                    }
                    throw new TransportException("Transport crashed");
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

        public void UnsubscribeClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException("clientId", "Client ID must not be null");

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
            IOutboundTransport trans;
            _transportLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                if (!_transports.ContainsKey(transportId))
                {
                    ArgumentException ex = new ArgumentException("Invalid client ID. Transport is not recognized or not active on this channel.");
                    ex.Data.Add("clientId-channel", clientId);
                    throw ex;
                }
                trans = _transports[transportId];
            }
            finally
            {
                _transportLock.ReleaseReaderLock();
            }


            try
            {
                try
                {
                    trans.UnsubscribeClient(transportClientId);

                    Log.Info("Client {0} unsubscribed from channel {1}", clientId, ID);
                    if (trans.SubscribedClients == 0)
                    {
                        _transportLock.AcquireWriterLock(DEFAULT_JOIN_TIMEOUT);
                        try
                        {
                            _transports.Remove(transportId);
                        }
                        finally
                        {
                            _transportLock.ReleaseWriterLock();
                        }
                        trans.Dispose();
                    }
                }
                catch (ObjectDisposedException)
                {
                    _transportLock.AcquireWriterLock(DEFAULT_JOIN_TIMEOUT);
                    try
                    {
                        _transports.Remove(transportId);
                    }
                    finally
                    {
                        _transportLock.ReleaseWriterLock();
                    }
                    throw new TransportException("Transport crashed");
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
        }

        #endregion

        #region IDisposable Membri di

        void IDisposable.Dispose()
        {
            if (Disposed) return;
            Dispose(true);
        }

        #endregion

        private void RunnerLoop()
        {
            try
            {
                while (Running)
                {
                    SyslogMessage msg = _messageQueue.Dequeue();
                    Interlocked.Increment(ref _processedMessages);
                    if (_withinCoalescenceWindow || SubscribedClients == 0 || !Filter.IsMatch(msg)) continue;

                    try
                    {
                        if (MessageReceived != null)
                            MessageReceived(this, new SyslogMessageEventArgs(msg));
                    }
                    catch (Exception ex)
                    {
                        if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, false));

                        Log.Error("Unable to forward messages from channel {0} via event", Name);
                        Log.Debug("Error details: {0}", ex.Message);
                    }

                    _transportLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                    try
                    {
                        foreach (KeyValuePair<string, IOutboundTransport> kvp in _transports)
                            try
                            {
                                kvp.Value.SubmitMessage(msg);
                            }
                            catch (ObjectDisposedException)
                            {
                                LockCookie ck = _transportLock.UpgradeToWriterLock(DEFAULT_JOIN_TIMEOUT);
                                try
                                {
                                    _transports.Remove(kvp.Key);
                                }
                                finally
                                {
                                    _transportLock.DowngradeFromWriterLock(ref ck);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, false));

                                Log.Error("Unable to forward messages from channel {0} to transport {1}", Name, kvp.Key);
                                Log.Debug("Error details: {0}", ex.Message);
                            }
                    }
                    finally
                    {
                        _transportLock.ReleaseReaderLock();
                    }
                    Interlocked.Increment(ref _deliveredMessages);

                    if (CoalescenceWindowMillis > 0)
                    {
                        _coalescenceTimer = new Timer(ResetCoalescence, null, (int)CoalescenceWindowMillis,
                                                      Timeout.Infinite);
                        _withinCoalescenceWindow = true;
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));

                Log.Alert("Unexpected error occurred in channel {0} background thread", Name);
                Log.Debug("Error details: {0}", ex.Message);

                throw; //This will cause the whole application to crash. At least we got the log trace
            }
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
                            try
                            {
                                if (MessageReceived != null)
                                    MessageReceived(this, new SyslogMessageEventArgs(msg));
                            }
                            catch { }

                            _transportLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                            try
                            {
                                foreach (KeyValuePair<string, IOutboundTransport> kvp in _transports)
                                    kvp.Value.SubmitMessage(msg);
                            }
                            catch { }
                            finally
                            {
                                _transportLock.ReleaseReaderLock();
                            }
                            if (CoalescenceWindowMillis > 0) break;
                        }
                }
            }
        }

        private void ResetCoalescence(object state)
        {
            _withinCoalescenceWindow = false;
        }

        public ILog Log { private get; set; }

        #region IRunnable Membri di

        /// <remarks/>
        public bool Running
        {
            get;
            private set;
        }

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

        private void LogStatistics(object state)
        {
            string[] transports;
            _transportLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                transports = new string[_transports.Count];
                _transports.Keys.CopyTo(transports, 0);
            }
            finally
            {
                _transportLock.ReleaseReaderLock();
            }

            Log.Debug("Statistics for channel {0} for the last minute. Processed {1} messages. Delivered {2} messages. In queue {3} messages. Active transports: {4}",
                ID,
                Interlocked.Exchange(ref _processedMessages, 0),
                Interlocked.Exchange(ref _deliveredMessages, 0),
                _messageQueue.Count,
                (transports.Length > 0) ? string.Join(",", transports) : "none"
                );
        }
    }
}