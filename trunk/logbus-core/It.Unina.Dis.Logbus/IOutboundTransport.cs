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

using CLIENT_ID_TYPE = System.String;
using CLIENT_INSTRUCTIONS_TYPE = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>>;
using TRANSPORT_INSTRUCTIONS_TYPE = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>>;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Outbound transport. Implements the delivery logic for remote clients
    /// </summary>
    public interface IOutboundTransport
        :IDisposable
    {

        /// <summary>
        /// Sends a Syslog message to subscribed clients
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <remarks>Messages have been already filtered</remarks>
        void SubmitMessage(SyslogMessage message);

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
        CLIENT_ID_TYPE SubscribeClient(TRANSPORT_INSTRUCTIONS_TYPE inputInstructions, out CLIENT_INSTRUCTIONS_TYPE outputInstructions);

        /// <summary>
        /// Gets if the current transport requires client refresh
        /// </summary>
        bool RequiresRefresh { get; }

        /// <summary>
        /// Gets the subscription's time to live, in seconds
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when RequiresRefresh is <i>false</i></exception>
        long SubscriptionTtl { get; }

        /// <summary>
        /// Refreshes subscription, if required, to the specified client
        /// </summary>
        /// <param name="clientId">ID of the client to refresh</param>
        /// <exception cref="ArgumentNullException">Argument is null</exception>
        /// <exception cref="NotSupportedException">The transport does not require refreshing</exception>
        /// <exception cref="InvalidOperationException">Client is not subscribed (or already expired)</exception>
        void RefreshClient(CLIENT_ID_TYPE clientId);
        
        /// <summary>
        /// Unsubscribes a client from the transport
        /// </summary>
        /// <param name="clientId"></param>
        void UnsubscribeClient(CLIENT_ID_TYPE clientId);
    }
}
