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

//Credit http://www.codeproject.com/KB/IP/Syslogd.aspx

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Syslog severity. The standard defines this value as a priority value
    /// </summary>
    public enum SyslogSeverity : byte
    {
        /// <summary>
        /// Emergency: system is unusable
        /// </summary>
        Emergency = 0,

        /// <summary>
        /// Alert: action must be taken immediately
        /// </summary>
        Alert = 1,

        /// <summary>
        /// Critical: critical conditions
        /// </summary>
        Critical = 2,

        /// <summary>
        /// Error: error conditions
        /// </summary>
        Error = 3,

        /// <summary>
        /// Warning: noticeable events
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Notice: normal but significant condition
        /// </summary>
        Notice = 5,

        /// <summary>
        /// Informational: low-priority informational messages
        /// </summary>
        Info = 6,

        /// <summary>
        /// Debug: debug-level messages
        /// </summary>
        Debug = 7
    }
}