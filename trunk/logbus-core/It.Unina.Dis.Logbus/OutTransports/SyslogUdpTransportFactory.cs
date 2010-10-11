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
using System.Globalization;
using System.Collections.Generic;
using It.Unina.Dis.Logbus.Loggers;
namespace It.Unina.Dis.Logbus.OutTransports
{
    /// <remarks>
    /// Supported configuration parameters:
    /// <list>
    /// <item><c>ttl</c>: time to live for clients. After that time without the client to refresh its subscription, it's considered dead and gets unsubscribed</item>
    /// </list>
    /// </remarks>
    [Design.TransportFactory("udp", Name = "RFC5426 transport", Description = "Syslog over UDP transport according to RFC5426")]
    internal sealed class SyslogUdpTransportFactory
        : IOutboundTransportFactory, ILogSupport
    {
        private Loggers.ILog _logger;
        private long _defaultTtl = 60000;

        #region IOutboundTransportFactory Membri di

        IOutboundTransport IOutboundTransportFactory.CreateTransport()
        {
            return new SyslogUdpTransport(_defaultTtl) { Log = Log };
        }

        string IConfigurable.GetConfigurationParameter(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            switch (key)
            {
                case "ttl":
                    {
                        return _defaultTtl.ToString(CultureInfo.InvariantCulture);
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter not supported by UDP transport");
                    }
            }
        }

        void IConfigurable.SetConfigurationParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            switch (key)
            {
                case "ttl":
                    {
                        if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _defaultTtl)) throw new InvalidOperationException("Invalid value for ttl");
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter not supported by UDP transport");
                    }
            }
        }

        IEnumerable<KeyValuePair<string, string>> IConfigurable.Configuration
        {
            set
            {
                foreach (KeyValuePair<string, string> kvp in value) ((IConfigurable)this).SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }

        #endregion

        #region ILogSupport Membri di

        public ILog Log
        {
            private get
            {
                if (_logger == null)
                {
                    _logger = LoggerHelper.GetLogger(WellKnownLogger.Logbus);
                    _logger.LogName = "SyslogUdpTransport";
                }

                return _logger;
            }
            set
            {
                _logger = value;
                if (string.IsNullOrEmpty(_logger.LogName))
                    _logger.LogName = "SyslogUdpTransport";
            }
        }

        #endregion
    }
}
