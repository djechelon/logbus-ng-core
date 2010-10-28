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
    /// Event args for client before subscription.
    /// Can be used to cancel subscription and lead to UnauthorizedException
    /// </summary>
    public class ClientSubscribingEventArgs
        : ClientSubscriptionEventArgs
    {
        private List<string> _reasons;

        /// <summary>
        /// Initializes a new instanceClientSubscribingEventArgs
        /// </summary>
        /// <param name="channel">Channel to which the client is subscribing</param>
        /// <param name="transport">Transport chosen by client</param>
        /// <param name="instructions">Instructions provided by client to the transport</param>
        public ClientSubscribingEventArgs(IOutboundChannel channel, string transport,
                                          IEnumerable<KeyValuePair<string, string>> instructions)
            : base(channel, transport, instructions)
        {
        }

        /// <summary>
        /// Whether the subscription will be canceled or not
        /// </summary>
        public bool Cancel
        {
            get { return _reasons == null; }
        }

        /// <summary>
        /// List of reasons provided by handlers to cancel the subscription
        /// </summary>
        public string[] ReasonForCanceling
        {
            get { return (_reasons == null) ? null : _reasons.ToArray(); }
        }

        /// <summary>
        /// Requests to cancel the subscription of the client by specifying a textual motivation
        /// </summary>
        /// <param name="reason">Reason beacause the subscription will be canceled</param>
        /// <remarks>If at least one handler requires canceling, subscription will be canceled</remarks>
        public void CancelByReason(string reason)
        {
            if (_reasons == null) _reasons = new List<string>();

            _reasons.Add(reason);
        }
    }
}