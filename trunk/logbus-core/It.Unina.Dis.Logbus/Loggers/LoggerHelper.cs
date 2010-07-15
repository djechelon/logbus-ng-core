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
using System.Configuration;
using It.Unina.Dis.Logbus.Configuration;
using System;
using It.Unina.Dis.Logbus.Loggers;
using System.Collections.Generic;

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

        public static LogbusSourceConfiguration Configuration
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a logger basing on configuration
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No valid configuration is available. You should use another method for a manual approach</exception>
        public static ILogCollector CreateDefaultCollector()
        {
            if (Configuration == null) throw new InvalidOperationException("This method requires a valid logger configuration to be instanced first");

            if (Configuration.logger == null || Configuration.logger.Length == 0)
                return new NullLogger();
            else if (Configuration.logger.Length == 1)
            {
                //Just one logger
                LoggerDefinition def = Configuration.logger[0];
                if (def == null) throw new InvalidOperationException("Invalid logger definition");
                return CreateByDefinition(def);
            }
            else
            {
                //Need to use MultiLogger
                MultiLogger ret = new MultiLogger();
                List<ILogCollector> lst = new List<ILogCollector>();
                foreach (LoggerDefinition def in Configuration.logger)
                    lst.Add(CreateByDefinition(def));
                ret.Collectors = lst.ToArray();

                return ret;
            }
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
        /// Creates a simple logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <returns>An ILog, to which clients only submit text part of message, and severity is chosen by the invoked method</returns>
        /// <remarks>Facility is set to Local4 as default value</remarks>
        public static ILog CreateUdpLogger(IPAddress logbus_ip, int logbus_port)
        {
            return new SimpleLogImpl(CreateUdpCollector(logbus_ip, logbus_port));
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
            return new SimpleLogImpl(facility, CreateUdpCollector(logbus_ip, logbus_port));
        }

        /// <summary>
        /// Creates a logger with default configuration
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Configuration is not set or invalid</exception>
        public static ILog CreateDefaultLogger()
        {
            return new SimpleLogImpl(CreateDefaultCollector());
        }

        /// <summary>
        /// Creates an FFDA logger that sends messages in Syslog format via UDP
        /// </summary>
        /// <param name="logbus_ip">IP address of logbus target</param>
        /// <param name="logbus_port">UDP port of logbus target</param>
        /// <returns>An FFDALogger, to which clients could sumbit FFDA Messages</returns>
        /// <remarks>Facility is set to Local0 as default value</remarks>
        public static FFDALogger CreateFFDALogger(IPAddress logbus_ip, int logbus_port)
        {
            return new FFDALogger(CreateUdpCollector(logbus_ip, logbus_port));
        }

        /// <summary>
        /// Creates an FFDA logger with the default logger
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Configuration is not set or is invalid</exception>
        public static FFDALogger CreateFFDALogger()
        {
            return new FFDALogger(CreateDefaultCollector());
        }


        private static ILogCollector CreateByDefinition(LoggerDefinition def)
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
                {
                    IConfigurable logger_conf = ret as IConfigurable;
                    foreach (KeyValuePair kvp in def.param)
                        logger_conf.SetConfigurationParameter(kvp.name, kvp.value);
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Invalid logger configuration", ex);
            }
        }
    }
}
