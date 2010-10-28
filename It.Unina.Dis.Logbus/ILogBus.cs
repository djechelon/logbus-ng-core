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
using It.Unina.Dis.Logbus.OutChannels;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// This interface represents the Logbus core service
    /// </summary>
    public interface ILogBus
        : ILogSource, ILogCollector, IRunnable, IDisposable
    {
        /// <summary>
        /// Lists the available transports by tag
        /// </summary>
        string[] AvailableTransports { get; }

        /// <summary>
        /// Adds an already-created outbound channel to the Logbus instance
        /// </summary>
        /// <param name="channel">Channel to add to the bus</param>
        void AddOutboundChannel(IOutboundChannel channel);

        /// <summary>
        /// Finds an outbound channel by ID
        /// </summary>
        /// <param name="channelId">ID of channel to find</param>
        /// <returns>Outbound channel matching ID</returns>
        /// <exception cref="System.ArgumentNullException"><c>channelId</c> is null or empty</exception>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">No channel matches given ID</exception>
        IOutboundChannel GetOutboundChannel(string channelId);

        /// <summary>
        /// Outbound channels available for subscription
        /// </summary>
        IList<IOutboundChannel> OutboundChannels { get; }

        /// <summary>
        /// Adds an inbound channel
        /// </summary>
        /// <param name="channel">Channel to add</param>
        /// <exception cref="System.ArgumentNullException"><c>channel</c> is null</exception>
        void AddInboundChannel(IInboundChannel channel);

        /// <summary>
        /// Removes an inbound channel
        /// </summary>
        /// <param name="channel">Channel to remove</param>
        /// <exception cref="System.ArgumentNullException"><c>channel</c> is null</exception>
        void RemoveInboundChannel(IInboundChannel channel);

        /// <summary>
        /// Inbound channels created by configuration
        /// </summary>
        IList<IInboundChannel> InboundChannels { get; }

        /// <summary>
        /// Helper class for transport instantiation
        /// </summary>
        ITransportFactoryHelper TransportFactoryHelper { get; set; }

        /// <summary>
        /// Core filter.
        /// Only messages matching it will be forwarded to subscribed clients
        /// </summary>
        IFilter MainFilter { get; set; }

        /// <summary>
        /// A new channel has been created
        /// </summary>
        event EventHandler<OutChannelCreationEventArgs> OutChannelCreated;

        /// <summary>
        /// An existing channel has been deleted
        /// </summary>
        event EventHandler<OutChannelDeletionEventArgs> OutChannelDeleted;

        /// <summary>
        /// Creates a new outbound channel
        /// </summary>
        /// <param name="channelId">Unique ID of channel</param>
        /// <param name="channelName">Descriptive name</param>
        /// <param name="channelFilter">Filter to apply</param>
        /// <param name="channelDescription">Human-readable description</param>
        /// <param name="coalescenceWindow">Coalescence window, in milliseconds</param>
        void CreateChannel(string channelId, string channelName, IFilter channelFilter, string channelDescription,
                           long coalescenceWindow);

        /// <summary>
        /// Removes a channel
        /// </summary>
        /// <param name="id">ID of channel to remove</param>
        void RemoveChannel(string id);

        /// <summary>
        /// A client is subscribing to a channel
        /// </summary>
        event EventHandler<ClientSubscribingEventArgs> ClientSubscribing;

        /// <summary>
        /// A client successfully subscribed a channel
        /// </summary>
        event EventHandler<ClientSubscribedEventArgs> ClientSubscribed;

        /// <summary>
        /// Subscribes a new client to the channel
        /// </summary>
        /// <param name="channelId">ID of channel</param>
        /// <param name="transportId">ID of transport to use</param>
        /// <param name="transportInstructions">Transport input instructions by client</param>
        /// <param name="clientInstructions">Client output instructions by transport</param>
        /// <returns></returns>
        string SubscribeClient(string channelId, string transportId,
                               IEnumerable<KeyValuePair<string, string>> transportInstructions,
                               out IEnumerable<KeyValuePair<string, string>> clientInstructions);

        /// <summary>
        /// Refreshes a client's subscription
        /// </summary>
        /// <param name="clientId">ID of client</param>
        /// <exception cref="System.InvalidOperationException">Client is not subscribed (or already expired)</exception>
        /// <exception cref="System.NotSupportedException">Transport doesn't support refreshing</exception>
        void RefreshClient(string clientId);

        /// <summary>
        /// A client unsubscribed from a channel
        /// </summary>
        event EventHandler<ClientUnsubscribedEventArgs> ClientUnsubscribed;

        /// <summary>
        /// Unsubscribes a client from a channel
        /// </summary>
        /// <param name="clientId">ID of client</param>
        void UnsubscribeClient(string clientId);

        /// <summary>
        /// Gets the plugins currently registered with Logbus-ng
        /// </summary>
        IEnumerable<IPlugin> Plugins { get; }
    }
}