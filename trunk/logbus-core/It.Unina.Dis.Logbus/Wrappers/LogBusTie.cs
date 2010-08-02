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
namespace It.Unina.Dis.Logbus.Wrappers
{
    /// <summary>
    /// Proxy to ILogBus
    /// </summary>
    public class LogBusTie
        : MarshalByRefObject, ILogBus
    {

        private ILogBus target;

        /// <summary>
        /// Initializes the proxy with an existing instance of ILogBus
        /// </summary>
        /// <param name="targetInstance">ILogBus instance to use</param>
        public LogBusTie(ILogBus targetInstance)
        {
            if (targetInstance == null) throw new ArgumentNullException("targetInstance");
            target = targetInstance;
            target.Error += Error;
            target.MessageReceived += MessageReceived;
            target.Started += Started;
            target.Starting += Starting;
            target.Stopped += Stopped;
            target.Stopping += Stopping;
            target.OutChannelCreated += OutChannelCreated;
            target.OutChannelDeleted += OutChannelDeleted;
        }

        #region ILogBus Membri di

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public string[] AvailableTransports
        {
            get { return target.AvailableTransports; }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public System.Collections.Generic.IList<IOutboundChannel> OutboundChannels
        {
            get { return target.OutboundChannels; }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public System.Collections.Generic.IList<IInboundChannel> InboundChannels
        {
            get { return target.InboundChannels; }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public ITransportFactoryHelper TransportFactoryHelper
        {
            get
            {
                return target.TransportFactoryHelper;
            }
            set
            {
                target.TransportFactoryHelper = value;
            }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public It.Unina.Dis.Logbus.Filters.IFilter MainFilter
        {
            get
            {
                return target.MainFilter;
            }
            set
            {
                target.MainFilter = value;
            }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void CreateChannel(string id, string name, It.Unina.Dis.Logbus.Filters.IFilter filter, string description, long coalescenceWindow)
        {
            target.CreateChannel(id, name, filter, description, coalescenceWindow);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void RemoveChannel(string id)
        {
            target.RemoveChannel(id);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public string SubscribeClient(string channelId, string transportId, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> transportInstructions, out System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> clientInstructions)
        {
            return target.SubscribeClient(channelId, transportId, transportInstructions, out clientInstructions);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void RefreshClient(string clientId)
        {
            target.RefreshClient(clientId);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public void UnsubscribeClient(string clientId)
        {
            target.UnsubscribeClient(clientId);
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public IEnumerable<IPlugin> Plugins
        {
            get
            {
                return target.Plugins;
            }
        }

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public event EventHandler<It.Unina.Dis.Logbus.OutChannels.OutChannelCreationEventArgs> OutChannelCreated;

        /// <summary>
        /// Required by ILogBus
        /// </summary>
        public event EventHandler<It.Unina.Dis.Logbus.OutChannels.OutChannelDeletionEventArgs> OutChannelDeleted;
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
            target.SubmitMessage(message);
        }

        #endregion

        #region IRunnable Membri di

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

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
            target.Start();
        }

        /// <summary>
        /// Required by IRunnable
        /// </summary>
        public void Stop()
        {
            target.Stop();
        }

        #endregion

        #region IDisposable Membri di

        /// <summary>
        /// Required by IDisposable
        /// </summary>
        public void Dispose()
        {
            target.Dispose();
        }

        #endregion

    }
}
