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
using It.Unina.Dis.Logbus.Collectors;
using It.Unina.Dis.Logbus.Loggers;
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
            return new FFDALogger(CollectorHelper.CreateCollectorByName(loggerName));
        }

        public  static IFFDAMonitorLogger CreateFFDAMonitorLogger()
        {
            return new FFDAMonitorLogger(CollectorHelper.CreateDefaultCollector());
        }

        public static IInstrumentedLogger CreateInstrumentedLogger(string loggerName)
        {
            return new FFDALogger(CollectorHelper.CreateCollectorByName(loggerName));
        }
    }
}
