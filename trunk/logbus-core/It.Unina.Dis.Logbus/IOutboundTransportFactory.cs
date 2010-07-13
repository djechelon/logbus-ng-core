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


namespace It.Unina.Dis.Logbus
{
    
    /// <summary>
    /// Creates transports according to configuration.
    /// Each outbound transport has its own factory that is aware of transport-specific configuration.
    /// Transports can <b>ONLY</b> be instantiated by a factory.
    /// There are two kinds of parameters: configuration parameters and client parameters.
    /// Configuration parameters are for internal
    /// <example>Example: UDP transport has its own UDPTransportFactory that has no configuration parameter.
    /// Clients provide their own parameters with IP address and port number for destination.
    /// Instead, UDP Multicast transport must know which Multicast groups are available. Factory will then
    /// instantiate a UDP Multicast group using one of the available group address. Clients subscribing to a
    /// Multicast group don't submit parameters but get runtime instructions by the transport itself
    /// </example>
    /// </summary>
    public interface IOutboundTransportFactory
        :IConfigurable
    {
        /// <summary>
        /// Instantiates an outbound transport to which clients must be subscribe when they subscribe to an outbound channel
        /// </summary>
        /// <returns></returns>
        IOutboundTransport CreateTransport();
    }
}
