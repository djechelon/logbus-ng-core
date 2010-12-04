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

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Outbound transport. Implements the delivery logic for remote clients
    /// </summary>
    public interface IOutboundTransport
        : ILogCollector, IDisposable
    {
        /// <summary>
        /// Gets the number of clients subscribed to this transport within the channel
        /// </summary>
        int SubscribedClients { get; }

        /// <summary>
        /// Subscribes a new client to the transport within the channel
        /// </summary>
        /// <param name="inputInstructions">Input instructions for the channel</param>
        /// <param name="outputInstructions">Output instructions for the client</param>
        /// <returns>Client ID to use in refresh and unsubscribe call</returns>
        /// <exception cref="It.Unina.Dis.Logbus.OutTransports.TransportException">Unable to subscribe client</exception>
        string SubscribeClient(IEnumerable<KeyValuePair<string, string>> inputInstructions, out IEnumerable<KeyValuePair<string, string>> outputInstructions);

        /// <summary>
        /// Gets if the current transport requires client refresh
        /// </summary>
        bool RequiresRefresh { get; }

        /// <summary>
        /// Gets the subscription's time to live, in seconds
        /// </summary>
        /// <exception cref="System.NotSupportedException">Thrown when RequiresRefresh is <i>false</i></exception>
        long SubscriptionTtl { get; }

        /// <summary>
        /// Refreshes subscription, if required, to the specified client
        /// </summary>
        /// <param name="clientId">ID of the client to refresh</param>
        /// <exception cref="System.ArgumentNullException">Argument is null</exception>
        /// <exception cref="System.NotSupportedException">The transport does not require refreshing</exception>
        /// <exception cref="It.Unina.Dis.Logbus.OutTransports.TransportException">Unable to unsubscribe client</exception>
        /// <exception cref="It.Unina.Dis.Logbus.OutTransports.ClientNotSubscribedException">Client is not subscribed</exception>
        void RefreshClient(string clientId);

        /// <summary>
        /// Unsubscribes a client from the transport
        /// </summary>
        /// <param name="clientId">ID of client to unsubscribe</param>
        /// <exception cref="It.Unina.Dis.Logbus.OutTransports.TransportException">Unable to unsubscribe client</exception>
        /// <exception cref="It.Unina.Dis.Logbus.OutTransports.ClientNotSubscribedException">Client is not subscribed</exception>
        void UnsubscribeClient(string clientId);
    }
}