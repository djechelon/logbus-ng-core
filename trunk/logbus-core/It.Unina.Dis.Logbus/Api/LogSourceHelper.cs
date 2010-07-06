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

using System.Net;
using It.Unina.Dis.Logbus.Utils;
namespace It.Unina.Dis.Logbus.Api
{
    /// <summary>
    /// This class provides services for Log sources (clients which generate log messages and want to send them to Logbus)
    /// </summary>
    public sealed class LogSourceHelper
    {

        private LogSourceHelper() { }

        /// <summary>
        /// Creates a Log collector which uses Syslog UDP sending
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <returns>A new instance of ILogCollector to submit SyslogMessages</returns>
        public static ILogCollector CreateUdpEntryPoint(IPAddress logbus_ip, int logbus_port)
        {
            return new Utils.SyslogUdpSender(logbus_ip, logbus_port);
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog CreateUdpLogger(IPAddress logbus_ip, int logbus_port)
        {
            return new LogImpl(CreateUdpEntryPoint(logbus_ip, logbus_port));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog CreateUdpLogger(IPAddress logbus_ip, int logbus_port, SyslogFacility facility)
        {
            return new LogImpl(facility, CreateUdpEntryPoint(logbus_ip, logbus_port));
        }
    }
}
