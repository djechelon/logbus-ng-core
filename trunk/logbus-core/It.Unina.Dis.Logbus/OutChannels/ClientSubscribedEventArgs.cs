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

using System.Collections.Generic;

namespace It.Unina.Dis.Logbus.OutChannels
{
    /// <summary>
    /// Event args for client that has subscribed to a channel.
    /// <remarks>Subcription is complete and cannot be canceled</remarks>
    /// </summary>
    public class ClientSubscribedEventArgs
        : ClientSubscriptionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of ClientSubscribedEventArgs
        /// </summary>
        /// <param name="channel">Channel to which the client is subscribing</param>
        /// <param name="transport">Transport chosen by client</param>
        /// <param name="instructions">Instructions provided by client to the transport</param>
        /// <param name="clientId">Client ID assigned by Logbus-ng</param>
        /// <param name="outInstructions">Instructions provided by transport to client</param>
        public ClientSubscribedEventArgs(IOutboundChannel channel, string transport,
                                         IEnumerable<KeyValuePair<string, string>> instructions, string clientId,
                                         IDictionary<string, string> outInstructions)
            : base(channel, transport, instructions)
        {
            ClientId = clientId;
            ClientInstructions = outInstructions;
        }

        /// <summary>
        /// Id of client assigned by Logbus-ng
        /// </summary>
        public string ClientId { get; protected set; }

        /// <summary>
        /// Instructions given by transport to client
        /// </summary>
        public IDictionary<string, string> ClientInstructions { get; protected set; }
    }
}