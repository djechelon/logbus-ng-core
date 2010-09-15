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
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
namespace It.Unina.Dis.Logbus.Loggers
{
    /// <summary>
    /// Logs to a remote server via Syslog TLS Transport (RFC 5425)
    /// </summary>
    internal class SyslogTlsLogger
        : ILogCollector, IConfigurable, IDisposable
    {

        #region Constructor/Destructor

        public SyslogTlsLogger(string remote_ip, int remote_port)
        {
            this.remote_host = remote_host;
            this.port = remote_port;
        }

        ~SyslogTlsLogger()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (client != null) client.Close();
            }
        }

        #endregion

        public IPEndPoint RemoteEndPoint { get; set; }

        private TcpClient client;
        private string remote_host;
        private int port;
        private SslStream remote_stream;
        private string certificate_path;
        private X509Certificate clientCertificate;
        private StreamWriter sw;

        #region ILogCollector Membri di

        public void SubmitMessage(SyslogMessage message)
        {
            if (client == null)
            {
                if (remote_host == null) throw new InvalidOperationException("Remote address not specified");
                if (port < 1 || port > 65535) port = InChannels.SyslogTlsReceiver.TLS_PORT;

                client = new TcpClient();
            }

            if (!client.Connected)
                try
                {
                    client.Connect(remote_host, port);
                    remote_stream = new SslStream(client.GetStream());
                    remote_stream.WriteTimeout = 3600000;

                    if (clientCertificate != null)
                    {
                        X509CertificateCollection certs = new X509CertificateCollection(new X509Certificate[] { clientCertificate });
                        remote_stream.AuthenticateAsClient(remote_host, certs, SslProtocols.Tls, true);
                    }
                    else
                    {
                        remote_stream.AuthenticateAsClient(remote_host);
                    }
                    sw = new StreamWriter(remote_stream, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    throw new LogbusException("Unable to log to remote TLS host", ex);
                }

            string payload = message.ToRfc5424String();
            sw.Write(string.Format("{0} {1}", payload.Length.ToString(CultureInfo.InvariantCulture), payload));

        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region IConfigurable Membri di

        public string GetConfigurationParameter(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "Key cannot be null");
            switch (key)
            {
                case "host":
                    return remote_host;
                case "port":
                    return port.ToString(CultureInfo.InvariantCulture);
                case "certificate":
                    return certificate_path;
                default:
                    {
                        NotSupportedException ex = new NotSupportedException("Invalid key");
                        ex.Data.Add("key", key);
                        throw ex;
                    }

            }
        }

        public void SetConfigurationParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "Key cannot be null");
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value", "Value cannot be null");
            switch (key)
            {
                case "host":
                    {
                        try
                        {
                            remote_host = value;
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException("Invalid IP address for remote endpoint", "value", ex);
                        }
                        break;
                    }

                case "port":
                    {
                        try
                        {
                            port = int.Parse(value);
                            if (port < 0 || port > 65535)
                                throw new ArgumentOutOfRangeException("value", port, "Port must be between 0 and 65535");
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException("Port must be integer", "value", ex);
                        }
                        break;
                    }
                case "certificate":
                    {
                        certificate_path = value;
                        try
                        {
                            clientCertificate = new X509Certificate(certificate_path);
                        }
                        catch { }
                        break;
                    }
                default:
                    throw new NotSupportedException("Invalid key");

            }
        }

        public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Configuration
        {
            set
            {
                foreach (KeyValuePair<string, string> kvp in value)
                    SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }

        #endregion
    }
}
