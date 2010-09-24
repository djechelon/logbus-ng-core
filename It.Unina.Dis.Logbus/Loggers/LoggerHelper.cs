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
using It.Unina.Dis.Logbus.Collectors;
using It.Unina.Dis.Logbus.Configuration;
using System;
using It.Unina.Dis.Logbus.InChannels;
using System.Collections.Generic;

namespace It.Unina.Dis.Logbus.Loggers
{
    /// <summary>
    /// This class provides services for Log sources (clients which generate log messages and want to send them to Logbus)
    /// </summary>
    public static class LoggerHelper
    {
        static LoggerHelper()
        {
            _loggers = new Dictionary<string, ILogger>();
            try
            {
                Configuration = ConfigurationHelper.SourceConfiguration;
            }
            catch (LogbusConfigurationException) { }
        }

        private static Dictionary<string, ILogger> _loggers;

        /// <summary>
        /// Gets or sets global source configuration
        /// </summary>
        public static LogbusLoggerConfiguration Configuration
        {
            get;
            set;
        }

        private static LoggerDefinition GetDefinition(string loggerName)
        {
            if (Configuration == null) throw new InvalidOperationException("No configuration specified");
            if (Configuration.logger == null) throw new InvalidOperationException("No logger specified in configuration");

            foreach (LoggerDefinition def in Configuration.logger)
            {
                if (def != null && def.name == loggerName) return def;
            }

            throw new LogbusException(string.Format("Logger {0} not found in configuration", loggerName));
        }

        /// <summary>
        /// Creates default logger by reflection
        /// </summary>
        /// <returns></returns>
        private static ILogger InstantiateLogger()
        {
            try
            {
                Type loggerType = typeof(SimpleLogImpl);
                if (Configuration != null)
                {
                    if (!string.IsNullOrEmpty(Configuration.defaultloggertype))
                    {
                        string typename = Configuration.defaultloggertype;

                        if (typename.IndexOf('.') < 0)
                        {
                            //This is probably a plain class name, overriding to It.Unina.Dis.Logbus.Loggers namespace
                            const string namespc = "It.Unina.Dis.Logbus.Loggers";
                            string assemblyname = typeof(LoggerHelper).Assembly.GetName().ToString();
                            typename = string.Format("{0}.{1}, {2}", namespc, typename, assemblyname);
                        }
                        loggerType = Type.GetType(typename);
                        if (!typeof(ILogger).IsAssignableFrom(loggerType))
                        {
                            LogbusConfigurationException ex =
                                new LogbusConfigurationException(
                                    "Registered logger type does not implement ILog");
                            ex.Data.Add("type", loggerType);
                            throw ex;
                        }
                    }
                }
                ILogger ret = Activator.CreateInstance(loggerType) as ILogger;
                return ret;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to create logger because of configuration error", ex);
            }
        }

        /// <summary>
        /// Creates logger by reflection for configured logger
        /// </summary>
        /// <returns></returns>
        private static ILogger InstantiateLogger(string loggerName)
        {
            if (string.IsNullOrEmpty(loggerName)) throw new ArgumentNullException("loggerName");
            if (_loggers.ContainsKey(loggerName)) return _loggers[loggerName];

            bool permanent = false;
            try
            {
                Type loggerType = typeof(SimpleLogImpl);
                if (Configuration != null)
                {
                    string typename = Configuration.defaultloggertype;

                    try
                    {
                        LoggerDefinition def = GetDefinition(loggerName);
                        if (!string.IsNullOrEmpty(def.type)) typename = def.type;
                        permanent = def.permanent;
                    }
                    catch (Exception) { }

                    if (!string.IsNullOrEmpty(typename))
                    {
                        if (typename.IndexOf('.') < 0)
                        {
                            //This is probably a plain class name, overriding to It.Unina.Dis.Logbus.Loggers namespace
                            const string namespc = "It.Unina.Dis.Logbus.Loggers";
                            string assemblyname = typeof(LoggerHelper).Assembly.GetName().ToString();
                            typename = string.Format("{0}.{1}, {2}", namespc, typename, assemblyname);
                        }
                        loggerType = Type.GetType(typename);
                        if (!typeof(ILogger).IsAssignableFrom(loggerType))
                        {
                            LogbusConfigurationException ex =
                                new LogbusConfigurationException(
                                    "Registered logger type does not implement ILogger");
                            ex.Data.Add("type", loggerType);
                            throw ex;
                        }
                    }
                }
                ILogger ret = Activator.CreateInstance(loggerType) as ILogger;
                ret.LogName = loggerName;
                if (permanent) _loggers.Add(loggerName, ret);
                return ret;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to create logger because of configuration error", ex);
            }
        }

        /// <summary>
        /// Creates a logger using default configuration
        /// </summary>
        /// <returns></returns>
        public static ILog GetLogger()
        {
            return GetLogger(CollectorHelper.CreateCollector());
        }

        /// <summary>
        /// Creates a logger with given collector
        /// </summary>
        /// <param name="collector">Ultimate destination of log messages</param>
        /// <returns></returns>
        public static ILog GetLogger(ILogCollector collector)
        {
            ILog ret = InstantiateLogger();
            ret.Collector = collector;
            return ret;
        }

        /// <summary>
        /// Creates a logger with given facility and default collector
        /// </summary>
        /// <param name="facility">Syslog facility to use</param>
        /// <returns></returns>
        public static ILog GetLogger(SyslogFacility facility)
        {
            ILogger ret = InstantiateLogger();
            ret.Collector = CollectorHelper.CreateCollector();
            ret.Facility = facility;
            return ret;
        }

        /// <summary>
        /// Creates a logger with given facility and collector
        /// </summary>
        /// <param name="facility">Syslog facility to use</param>
        /// <param name="collector">Ultimate destination of log messages</param>
        /// <returns></returns>
        public static ILog GetLogger(SyslogFacility facility, ILogCollector collector)
        {
            ILogger ret = InstantiateLogger();
            ret.Collector = collector;
            ret.Facility = facility;
            return ret;
        }

        /// <summary>
        /// Creates a logger by
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        public static ILog GetLogger(string loggerName)
        {
            ILog ret = InstantiateLogger(loggerName);
            //WRONG!!! Loggers can be configured with a specific collector
            ret.Collector = CollectorHelper.CreateCollector();
            return ret;
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbusIp">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        [Obsolete("You should use GetUnreliableLogger instead")]
        public static ILog CreateUdpLogger(IPAddress logbusIp, int logbusPort)
        {
            return GetLogger(CollectorHelper.CreateUnreliableCollector(logbusIp, logbusPort));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbusIp">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog GetUnreliableLogger(IPAddress logbusIp, int logbusPort)
        {
            return GetLogger(CollectorHelper.CreateUnreliableCollector(logbusIp, logbusPort));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP, using default port
        /// </summary>
        /// <param name="logbusIp">IP address of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog GetUnreliableLogger(IPAddress logbusIp)
        {
            return GetLogger(CollectorHelper.CreateUnreliableCollector(logbusIp, SyslogUdpReceiver.DEFAULT_PORT));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP, using default port
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog GetUnreliableLogger(string logbusHost)
        {
            return GetLogger(CollectorHelper.CreateUnreliableCollector(logbusHost, SyslogUdpReceiver.DEFAULT_PORT));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via TLS
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <param name="logbusPort">TLS port of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog GetReliableLogger(string logbusHost, int logbusPort)
        {
            return GetLogger(CollectorHelper.CreateReliableCollector(logbusHost, logbusPort));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via TLS, using default port
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog GetReliableLogger(string logbusHost)
        {
            return GetLogger(CollectorHelper.CreateReliableCollector(logbusHost, SyslogTlsReceiver.TLS_PORT));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbusIp">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        [Obsolete("You should use GetUnreliableLogger instead")]
        public static ILog CreateUdpLogger(IPAddress logbusIp, int logbusPort, SyslogFacility facility)
        {
            return GetUnreliableLogger(logbusIp, logbusPort, facility);
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbusIp">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog GetUnreliableLogger(IPAddress logbusIp, int logbusPort, SyslogFacility facility)
        {
            return GetLogger(facility, CollectorHelper.CreateUnreliableCollector(logbusIp, logbusPort));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog GetUnreliableLogger(string logbusHost, int logbusPort, SyslogFacility facility)
        {
            return GetLogger(facility, CollectorHelper.CreateUnreliableCollector(logbusHost, logbusPort));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP, using default port
        /// </summary>
        /// <param name="logbusIp">IP address of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog GetUnreliableLogger(IPAddress logbusIp, SyslogFacility facility)
        {
            return GetLogger(facility, CollectorHelper.CreateUnreliableCollector(logbusIp, SyslogUdpReceiver.DEFAULT_PORT));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP, using default port
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog GetUnreliableLogger(string logbusHost, SyslogFacility facility)
        {
            return GetLogger(facility, CollectorHelper.CreateUnreliableCollector(logbusHost, SyslogUdpReceiver.DEFAULT_PORT));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via TLS
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog GetReliableLogger(string logbusHost, int logbusPort, SyslogFacility facility)
        {
            return GetLogger(facility, CollectorHelper.CreateReliableCollector(logbusHost, logbusPort));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via TLS, using default port
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog GetReliableLogger(string logbusHost, SyslogFacility facility)
        {
            return GetLogger(facility, CollectorHelper.CreateReliableCollector(logbusHost, SyslogTlsReceiver.TLS_PORT));
        }

        /// <summary>
        /// Creates a logger with default configuration
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Configuration is not set or invalid</exception>
        [Obsolete("You should use CreateLogger instead")]
        public static ILog CreateDefaultLogger()
        {
            return GetLogger();
        }

        /// <summary>
        /// Creates a logger by logger name
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        [Obsolete("You should us CreateLogger instead")]
        public static ILog CreateLoggerByName(string loggerName)
        {
            return GetLogger(loggerName);
        }


    }
}
