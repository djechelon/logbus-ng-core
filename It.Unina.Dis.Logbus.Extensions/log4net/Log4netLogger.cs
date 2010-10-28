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
using System.Threading;
using log4net;
using log4net.Core;

namespace It.Unina.Dis.Logbus.log4net
{
    /// <summary>
    /// Logger that forwards messages to an existing Log4net logger
    /// </summary>
    /// <remarks>
    /// Configuration parameters:
    /// <list>
    /// <item><code>logger</code><description>Name of existing Log4net logger</description></item>
    /// </list>
    /// </remarks>
    internal class Log4netLogger
        : ILogCollector, IConfigurable
    {
        #region ILogCollector Membri di

        private ILogger logger;

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            if (logger == null) logger = LogManager.GetLogger(GetType()).Logger;

            LoggingEventData ld = new LoggingEventData();
            if (message.Timestamp != null) ld.TimeStamp = message.Timestamp.Value;
            ld.Message = message.Text;
            ld.ThreadName = Thread.CurrentThread.Name;

            switch (message.Severity)
            {
                case SyslogSeverity.Debug:
                    {
                        ld.Level = Level.Debug;
                        break;
                    }
                case SyslogSeverity.Info:
                    {
                        ld.Level = Level.Info;
                        break;
                    }
                case SyslogSeverity.Notice:
                    {
                        ld.Level = Level.Notice;
                        break;
                    }
                case SyslogSeverity.Warning:
                    {
                        ld.Level = Level.Warn;
                        break;
                    }
                case SyslogSeverity.Error:
                    {
                        ld.Level = Level.Error;
                        break;
                    }
                case SyslogSeverity.Alert:
                    {
                        ld.Level = Level.Alert;
                        break;
                    }
                case SyslogSeverity.Critical:
                    {
                        ld.Level = Level.Critical;
                        break;
                    }
                case SyslogSeverity.Emergency:
                    {
                        ld.Level = Level.Emergency;
                        break;
                    }
            }

            LoggingEvent le = new LoggingEvent(ld);

            try
            {
                logger.Log(le);
            }
            catch (LogException ex)
            {
                throw new LogbusException("Unable to log", ex);
            }
        }

        #endregion

        #region IConfigurable Membri di

        string IConfigurable.GetConfigurationParameter(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            switch (key)
            {
                case "logger":
                    {
                        return logger.Name;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter not supported");
                    }
            }
        }

        void IConfigurable.SetConfigurationParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            switch (key)
            {
                case "logger":
                    {
                        if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                        logger = LogManager.GetLogger(value).Logger;
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter not supported");
                    }
            }
        }

        IEnumerable<KeyValuePair<string, string>> IConfigurable.Configuration
        {
            set
            {
                foreach (KeyValuePair<string, string> kvp in value)
                {
                    ((IConfigurable) this).SetConfigurationParameter(kvp.Key, kvp.Value);
                }
            }
        }

        #endregion
    }
}