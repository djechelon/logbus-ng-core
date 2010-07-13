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
namespace It.Unina.Dis.Logbus.Wrappers
{
    public class LogBusTie
        : MarshalByRefObject, ILogBus
    {

        private ILogBus target;

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
        }

        #region ILogBus Membri di

        public string[] AvailableTransports
        {
            get { return target.AvailableTransports; }
        }

        public System.Collections.Generic.IList<IOutboundChannel> OutboundChannels
        {
            get { return target.OutboundChannels; }
        }

        public System.Collections.Generic.IList<IInboundChannel> InboundChannels
        {
            get { return target.InboundChannels; }
        }

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

        public void CreateChannel(string id, string name, It.Unina.Dis.Logbus.Filters.IFilter filter, string description, long coalescenceWindow)
        {
            target.CreateChannel(id, name, filter, description, coalescenceWindow);
        }

        public void RemoveChannel(string id)
        {
            target.RemoveChannel(id);
        }

        public string SubscribeClient(string channelId, string transportId, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> transportInstructions, out System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> clientInstructions)
        {
            return target.SubscribeClient(channelId, transportId, transportInstructions, out clientInstructions);
        }

        public void RefreshClient(string clientId)
        {
            target.RefreshClient(clientId);
        }

        public void UnsubscribeClient(string clientId)
        {
            target.UnsubscribeClient(clientId);
        }

        #endregion

        #region ILogSource Membri di

        public event SyslogMessageEventHandler MessageReceived;

        #endregion

        #region ILogCollector Membri di

        public void SubmitMessage(SyslogMessage message)
        {
            target.SubmitMessage(message);
        }

        #endregion

        #region IRunnable Membri di

        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event UnhandledExceptionEventHandler Error;

        public void Start()
        {
            target.Start();
        }

        public void Stop()
        {
            target.Stop();
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            target.Dispose();
        }

        #endregion
    }
}
