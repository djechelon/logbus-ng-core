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

using It.Unina.Dis.Logbus.Filters;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;
using System;
using It.Unina.Dis.Logbus.Utils;

namespace It.Unina.Dis.Logbus.OutChannels
{
    internal sealed class SimpleOutChannel
        : IOutboundChannel, IAsyncRunnable
    {

        private Dictionary<string, IOutboundTransport> transports = new Dictionary<string, IOutboundTransport>();
        private Timer coalescence_timer;
        private Thread worker_thread;
        private BlockingFifoQueue<SyslogMessage> message_queue;
        private object objCoalescenceLock = new object();
        private bool inCoalescence;

        #region Support properties

        private bool WithinCoalescenceWindow
        {
            get { lock (objCoalescenceLock) return inCoalescence; }
            set { lock (objCoalescenceLock) inCoalescence = value; }
        }

        private bool Running
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }


        #endregion

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
            if (Running)
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

        public event SyslogMessageEventHandler MessageReceived;

        string IOutboundChannel.ID
        {
            get;
            set;
        }

        string IOutboundChannel.Name
        {
            get;
            set;
        }

        string IOutboundChannel.Description
        {
            get;
            set;
        }

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (!Running) throw new InvalidOperationException("Channel not started");
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
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (Running) throw new InvalidOperationException("Channel is already started");

            message_queue = new BlockingFifoQueue<SyslogMessage>();

            worker_thread = new Thread(RunnerLoop);
            worker_thread.IsBackground = true;
            worker_thread.Start();

            Running = true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (!Running) throw new InvalidOperationException("Channel is not running");

            //Tell the thread to stop, the good way
            Running = false;
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
                while (Running)
                {
                    SyslogMessage msg = message_queue.Dequeue();
                    if (!WithinCoalescenceWindow)
                        if (Filter.IsMatch(msg))
                        {
                            if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(msg));
                            foreach (KeyValuePair<string, IOutboundTransport> kvp in transports)
                                kvp.Value.SubmitMessage(msg);
                            if (CoalescenceWindowMillis > 0)
                            {
                                coalescence_timer = new Timer(ResetCoalescence, null, (int)CoalescenceWindowMillis, Timeout.Infinite);
                                WithinCoalescenceWindow = true;
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
                
                if (!WithinCoalescenceWindow)
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
            WithinCoalescenceWindow = false;
        }


        #region IRunnable Membri di

        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        public event EventHandler Started;

        public event EventHandler Stopped;

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

