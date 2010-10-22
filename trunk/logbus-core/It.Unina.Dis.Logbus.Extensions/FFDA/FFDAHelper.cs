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

using It.Unina.Dis.Logbus.Collectors;
using System.Net;
namespace It.Unina.Dis.Logbus.FFDA
{
    /// <summary>
    /// Creates FFDA loggers
    /// </summary>
    public sealed class FFDAHelper
    {

        /// <summary>
        /// Creates an FFDA logger by logger name. If logger is maked as static and has been already instanced, the method returns the current
        /// instance of the logger
        /// </summary>
        /// <param name="loggerName">Name of logger</param>
        /// <returns></returns>
        /// <exception cref="LogbusException">Logger is not found</exception>
        /// <exception cref="System.InvalidOperationException">Configuration is not set or is invalid</exception>
        public static IFFDALogger CreateFFDALogger(string loggerName)
        {
            ILogCollector collector;
            try
            {
                collector = CollectorHelper.CreateCollectorForLogger(loggerName);
            }
            catch
            {
                collector = CollectorHelper.CreateCollector();
            }
            return new FFDALogger(collector) { LogName = loggerName };
        }

        /// <summary>
        /// Creates the FFDA logger for monitors
        /// </summary>
        /// <returns></returns>
        public static IFFDAMonitorLogger CreateFFDAMonitorLogger()
        {
            return new FFDAMonitorLogger(CollectorHelper.CreateCollector());
        }

        /// <summary>
        /// Creates an instrumentation logger. It can perform both regular logging and FFD logging
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        public static IInstrumentedLogger CreateInstrumentedLogger(string loggerName)
        {
            ILogCollector collector;
            try
            {
                collector = CollectorHelper.CreateCollectorForLogger(loggerName);
            }
            catch
            {
                collector = CollectorHelper.CreateCollector();
            }
            return new FFDALogger(collector) { LogName = loggerName };
        }

        /// <summary>
        /// Creates an unreliable FFD logger
        /// </summary>
        /// <param name="loggerName">Name of logger</param>
        /// <param name="host">Destination host</param>
        /// <param name="port">Destination port</param>
        /// <returns>An FFD logger that works on unreliable transport</returns>
        public static IFFDALogger CreateUnreliableFFDALogger(string loggerName, string host, int port)
        {
            return new FFDALogger(CollectorHelper.CreateUnreliableCollector(host, port)) { LogName = loggerName };
        }

        /// <summary>
        /// Creates an unreliable FFD logger
        /// </summary>
        /// <param name="loggerName">Name of logger</param>
        /// <param name="host">Destination host</param>
        /// <param name="port">Destination port</param>
        /// <returns>An FFD logger that works on unreliable transport</returns>
        public static IFFDALogger CreateUnreliableFFDALogger(string loggerName, IPAddress host, int port)
        {
            return new FFDALogger(CollectorHelper.CreateUnreliableCollector(host, port)) { LogName = loggerName };
        }

        /// <summary>
        /// Creates a reliable FFD logger
        /// </summary>
        /// <param name="loggerName">Name of logger</param>
        /// <param name="host">Destination host</param>
        /// <param name="port">Destination port</param>
        /// <returns>An FFD logger that works on reliable transport</returns>
        /// <remarks>Log messages are not subject to loss, however this may affect performance</remarks>
        public static IFFDALogger CreateReliableFFDALogger(string loggerName, string host, int port)
        {
            return new FFDALogger(CollectorHelper.CreateUnreliableCollector(host, port)) { LogName = loggerName };
        }
    }
}
