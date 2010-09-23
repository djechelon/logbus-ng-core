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
using System.Net;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Loggers;

namespace It.Unina.Dis.Logbus.Collectors
{
    /// <summary>
    /// Helper class for constructing log collectors
    /// </summary>
    public static class CollectorHelper
    {

        private static LogbusLoggerConfiguration Configuration
        {
            get { return LoggerHelper.Configuration; }
        }
        /// <summary>
        /// Constructs a logger basing on configuration
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">No valid configuration is available. You should use another method for a manual approach</exception>
        public static ILogCollector CreateDefaultCollector()
        {
            if (Configuration != null)
            {
                if (string.IsNullOrEmpty(Configuration.defaultcollector)) throw new InvalidOperationException("No default collector specifed in configuration file");

                return CreateCollectorByName(Configuration.defaultcollector);
            }
            //Else get default
            return new ConsoleCollector();
        }

        /// <summary>
        /// Creates a logger by name
        /// </summary>
        /// <param name="collectorName">Name of collector</param>
        /// <returns></returns>
        /// <remarks>There are special well-known loggers:
        /// <list>
        /// <item><code>Logbus</code><description>Collector that forwards messages to the current Logbus instance</description></item>
        /// </list></remarks>
        /// <exception cref="InvalidOperationException">No or invalid configuration is specified</exception>
        public static ILogCollector CreateCollectorByName(string collectorName)
        {
            if (string.IsNullOrEmpty(collectorName)) throw new ArgumentNullException("collectorName");

            if (Configuration == null || Configuration.collector == null || Configuration.collector.Length < 1)
                throw new InvalidOperationException("Invalid configuration. Either no configuration is set or no collector is specified");

            //Try to find the first logger marked default
            foreach (LogbusCollectorDefinition def in Configuration.collector)
            {
                if (def.id == collectorName) return CreateByDefinition(def);
            }

            //Let's see if the logger name is well-knwon
            switch (collectorName)
            {
                case "Logbus":
                    {
                        return LogbusSingletonHelper.Instance;
                    }
            }
            //Else throw error: logger is not defined in configuration
            throw new LogbusException(string.Format("Logger {0} not found", collectorName));
        }

        /// <summary>
        /// Creates a Log collector which uses Syslog UDP sending
        /// </summary>
        /// <param name="logbusIp">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <returns>A new instance of ILogCollector to submit SyslogMessages</returns>
        [Obsolete("You should use CreateUnreliableCollector instead", false)]
        public static ILogCollector CreateUdpCollector(IPAddress logbusIp, int logbusPort)
        {
            return CreateUnreliableCollector(logbusIp, logbusPort);
        }

        /// <summary>
        /// Creates a Log collector which uses Syslog UDP sending
        /// </summary>
        /// <param name="logbusIp">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <returns>A new instance of ILogCollector to submit SyslogMessages</returns>
        public static ILogCollector CreateUnreliableCollector(IPAddress logbusIp, int logbusPort)
        {
            return new SyslogUdpCollector(logbusIp, logbusPort);
        }

        /// <summary>
        /// Creates a Log collector which uses Syslog UDP sending
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <returns>A new instance of ILogCollector to submit SyslogMessages</returns>
        [Obsolete("You should use CreateUnreliableCollector instead", false)]
        public static ILogCollector CreateUdpCollector(string logbusHost, int logbusPort)
        {
            return CreateUnreliableCollector(logbusHost, logbusPort);
        }

        /// <summary>
        /// Creates a Log collector which uses Syslog UDP sending
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <returns>A new instance of ILogCollector to submit SyslogMessages</returns>
        public static ILogCollector CreateUnreliableCollector(string logbusHost, int logbusPort)
        {
            return new SyslogUdpCollector(logbusHost, logbusPort);
        }

        /// <summary>
        /// Creates a Log collector which uses Syslog TLS sending
        /// </summary>
        /// <param name="logbusHost">IP address of logbus target</param>
        /// <param name="logbusPort">UDP port of logbus target</param>
        /// <returns>A new instance of ILogCollector to submit SyslogMessages</returns>
        public static ILogCollector CreateReliableCollector(string logbusHost, int logbusPort)
        {
            return new SyslogTlsCollector(logbusHost, logbusPort);
        }

        internal static ILogCollector CreateByDefinition(LogCollectorDefinitionBase def)
        {
            if (def == null) throw new ArgumentNullException("def");
            try
            {
                string typename = def.type;
                if (typename.IndexOf('.') < 0)
                {
                    //This is probably a plain class name, overriding to It.Unina.Dis.Logbus.InChannels namespace
                    const string namespc = "It.Unina.Dis.Logbus.Collectors";
                    string assemblyname = typeof(CollectorHelper).Assembly.GetName().ToString();
                    typename = string.Format("{0}.{1}, {2}", namespc, typename, assemblyname);
                }
                Type loggerType = Type.GetType(typename);
                if (!typeof(ILogCollector).IsAssignableFrom(loggerType))
                {
                    LogbusConfigurationException ex = new LogbusConfigurationException("Registered collector type does not implement ILogCollector");
                    ex.Data.Add("type", loggerType);
                    throw ex;
                }
                ILogCollector ret = Activator.CreateInstance(loggerType) as ILogCollector;
                if (def.param != null && ret is IConfigurable)
                    foreach (KeyValuePair kvp in def.param)
                        ((IConfigurable)ret).SetConfigurationParameter(kvp.name, kvp.value);

                return ret;
            }
            catch (Exception ex)
            {
                throw new LogbusConfigurationException("Invalid collector configuration", ex);
            }
        }
    }
}
