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
    /// Used by the OutChannelCreated event
    /// </summary>
    public sealed class OutChannelCreationEventArgs
        : EventArgs
    {
        /// <remarks/>
        public OutChannelCreationEventArgs(IOutboundChannel channel)
        {
            if (channel == null) throw new ArgumentNullException("channel");
            Channel = channel;
        }

        /// <summary>
        /// Channel just created
        /// </summary>
        public IOutboundChannel Channel { get; private set; }
    }
}