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

namespace It.Unina.Dis.Logbus.Design
{
    /// <summary>
    /// Defines an outbound transport factory's attributes.
    /// Main attribute is the tag. There cannot exist two classes with the same tag.
    /// The attribute MUST be applied to a subclass of <see cref="IOutboundTransportFactory"/> and NOT to
    /// a subclass of <see cref="IOutboundTransport"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class TransportFactoryAttribute : Attribute
    {
        /// <summary>
        /// Initializes TransportFactoryAttribute with the required tag
        /// </summary>
        /// <param name="tag"></param>
        public TransportFactoryAttribute(string tag)
        {
            Tag = tag;
        }

        /// <summary>
        /// Identifying tag for outbound transport
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// Name of transport created by this factory
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Human-readable description
        /// </summary>
        public string Description { get; set; }
    }
}