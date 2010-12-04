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


using System;
using System.Collections.Generic;
using It.Unina.Dis.Logbus.Filters;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Defines an Outbound channel.
    /// Basically, it is an entity that will forward to all the subscribed clients all the messages that match the filter.
    /// Clients subscribe to the channel obtaining a Client ID for future use. Subscription could be subject to refresh (if subscription expires, it gets truncated).
    /// Subscribed clients may at any time unsubscribe the channel to stop receiving messages.
    /// 
    /// When subscribing, clients choose a transport option by ID and, if required by the transport's design contract, submit input instructions (ie. ip and port).
    /// The clients may, if required by the transport's design contract, receive further instructions (ie. multicast group to join)
    /// </summary>
    public interface IOutboundChannel
        : ILogCollector, ILogSource, IRunnable, IDisposable
    {
        /// <summary>
        /// Channel's unique ID. Must be unique for each instance of ILogBus
        /// </summary>
        string ID { get; set; }

        /// <summary>
        /// Channel's human-readable descriptive name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Channel's full human-readable description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Number of subscribed clients
        /// </summary>
        int SubscribedClients { get; }

        /// <summary>
        /// Filter that needs to be matched by messages for actual forwarding
        /// </summary>
        IFilter Filter { get; set; }

        /// <summary>
        /// Coalescence window (milliseconds).
        /// After a message has been forwarded, no other message will be forwarded until the time window expires
        /// </summary>
        /// <value>Number of milliseconds to wait. 0 means no coalescence window is set</value>
        ulong CoalescenceWindowMillis { get; set; }

        /// <summary>
        /// Sets the transport factory helper
        /// </summary>
        ITransportFactoryHelper TransportFactoryHelper { set; }

        /// <summary>
        /// Subscribes a new client and assigns a new ID
        /// </summary>
        /// <param name="transportId">Standard ID for transport</param>
        /// <param name="inputInstructions">Parameters generated by the client that will be used by transport to customize delivery. Requirement and semantics is defined by design contract</param>
        /// <param name="outputInstructions">Parameters generated by the transport that will be used by the client to customize delivery. Requirement and semantics is defined by design contract</param>
        /// <returns>Client ID in channel-specific format</returns>
        /// <exception cref="System.ArgumentNullException">Transport ID is null</exception>
        /// <exception cref="System.ArgumentException">Argument is invalid</exception>
        /// <exception cref="It.Unina.Dis.Logbus.OutTransports.TransportException">Exception reported by transport (ie. bad parameters)</exception>
        /// <exception cref="LogbusException"></exception>
        string SubscribeClient(string transportId, IEnumerable<KeyValuePair<string, string>> inputInstructions, out IEnumerable<KeyValuePair<string, string>> outputInstructions);

        /// <summary>
        /// Refreshes the subscription of an already-subscribed client
        /// </summary>
        /// <param name="clientId">Client ID as returned by <see cref="SubscribeClient"/></param>
        /// <exception cref="System.ArgumentNullException">Argument is null</exception>
        /// <exception cref="System.ArgumentException">Argument is invalid</exception>
        /// <exception cref="LogbusException"></exception>
        /// <exception cref="System.InvalidOperationException">Client is not subscribed (or already expired)</exception>
        /// <exception cref="It.Unina.Dis.Logbus.OutTransports.TransportException">Exception reported by transport (ie. refresh not supported)</exception>
        void RefreshClient(string clientId);

        /// <summary>
        /// Unsubscribes a client
        /// </summary>
        /// <param name="clientId">Client ID as returned by <see cref="SubscribeClient"/></param>
        /// <exception cref="System.ArgumentNullException">Argument is null</exception>
        /// <exception cref="System.ArgumentException">Argument is invalid</exception>
        /// <exception cref="LogbusException"></exception>
        /// <exception cref="It.Unina.Dis.Logbus.OutTransports.TransportException">Exception reported by transport (ie. client not found)</exception>
        void UnsubscribeClient(string clientId);
    }
}