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

namespace It.Unina.Dis.Logbus.Loggers
{
    /// <summary>
    /// Implement this interface if you need to re-define a logger.
    /// User code will never have visibility over this interface, but only over ILog
    /// </summary>
    public interface ILogger
        : ILog, ICloneable
    {
        /// <summary>
        /// Gets or sets the facility for the logger
        /// </summary>
        SyslogFacility Facility { get; set; }

        /// <summary>
        /// Gets or sets the heartbeat interval, in seconds
        /// </summary>
        /// <remarks>Heartbeating is by default done using special Syslog messages marked with ID "HEARTBEAT"</remarks>
        int HeartbeatInterval { get; set; }
    }
}