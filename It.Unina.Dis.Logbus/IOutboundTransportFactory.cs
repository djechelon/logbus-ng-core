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

//For now let's try with System.Object. Maybe one day we'll switch to String
using OUT_TRANSPORT_CONFIG_TYPE = System.Object;

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
    {
        /// <summary>
        /// Instantiates an outbound transport to which clients must be subscribe when they subscribe to an outbound channel
        /// </summary>
        /// <returns></returns>
        IOutboundTransport CreateTransport();

        /// <summary>
        /// Gets a configuration parameter by name
        /// </summary>
        /// <param name="key">Name of configuration parameter</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Key is null</exception>
        /// <exception cref="NotSupportedException">Key is not supported by this transport factory</exception>
        OUT_TRANSPORT_CONFIG_TYPE GetConfigurationParameter(string key);
        
        /// <summary>
        /// Sets a configuration parameter by key
        /// </summary>
        /// <param name="key">Name of configuration parameter</param>
        /// <param name="value">Value to set. Can be null</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Key is null</exception>
        /// <exception cref="ArgumentException">The specific transport doesn't accept such value</exception>
        /// <exception cref="NotSupportedException">Key is not supported by this transport factory</exception>
        void SetConfigurationParameter(string key, OUT_TRANSPORT_CONFIG_TYPE value);

        /// <summary>
        /// Gets or sets all configuration parameters in one shot
        /// </summary>
        IEnumerable<KeyValuePair<string, OUT_TRANSPORT_CONFIG_TYPE>> Configuration { get; set; }
    }
}
