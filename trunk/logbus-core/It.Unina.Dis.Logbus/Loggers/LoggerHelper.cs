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
using It.Unina.Dis.Logbus.Configuration;
using System;
using It.Unina.Dis.Logbus.InChannels;

namespace It.Unina.Dis.Logbus.Loggers
{
    /// <summary>
    /// This class provides services for Log sources (clients which generate log messages and want to send them to Logbus)
    /// </summary>
    public sealed class LoggerHelper
    {

        private LoggerHelper() { }

        static LoggerHelper()
        {
            try
            {
                Configuration = ConfigurationHelper.SourceConfiguration;
            }
            catch (LogbusConfigurationException) { }
        }

        /// <summary>
        /// Gets or sets global source configuration
        /// </summary>
        public static LogbusSourceConfiguration Configuration
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a logger basing on configuration
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">No valid configuration is available. You should use another method for a manual approach</exception>
        public static ILogCollector CreateDefaultCollector()
        {
            if (Configuration != null && Configuration.logger != null || Configuration.logger.Length > 0)
            {
                //Try to find the first logger marked default
                foreach (LoggerDefinition def in Configuration.logger)
                {
                    if (def.@default) return CreateByDefinition(def);
                }
            }
            //Else get default
            return new ConsoleLogger();
        }

        /// <summary>
        /// Creates a logger by name
        /// </summary>
        /// <param name="loggerName">Name of logger</param>
        /// <returns></returns>
        /// <remarks>There are special well-known loggers:
        /// <list>
        /// <item><code>Logbus</code><description>Logger that forwards messages to the current Logbus instance</description></item>
        /// </list></remarks>
        public static ILogCollector CreateCollectorByName(string loggerName)
        {
            if (string.IsNullOrEmpty(loggerName)) throw new ArgumentNullException("loggerName");

            if (Configuration != null && Configuration.logger != null && Configuration.logger.Length > 0)
            {
                //Try to find the first logger marked default
                foreach (LoggerDefinition def in Configuration.logger)
                {
                    if (def.name == loggerName) return CreateByDefinition(def);
                }
            }
            //Let's see if the logger name is well-knwon
            switch (loggerName)
            {
                case "Logbus":
                    {
                        return LogbusSingletonHelper.Instance;
                    }
            }
            //Else throw error: logger is not defined in configuration
            throw new LogbusException(string.Format("Logger {0} not found", loggerName));


        }

        /// <summary>
        /// Creates a Log collector which uses Syslog UDP sending
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <returns>A new instance of ILogCollector to submit SyslogMessages</returns>
        public static ILogCollector CreateUdpCollector(IPAddress logbus_ip, int logbus_port)
        {
            return new SyslogUdpLogger(logbus_ip, logbus_port);
        }

        /// <summary>
        /// Creates a Log collector which uses Syslog TLS sending
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <returns>A new instance of ILogCollector to submit SyslogMessages</returns>
        public static ILogCollector CreateReliableCollector(string logbus_host, int logbus_port)
        {
            return new SyslogTlsLogger(logbus_host, logbus_port);
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        [Obsolete("You should use CreateUnreliableLogger instead")]
        public static ILog CreateUdpLogger(IPAddress logbus_ip, int logbus_port)
        {
            return new SimpleLogImpl(CreateUdpCollector(logbus_ip, logbus_port));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog CreateUnreliableLogger(IPAddress logbus_ip, int logbus_port)
        {
            return new SimpleLogImpl(CreateUdpCollector(logbus_ip, logbus_port));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP, using default port
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog CreateUnreliableLogger(IPAddress logbus_ip)
        {
            return new SimpleLogImpl(CreateUdpCollector(logbus_ip, SyslogUdpReceiver.DEFAULT_PORT));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via TLS
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">TLS port of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog CreateReliableLogger(string logbus_host, int logbus_port)
        {
            return new SimpleLogImpl(CreateReliableCollector(logbus_host, logbus_port));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via TLS, using default port
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog CreateReliableLogger(string logbus_host)
        {
            return new SimpleLogImpl(CreateReliableCollector(logbus_host, SyslogTlsReceiver.TLS_PORT));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        [Obsolete("You should use CreateUnreliableLogger instead")]
        public static ILog CreateUdpLogger(IPAddress logbus_ip, int logbus_port, SyslogFacility facility)
        {
            return new SimpleLogImpl(facility, CreateUdpCollector(logbus_ip, logbus_port));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog CreateUnreliableLogger(IPAddress logbus_ip, int logbus_port, SyslogFacility facility)
        {
            return new SimpleLogImpl(facility, CreateUdpCollector(logbus_ip, logbus_port));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via UDP, using default port
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog CreateUnreliableLogger(IPAddress logbus_ip, SyslogFacility facility)
        {
            return new SimpleLogImpl(facility, CreateUdpCollector(logbus_ip, SyslogUdpReceiver.DEFAULT_PORT));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via TLS
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog CreateReliableLogger(string logbus_host, int logbus_port, SyslogFacility facility)
        {
            return new SimpleLogImpl(facility, CreateReliableCollector(logbus_host, logbus_port));
        }

        /// <summary>
        /// Creates a simple logger that sends messages in Syslog format via TLS, using default port
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="facility">Syslog facility to override</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        public static ILog CreateReliableLogger(string logbus_host, SyslogFacility facility)
        {
            return new SimpleLogImpl(facility, CreateReliableCollector(logbus_host, SyslogTlsReceiver.TLS_PORT));
        }

        /// <summary>
        /// Creates a logger with default configuration
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Configuration is not set or invalid</exception>
        public static ILog CreateDefaultLogger()
        {
            return new SimpleLogImpl(CreateDefaultCollector());
        }

        /// <summary>
        /// Creates a logger by logger name
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        public static ILog CreateLoggerByName(string loggerName)
        {
            return new SimpleLogImpl(CreateCollectorByName(loggerName));
        }

        internal static ILogCollector CreateByDefinition(LogCollectorDefinition def)
        {
            if (def == null) throw new ArgumentNullException("def");
            try
            {
                string typename = def.type;
                if (typename.IndexOf('.') < 0)
                {
                    //This is probably a plain class name, overriding to It.Unina.Dis.Logbus.InChannels namespace
                    string namespc = "It.Unina.Dis.Logbus.Loggers";
                    string assemblyname = typeof(LoggerHelper).Assembly.GetName().ToString();
                    typename = string.Format("{0}.{1}, {2}", namespc, typename, assemblyname);
                }
                Type logger_type = Type.GetType(typename);
                if (!typeof(ILogCollector).IsAssignableFrom(logger_type))
                {
                    LogbusConfigurationException ex = new LogbusConfigurationException("Registered logger type does not implement ILogCollector");
                    ex.Data.Add("type", logger_type);
                    throw ex;
                }
                ILogCollector ret = Activator.CreateInstance(logger_type) as ILogCollector;
                if (def.param != null && ret is IConfigurable)
                    foreach (KeyValuePair kvp in def.param)
                        ((IConfigurable)ret).SetConfigurationParameter(kvp.name, kvp.value);

                return ret;
            }
            catch (Exception ex)
            {
                throw new LogbusConfigurationException("Invalid logger configuration", ex);
            }
        }
    }
}
