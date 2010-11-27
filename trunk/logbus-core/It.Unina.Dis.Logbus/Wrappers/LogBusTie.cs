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
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.OutChannels;

namespace It.Unina.Dis.Logbus.Wrappers
{
    /// <summary>
    /// Proxy to ILogBus
    /// </summary>
    public class LogBusTie
        : MarshalByRefObject, ILogBus
    {
        private readonly ILogBus _target;

        /// <summary>
        /// Initializes the proxy with an existing instance of ILogBus
        /// </summary>
        /// <param name="targetInstance">ILogBus instance to use</param>
        public LogBusTie(ILogBus targetInstance)
        {
            if (targetInstance == null) throw new ArgumentNullException("targetInstance");
            _target = targetInstance;
            _target.Error += Error;
            _target.MessageReceived += MessageReceived;
            _target.Started += Started;
            _target.Starting += Starting;
            _target.Stopped += Stopped;
            _target.Stopping += Stopping;
            _target.OutChannelCreated += OutChannelCreated;
            _target.OutChannelDeleted += OutChannelDeleted;
            _target.ClientSubscribed += ClientSubscribed;
            _target.ClientSubscribing += ClientSubscribing;
            _target.ClientUnsubscribed += ClientUnsubscribed;
        }

        #region ILogBus Membri di

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public string[] AvailableTransports
        {
            get { return _target.AvailableTransports; }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public IList<IOutboundChannel> OutboundChannels
        {
            get { return _target.OutboundChannels; }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public IList<IInboundChannel> InboundChannels
        {
            get { return _target.InboundChannels; }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public ITransportFactoryHelper TransportFactoryHelper
        {
            get { return _target.TransportFactoryHelper; }
            set { _target.TransportFactoryHelper = value; }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public IFilter MainFilter
        {
            get { return _target.MainFilter; }
            set { _target.MainFilter = value; }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void CreateChannel(string channelId, string channelName, IFilter channelFilter, string channelDescription,
                                  long coalescenceWindow)
        {
            _target.CreateChannel(channelId, channelName, channelFilter, channelDescription, coalescenceWindow);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void RemoveChannel(string id)
        {
            _target.RemoveChannel(id);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public event EventHandler<ClientSubscribingEventArgs> ClientSubscribing;

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public event EventHandler<ClientSubscribedEventArgs> ClientSubscribed;

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public string SubscribeClient(string channelId, string transportId,
                                      IEnumerable<KeyValuePair<string, string>> transportInstructions,
                                      out IEnumerable<KeyValuePair<string, string>> clientInstructions)
        {
            return _target.SubscribeClient(channelId, transportId, transportInstructions, out clientInstructions);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void RefreshClient(string clientId)
        {
            _target.RefreshClient(clientId);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public event EventHandler<ClientUnsubscribedEventArgs> ClientUnsubscribed;

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void UnsubscribeClient(string clientId)
        {
            _target.UnsubscribeClient(clientId);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public IEnumerable<IPlugin> Plugins
        {
            get { return _target.Plugins; }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public event EventHandler<OutChannelCreationEventArgs> OutChannelCreated;

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public event EventHandler<OutChannelDeletionEventArgs> OutChannelDeleted;

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void AddOutboundChannel(IOutboundChannel channel)
        {
            _target.AddOutboundChannel(channel);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void AddInboundChannel(IInboundChannel channel)
        {
            _target.AddInboundChannel(channel);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void RemoveInboundChannel(IInboundChannel channel)
        {
            _target.RemoveInboundChannel(channel);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public IOutboundChannel GetOutboundChannel(string channelId)
        {
            return _target.GetOutboundChannel(channelId);
        }

        #endregion

        #region ILogSource Membri di

        /// <summary>
        /// Required by ILogSource
        /// </summary>
        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        #endregion

        #region ILogCollector Membri di

        /// <summary>
        /// Required by ILogCollector
        /// </summary>
        public void SubmitMessage(SyslogMessage message)
        {
            _target.SubmitMessage(message);
        }

        #endregion

        #region IRunnable Membri di

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public bool Running
        {
            get { return _target.Running; }
        }

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public event EventHandler<CancelEventArgs> Starting;

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public event EventHandler<CancelEventArgs> Stopping;

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public event UnhandledExceptionEventHandler Error;

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public void Start()
        {
            _target.Start();
        }

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public void Stop()
        {
            _target.Stop();
        }

        #endregion

        #region IDisposable Membri di

        /// <summary>
        /// Required by IDisposable
        /// </summary>
        public void Dispose()
        {
            _target.Dispose();
        }

        #endregion

        /// <remarks/>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}