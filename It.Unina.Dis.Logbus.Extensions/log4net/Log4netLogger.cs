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
using System;
using log4net;
using It.Unina.Dis.Logbus.RemoteLogbus;
using System.Collections.Generic;
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

        private global::log4net.Core.ILogger logger;

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            throw new NotImplementedException();

            LoggingEventData ld = new LoggingEventData();
            ld.TimeStamp = message.Timestamp.Value;

            LoggingEvent le = new LoggingEvent(ld);

            logger.Log(le);
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

        System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> IConfigurable.Configuration
        {
            set
            {
                foreach (KeyValuePair<string,string> kvp in value)
                {
                    ((IConfigurable)this).SetConfigurationParameter(kvp.Key, kvp.Value);
                }
            }
        }

        #endregion
    }
}
