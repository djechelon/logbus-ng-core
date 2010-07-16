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

using It.Unina.Dis.Logbus.Loggers;
using System.Net;
namespace It.Unina.Dis.Logbus.FFDA
{
    public sealed class FFDAHelper
    {
        /// <summary>
        /// Creates an FFDA logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <returns>An FFDALogger, to which clients could sumbit FFDA Messages</returns>
        /// <remarks>Facility is set to Local0 as default value</remarks>
        public static FFDALogger CreateFFDALogger(IPAddress logbus_ip, int logbus_port)
        {
            return new FFDALogger(LoggerHelper.CreateUdpCollector(logbus_ip, logbus_port));
        }

        /// <summary>
        /// Creates an FFDA logger with the default logger
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Configuration is not set or is invalid</exception>
        public static FFDALogger CreateFFDALogger()
        {
            return new FFDALogger(LoggerHelper.CreateDefaultCollector());
        }
    }
}
