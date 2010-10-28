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

namespace It.Unina.Dis.Logbus.OutChannels
{
    /// <summary>
    /// Base class for client subscription's related events
    /// </summary>
    public abstract class ClientSubscriptionEventArgs
        : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of ClientSubscriptionEventArgs
        /// </summary>
        /// <param name="channel">Channel to which the client is subscribing</param>
        /// <param name="transport">Transport chosen by client</param>
        /// <param name="instructions">Instructions provided by client to the transport</param>
        protected ClientSubscriptionEventArgs(IOutboundChannel channel, string transport,
                                              IEnumerable<KeyValuePair<string, string>> instructions)
        {
            Channel = channel;
            ChosenTransport = transport;
            TransportConfiguration = instructions;
        }

        /// <summary>
        /// Channel the client is subscribing to
        /// </summary>
        public IOutboundChannel Channel { get; protected set; }

        /// <summary>
        /// Transport chosen by client
        /// </summary>
        public string ChosenTransport { get; protected set; }

        /// <summary>
        /// Instructions given to transport
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> TransportConfiguration { get; protected set; }
    }
}