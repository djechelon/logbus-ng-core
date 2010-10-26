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
namespace It.Unina.Dis.Logbus.FFDA
{
    /// <summary>
    /// Supports the event of receiving an FFDA message
    /// </summary>
    public sealed class FFDAEventArgs
        : SyslogMessageEventArgs
    {

        /// <summary>
        /// Type of FFDA event
        /// </summary>
        public FFDAEvent EventType;

        /// <summary>
        /// ID of execution flow, if specified
        /// </summary>
        public string FlowId;

        /// <summary>
        /// Host-name generating FFDA message
        /// </summary>
        public string Host;

        /// <summary>
        /// Process ID (or application name) generating FFDA message
        /// </summary>
        public string Process;

        /// <summary>
        /// Name of logger that generated FFDA message
        /// </summary>
        public string LoggerName;

    }
}
