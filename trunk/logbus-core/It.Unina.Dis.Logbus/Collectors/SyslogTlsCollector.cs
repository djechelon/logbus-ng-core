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
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using It.Unina.Dis.Logbus.InChannels;
using System.Runtime.CompilerServices;

namespace It.Unina.Dis.Logbus.Collectors
{
    /// <summary>
    /// Logs to a remote server via Syslog TLS Transport (RFC 5425)
    /// </summary>
    internal class SyslogTlsCollector
        : ILogCollector, IConfigurable, IDisposable
    {
        #region Constructor/Destructor

        public SyslogTlsCollector()
        {
        }

        public SyslogTlsCollector(string remoteHost, int remotePort)
        {
            _host = remoteHost;
            _port = remotePort;
        }

        ~SyslogTlsCollector()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            _disposed = true;

            if (disposing)
            {
                if (_client != null) _client.Close();
            }
        }

        #endregion

        public IPEndPoint RemoteEndPoint { get; set; }

        private TcpClient _client;
        private string _host;
        private int _port;
        private SslStream _remoteStream;
        private string _certificatePath;
        private X509Certificate _clientCertificate;
        private volatile bool _disposed;

        #region ILogCollector Membri di

        /// <remarks>
        /// TcpClient.Send is not multithread safe, better not doing concurrently
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (_client == null)
            {
                if (_host == null) throw new InvalidOperationException("Remote address not specified");
                if (_port < 1 || _port > 65535) _port = SyslogTlsReceiver.TLS_PORT;

                _client = new TcpClient();
            }

            if (!_client.Connected)
                try
                {
                    _client.Connect(_host, _port);
                    _remoteStream = new SslStream(_client.GetStream(), false, TlsServerValidator, TlsClientSelector) { WriteTimeout = 3600000 };

                    //remote_stream.AuthenticateAsClient(host, null, SslProtocols.Tls, true);
                    _remoteStream.AuthenticateAsClient(_host);
                }
                catch (Exception ex)
                {
                    throw new LogbusException("Unable to log to remote TLS host", ex);
                }

            byte[] payload = Encoding.UTF8.GetBytes(message.ToRfc5424String());
            byte[] lengthAndSpace = Encoding.UTF8.GetBytes(payload.Length.ToString(CultureInfo.InvariantCulture)+" ");
            _remoteStream.Write(lengthAndSpace,0,lengthAndSpace.Length);
            _remoteStream.Write(payload,0,payload.Length);
        }

        protected virtual bool TlsServerValidator(Object sender, X509Certificate certificate, X509Chain chain,
                                                  SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        protected virtual X509Certificate TlsClientSelector(Object sender, string targetHost,
                                                            X509CertificateCollection localCertificates,
                                                            X509Certificate remoteCertificate,
                                                            string[] acceptableIssuers)
        {
            return _clientCertificate;
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            if (_disposed) return;
            Dispose(true);
        }

        #endregion

        #region IConfigurable Membri di

        public string GetConfigurationParameter(string key)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "Key cannot be null");
            switch (key)
            {
                case "host":
                    return _host;
                case "port":
                    return _port.ToString(CultureInfo.InvariantCulture);
                case "certificate":
                    return _certificatePath;
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
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

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
                            _host = value;
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
                            _port = int.Parse(value);
                            if (_port < 0 || _port > 65535)
                                throw new ArgumentOutOfRangeException("value", _port, "Port must be between 0 and 65535");
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
                        _certificatePath = value;
                        try
                        {
                            _clientCertificate = new X509Certificate(_certificatePath);
                        }
                        catch
                        {
                        }
                        break;
                    }
                default:
                    throw new NotSupportedException("Invalid key");
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Configuration
        {
            set
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                foreach (KeyValuePair<string, string> kvp in value)
                    SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }

        #endregion
    }
}