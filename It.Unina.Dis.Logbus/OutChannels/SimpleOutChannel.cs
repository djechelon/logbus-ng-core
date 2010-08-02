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

using It.Unina.Dis.Logbus.Filters;
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
        : IOutboundChannel, IAsyncRunnable
    {

        private Dictionary<string, IOutboundTransport> transports = new Dictionary<string, IOutboundTransport>();
        private Timer coalescence_timer;
        private Thread worker_thread;
        private BlockingFifoQueue<SyslogMessage> message_queue;

        private volatile bool withinCoalescenceWindow;
        private volatile bool running;


        #region Constructor/Destructor

        public SimpleOutChannel()
        {
            startDelegate = new ThreadStart(Start);
            stopDelegate = new ThreadStart(Stop);
        }

        ~SimpleOutChannel()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) return;
            if (running)
                ((IOutboundChannel)this).Stop();


            if (disposing)
            {
                foreach (KeyValuePair<string, IOutboundTransport> trans in transports) trans.Value.Dispose();
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

        string IOutboundChannel.ID
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
            message_queue.Enqueue(message);
        }

        int IOutboundChannel.SubscribedClients
        {
            get
            {
                int ret = 0;
                foreach (KeyValuePair<string, IOutboundTransport> kvp in transports) ret += kvp.Value.SubscribedClients;
                return ret;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            try
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                if (running) throw new InvalidOperationException("Channel is already started");

                if (Starting != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Starting(this, e);
                    if (e.Cancel) return;
                }

                message_queue = new BlockingFifoQueue<SyslogMessage>();

                worker_thread = new Thread(RunnerLoop);
                worker_thread.IsBackground = true;
                worker_thread.Start();

                running = true;

                if (Started != null) Started(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            try
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                if (!running) throw new InvalidOperationException("Channel is not running");

                if (Stopping != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Stopping(this, e);
                    if (e.Cancel) return;
                }

                //Tell the thread to stop, the good way
                running = false;
                if (worker_thread.ThreadState == ThreadState.WaitSleepJoin)
                    worker_thread.Join(5000); //Wait if the thread is already doing something "useful"
                if (worker_thread.ThreadState != ThreadState.Stopped)
                {
                    //Thread was locked. Going by brute force!!!
                    try
                    {
                        Thread.BeginCriticalRegion();
                        worker_thread.Abort();
                    }
                    finally
                    {
                        Thread.EndCriticalRegion();
                    }
                    worker_thread.Join(); //Giving it all the time it needs
                }
                message_queue = null;

                if (Stopped != null) Stopped(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        public It.Unina.Dis.Logbus.Filters.IFilter Filter
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


        public string SubscribeClient(string transportId, IEnumerable<KeyValuePair<string, string>> inputInstructions, out IEnumerable<KeyValuePair<string, string>> outputInstructions)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(transportId)) throw new ArgumentNullException("transportId", "Transport ID cannot be null");

            IOutboundTransport to_subscribe;
            if (transports.ContainsKey(transportId))
            {
                to_subscribe = transports[transportId];
            }
            else
            {
                try
                {
                    to_subscribe = TransportFactoryHelper.GetFactory(transportId).CreateTransport();
                }
                catch (NotSupportedException e)
                {
                    throw new LogbusException("Given transport is not supported on this node", e);
                }
                transports.Add(transportId, to_subscribe);
            }

            try
            {
                return string.Format("{0}:{1}", transportId, to_subscribe.SubscribeClient(inputInstructions, out outputInstructions));
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

        public void RefreshClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("Client ID must not be null");
            int indexof = clientId.IndexOf(':');
            if (indexof < 0)
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }

            string transport_id = clientId.Substring(0, indexof), transport_client_id = clientId.Substring(indexof + 1);
            if (string.IsNullOrEmpty(transport_id))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }


            //First find the transport
            if (!transports.ContainsKey(transport_id))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }
            IOutboundTransport trans = transports[transport_id];


            try
            {
                trans.RefreshClient(transport_client_id);
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

        public void UnsubscribeClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("Client ID must not be null");
            int indexof = clientId.IndexOf(':');
            if (indexof < 0)
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }

            string transport_id = clientId.Substring(0, indexof), transport_client_id = clientId.Substring(indexof + 1);
            if (string.IsNullOrEmpty(transport_id))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }


            //First find the transport
            if (!transports.ContainsKey(transport_id))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-channel", clientId);
                throw ex;
            }
            IOutboundTransport trans = transports[transport_id];


            try
            {
                trans.UnsubscribeClient(transport_client_id);
                if (trans.SubscribedClients == 0)
                {
                    transports.Remove(transport_id);
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


        #endregion

        #region IDisposable Membri di

        void System.IDisposable.Dispose()
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
                while (running)
                {
                    SyslogMessage msg = message_queue.Dequeue();
                    if (!withinCoalescenceWindow)
                        if (Filter.IsMatch(msg))
                        {
                            if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(msg));
                            foreach (KeyValuePair<string, IOutboundTransport> kvp in transports)
                                kvp.Value.SubmitMessage(msg);
                            if (CoalescenceWindowMillis > 0)
                            {
                                coalescence_timer = new Timer(ResetCoalescence, null, (int)CoalescenceWindowMillis, Timeout.Infinite);
                                withinCoalescenceWindow = true;
                            }
                        }
                }
            }
            catch (ThreadAbortException)
            { }
            finally
            {
                if (coalescence_timer != null) coalescence_timer.Dispose();
                //Someone is telling me to stop
                //Flush and terminate
                IEnumerable<SyslogMessage> left_messages = message_queue.FlushAndDispose();

                if (!withinCoalescenceWindow)
                {
                    foreach (SyslogMessage msg in left_messages)
                        if (Filter.IsMatch(msg))
                        {
                            if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(msg));
                            foreach (KeyValuePair<string, IOutboundTransport> kvp in transports)
                                kvp.Value.SubmitMessage(msg);
                            if (CoalescenceWindowMillis > 0) break;
                        }
                }
            }
        }

        private void ResetCoalescence(object state)
        {
            withinCoalescenceWindow = false;
        }

        private ILog Log
        {
            get;
            set;
        }

        #region IRunnable Membri di

        /// <remarks/>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        /// <remarks/>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

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
    }
}

