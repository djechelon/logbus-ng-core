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

using System.Collections.Generic;
namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Simplified interface for controlling a Logbus object
    /// </summary>
    public interface ILogbusController
    {
        #region Channel Management
        IOutboundChannel[] AvailableChannels { get; }

        void CreateChannel(string id, string name, Filters.IFilter filter, string description, long coalescenceWindow);

        void RemoveChannel(string id);

        #endregion

        #region Channel Subscription

        string[] AvailableTransports { get; }

        string SubscribeClient(string channelId, string transportId, IEnumerable<KeyValuePair<string, string>> transportInstructions, out IEnumerable<KeyValuePair<string, string>> clientInstructions);

        /// <exception cref="InvalidOperationException">Client is not subscribed (or already expired)</exception>
        void RefreshClient(string clientId);

        void UnsubscribeClient(string clientId);
        #endregion
    }
}
