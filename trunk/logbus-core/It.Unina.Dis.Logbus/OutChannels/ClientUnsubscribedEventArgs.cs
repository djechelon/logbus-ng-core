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

namespace It.Unina.Dis.Logbus.OutChannels
{
    /// <summary>
    /// Event args for client unsubscription
    /// </summary>
    public class ClientUnsubscribedEventArgs
        : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of ClientUnsubscribedEventArgs
        /// </summary>
        /// <param name="channel">Channel to which the client was subscribed</param>
        /// <param name="clientId">ID of client</param>
        public ClientUnsubscribedEventArgs(IOutboundChannel channel, string clientId)
        {
            Channel = channel;
            ClientId = clientId;
        }

        /// <summary>
        /// Channel the client was subscribed to
        /// </summary>
        public IOutboundChannel Channel { get; protected set; }

        /// <summary>
        /// Id of client assigned by Logbus-ng
        /// </summary>
        public string ClientId { get; protected set; }
    }
}