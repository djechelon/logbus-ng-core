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

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Helper interface for transport creation.
    /// This class holds the factories for all kinds of transports
    /// </summary>
    public interface ITransportFactoryHelper
    {
        /// <summary>
        /// Simple indexer property that does both Get and Add/Remove
        /// </summary>
        /// <param name="transportId">Transport type's ID</param>
        /// <returns>Factory for given transport type</returns>
        /// <exception cref="System.NotSupportedException">Thrown when no transport factory is bound to the given ID</exception>
        /// <exception cref="System.ArgumentNullException">transportId is null or empty</exception>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        IOutboundTransportFactory this[string transportId] { get; set; }

        /// <summary>
        /// Returns the factory currently associated to the given transport type
        /// </summary>
        /// <param name="transportId">ID for transport type</param>
        /// <returns>Factory for given transport type</returns>
        /// <exception cref="System.NotSupportedException">Thrown when no transport factory is bound to the given ID</exception>
        /// <exception cref="System.ArgumentNullException">transportId is null or empty</exception>
        IOutboundTransportFactory GetFactory(string transportId);

        /// <summary>
        /// Adds a factory for a new transport ID
        /// </summary>
        /// <param name="transportId">New transport ID</param>
        /// <param name="factory">Instance of IOutboundTransportFactory to use as factory</param>
        /// <exception cref="System.ArgumentNullException">Thrown if either transportId or factory is null</exception>
        /// <exception cref="System.InvalidOperationException">A factory already exists for the given transport ID</exception>
        void AddFactory(string transportId, IOutboundTransportFactory factory);

        /// <summary>
        /// Removes the factory associated to the given transport type
        /// </summary>
        /// <param name="transportId">Transport type's ID</param>
        /// <exception cref="System.InvalidOperationException">No factory was associated to the given ID</exception>
        /// <exception cref="System.ArgumentNullException">transportId is null or empty</exception>
        void RemoveFactory(string transportId);

        /// <summary>
        /// Lists currently available transports
        /// </summary>
        string[] AvailableTransports { get; }
    }
}
