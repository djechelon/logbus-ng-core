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
using System.Net;
namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Advanced attributes for Syslog messages, both from RFC 5424 and Logbus-ng standard fields
    /// </summary>
    public struct SyslogAttributes
    {
        /// <summary>
        /// Class that originated the message
        /// </summary>
        /// <remarks>This attribute is Logbus-ng specific for loggers</remarks>
        public string ClassName;

        /// <summary>
        /// Module that originated the message
        /// </summary>
        /// <remarks>This attribute is Logbus-ng specific for loggers. For C#, it's the assembly, for Java, it's the package</remarks>
        public string ModuleName;

        /// <summary>
        /// Method that originated the message
        /// </summary>
        /// <remarks>This attribute is Logbus-ng specific for loggers</remarks>
        public string MethodName;

        /// <summary>
        /// Log that originated the message
        /// </summary>
        /// <remarks>This attribute is Logbus-ng specific for loggers</remarks>
        public string LogName;

        /// <summary>
        /// Whether the origin knows its time zone
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public bool TimeZoneKnown;

        /// <summary>
        /// Whether the origin knows that its local time is synchronized with a standard source
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public bool TimeSynchronized;

        /// <summary>
        /// Synchronization accuracy for the origin's local time
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public long TimeSyncAccuracy;

        /// <summary>
        /// IP address of originator
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public IPAddress OriginIpAddress;

        /// <summary>
        /// SMI Network Management Private Enterprise Code for the message
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public string EnterpriseId;

        /// <summary>
        /// Software originating the message
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public string OriginatorSoftware;

        /// <summary>
        /// Version of software that originated the message
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public string OriginatorSoftwareVersion;

        /// <summary>
        /// Sequence ID of the message
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public Int32 SequenceId;

        /// <summary>
        /// Uptime of the machine that generated the message
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public long SystemUptime;

        /// <summary>
        /// Language for the text part of the log message
        /// </summary>
        /// <remarks>Standard attribute defined in RFC 5424</remarks>
        public string MessageLanguage;
    }
}
