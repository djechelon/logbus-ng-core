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
using System.Text;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Logbus inbound channel. Collects log messages and notifies Logbus core of new messages
    /// </summary>
    public interface IInboundChannel
        : ILogSource, IDisposable
    {

        /// <summary>
        /// Descriptive name of Inbound channel
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Starts collecting Syslog messages
        /// </summary>
        void Start();

        /// <summary>
        /// Stops collecting Syslog messages
        /// </summary>
        void Stop();

        /// <summary>
        /// Notifies a parse error occurred
        /// </summary>
        event ParseErrorEventHandler ParseError;

        /// <summary>
        /// Configures the channel according to the design contract
        /// </summary>
        IDictionary<string, string> Configuration { get; }

    }
}