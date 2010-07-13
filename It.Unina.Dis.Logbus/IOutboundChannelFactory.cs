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

using It.Unina.Dis.Logbus.Filters;
namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Factory for Outbound Channels. Can be overriden in Logbus core
    /// </summary>
    public interface IOutboundChannelFactory
    {
        /// <summary>
        /// Creates a new channel instance
        /// </summary>
        /// <param name="name">Channel name</param>
        /// <param name="description">Channel description</param>
        /// <param name="filter">Channel filter</param>
        /// <returns>A new instance of a class that implements IOutboundChannel</returns>
        /// <exception cref="InvalidOperationException">Factory helper is not set</exception>
        IOutboundChannel CreateChannel(string name, string description, IFilter filter);

        /// <summary>
        /// Sets the Factory helper, needed for channel creation
        /// </summary>
        ITransportFactoryHelper TransportFactoryHelper { set; }

    }
}
