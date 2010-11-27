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
    /// Used by OutChannelDeleted event
    /// </summary>
    public sealed class OutChannelDeletionEventArgs
        : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of OutChannelDeletionEventArgs
        /// </summary>
        /// <param name="channelId">ID of channel just deleted</param>
        public OutChannelDeletionEventArgs(string channelId)
        {
            if (string.IsNullOrEmpty(channelId)) throw new ArgumentNullException("channelId");
            ChannelId = channelId;
        }

        /// <summary>
        /// ID of channel just deleted
        /// </summary>
        public string ChannelId { get; private set; }
    }
}