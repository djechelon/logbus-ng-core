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
    /// Special logger for logging events internally to Logbus-ng service
    /// </summary>
    /// <remarks>
    /// This logger can be used <b>only</b> from within applications that
    /// run a Logbus service, and it must not be confused with any other
    /// logger that logs to a remote Logbus-ng service
    /// </remarks>
    internal class LogbusLogger
        : ILogCollector
    {
        #region Constructor
        /// <summary>
        /// Initializes LogbusLogger with a given instance of a Logbus service
        /// or one of its wrappers
        /// </summary>
        /// <param name="targetLogbus">Logbus service that will collect messages</param>
        public LogbusLogger(ILogBus targetLogbus)
        {
            if (targetLogbus == null) throw new ArgumentException("targetLogbus");
            target = targetLogbus;
        }

        /// <summary>
        /// Initializes LogbusLogger with the default singleton instance of Logbus service
        /// </summary>
        public LogbusLogger()
            : this(LogbusSingletonHelper.Instance) { }
        #endregion
        
        private ILogBus target;

        #region ILogCollector Membri di

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            target.SubmitMessage(message);
        }

        #endregion
    }
}
