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
using System.Security.Cryptography.X509Certificates;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Design;
using It.Unina.Dis.Logbus.Loggers;
using It.Unina.Dis.Logbus.Utils;

namespace It.Unina.Dis.Logbus.OutTransports
{
    /// <remarks>
    /// Supported configuration parameters:
    /// <list>
    /// <item><c>validateClientCertificate</c> (true|false [case insensitive]): whether to accept or not a client's subscription only if the client owns a valid certificate*</item>
    /// </list>
    /// 
    /// *Logbus-ng client must own a valid SSL SERVER certificate, see documentation for details
    /// </remarks>
    [TransportFactory("tls", Name = "TLS transport", Description = "Reliable transport implementing RFC 5425")]
    internal sealed class SyslogTlsTransportFactory
        : IOutboundTransportFactory, ILogSupport
    {
        private string _certificatePath;
        private ILog _logger;

        public SyslogTlsTransportFactory()
        {
            ServerCertificate = CertificateUtilities.DefaultCertificate;
        }

        /// <summary>
        /// Whether to validate or not client certificate
        /// </summary>
        public bool ValidateClientCertificate { get; set; }

        public X509Certificate2 ServerCertificate { get; set; }

        #region IOutboundTransportFactory Membri di

        public IOutboundTransport CreateTransport()
        {
            return new SyslogTlsTransport(ServerCertificate, ValidateClientCertificate) {Log = Log};
        }

        #endregion

        #region IConfigurable Membri di

        public string GetConfigurationParameter(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            switch (key)
            {
                case "certificatePath":
                    {
                        return _certificatePath;
                    }
                case "validateClientCertificate":
                    {
                        return ValidateClientCertificate ? "true" : "false";
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter not supported by TLS transport");
                    }
            }
        }

        public void SetConfigurationParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            switch (key)
            {
                case "certificatePath":
                    {
                        try
                        {
                            ServerCertificate = (string.IsNullOrEmpty(value))
                                                    ? null
                                                    : CertificateUtilities.LoadCertificate(value);
                            _certificatePath = value;
                        }
                        catch (LogbusException ex)
                        {
                            throw new LogbusConfigurationException("Invalid certificate configuration", ex);
                        }
                        break;
                    }
                case "validateClientCertificate":
                    {
                        try
                        {
                            ValidateClientCertificate = bool.Parse(value.ToLower());
                        }
                        catch (FormatException ex)
                        {
                            throw new LogbusConfigurationException(
                                "TLS transport \"validateClientCertificate\" parameter must be boolean", ex);
                        }
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter not supported by TLS transport");
                    }
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Configuration
        {
            set { foreach (KeyValuePair<string, string> kvp in value) SetConfigurationParameter(kvp.Key, kvp.Value); }
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