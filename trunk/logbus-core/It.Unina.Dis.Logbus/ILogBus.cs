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
using It.Unina.Dis.Logbus.Filters;
using System.ComponentModel;
namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// This interface represents the Logbus core service
    /// </summary>
    public interface ILogBus
        : ILogSource, ILogCollector, IDisposable
    {

        #region Events
        /// <summary>
        /// Logbus is starting
        /// </summary>
        event EventHandler<CancelEventArgs> Starting;

        /// <summary>
        /// Logbus is stopping
        /// </summary>
        event EventHandler<CancelEventArgs> Stopping;

        /// <summary>
        /// Logbus has started
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Logbus has stopped
        /// </summary>
        event EventHandler Stopped;

        /// <summary>
        /// An error occurred in the core
        /// </summary>
        event UnhandledExceptionEventHandler Error;
        #endregion

        /// <summary>
        /// Lists the available transports by tag
        /// </summary>
        string[] AvailableTransports { get; }

        /// <summary>
        /// Outbound channels available for subscription
        /// </summary>
        IList<IOutboundChannel> OutboundChannels { get; }

        /// <summary>
        /// Inbound channels created by configuration
        /// </summary>
        IList<IInboundChannel> InboundChannels { get; }

        /// <summary>
        /// Helper class for transport instantiation
        /// </summary>
        ITransportFactoryHelper TransportFactoryHelper { get; set; }

        /// <summary>
        /// Starts Logbus
        /// </summary>
        void Start();

        /// <summary>
        /// Stops Logbus
        /// </summary>
        void Stop();

        /// <summary>
        /// Core filter.
        /// Only messages matching it will be forwarded to subscribed clients
        /// </summary>
        IFilter MainFilter { get; set; }
    }
}
