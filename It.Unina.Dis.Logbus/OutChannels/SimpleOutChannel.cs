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

namespace It.Unina.Dis.Logbus.OutChannels
{
    internal sealed class SimpleOutChannel
        : IOutboundChannel
    {

        private Dictionary<string, IOutboundTransport> transports = new Dictionary<string, IOutboundTransport>();
        private Timer coalescence_timer;
        private Thread worker_thread;
        private BlockingFifoQueue<SyslogMessage> message_queue;

        #region Support properties

        private bool Coalesced
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private bool Started
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
            message_queue = new BlockingFifoQueue<SyslogMessage>();
        }

        ~SimpleOutChannel()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
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
            message_queue.Enqueue(message);
        }

        int IOutboundChannel.SubscribedClients
        {
            get { throw new System.NotImplementedException(); }
        }

        void IOutboundChannel.Start()
        {
            if (Started) throw new InvalidOperationException("Channel is already started");

            worker_thread = new Thread(RunnerLoop);
            worker_thread.IsBackground = true;
            worker_thread.Start();

            throw new System.NotImplementedException();
        }

        void IOutboundChannel.Stop()
        {
            if (!Started) throw new InvalidOperationException("Channel is not running");

            throw new System.NotImplementedException();

            Started = false;
            worker_thread.Join();
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
            throw new NotImplementedException();
        }

        public void RefreshClient(string clientId)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeClient(string clientId)
        {
            throw new NotImplementedException();
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
            Started = true;

            while (Started)
            {
                //Will cause deadlock if running Stop() when queue is empty!!!!!!!!!!!!!!!

                SyslogMessage msg = message_queue.Dequeue();
            }

        }

    }
}
