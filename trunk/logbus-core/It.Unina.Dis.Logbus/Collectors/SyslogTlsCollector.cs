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
using It.Unina.Dis.Logbus.Loggers;
using System.Threading;
using It.Unina.Dis.Logbus.Utils;

namespace It.Unina.Dis.Logbus.Collectors
{
    /// <summary>
    /// Logs to a remote server via Syslog TLS Transport (RFC 5425)
    /// </summary>
    internal class SyslogTlsCollector
        : ILogCollector, IConfigurable, IDisposable
    {
        private const int QUEUE_CAPACITY = 128;

        private TcpClient _client;
        private string _host;
        private int _port;
        private SslStream _remoteStream;
        private string _certificatePath;
        private X509Certificate _clientCertificate;
        private bool _disposed;
        private readonly ILog _log;
        private readonly Thread _deliveryThread;
        private readonly IFifoQueue<SyslogMessage> _queue;
        private bool _configured;


        #region Constructor/Destructor

        public SyslogTlsCollector()
        {
            _log = LoggerHelper.GetLogger(WellKnownLogger.CollectorInternal);
            _queue = new FastFifoQueue<SyslogMessage>(QUEUE_CAPACITY);
            _deliveryThread = new Thread(DeliveryLoop) { Name = "SyslogTlsCollector.DeliveryThread" };
            _configured = false;
        }

        public SyslogTlsCollector(string remoteHost, int remotePort)
            : this()
        {
            _host = remoteHost;
            _port = remotePort;
            _configured = true;
            _deliveryThread.Start();
        }

        ~SyslogTlsCollector()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            GC.SuppressFinalize(this);

            _disposed = true;

            _deliveryThread.Interrupt();
            _deliveryThread.Join(5000);

            if (disposing)
            {
                if (_client != null) _client.Close();
            }
        }

        #endregion


        #region ILogCollector Membri di

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _queue.Enqueue(message);

            if (!_configured)
                if (string.IsNullOrEmpty(_host))
                    throw new InvalidOperationException("Unable to use SyslogTlsCollector without host name");
                else
                    lock (this)
                        if (!_configured)
                        {
                            _deliveryThread.Start();
                            _configured = true;
                        }
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
            if (_configured)
                throw new NotSupportedException("Unable to overwrite existing configuration");


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

            if (!string.IsNullOrEmpty(_host) && _port != 0)
            {
                _configured = true;
                _deliveryThread.Start();
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

        private void DeliveryLoop()
        {
            try
            {
                while (true)
                    try
                    {
                        if (_client == null)
                        {
                            if (_host == null) throw new InvalidOperationException("Remote address not specified");
                            if (_port < 1 || _port > 65535) _port = SyslogTlsReceiver.TLS_PORT;

                            _log.Debug("Creating new TcpClient for SyslogTlsCollector to {0}:{1}", _host, _port);
                            _client = new TcpClient
                                          {
                                              NoDelay = true,
                                              SendBufferSize = 1048576,
                                              SendTimeout = 3600000
                                          };
                        }

                        if (!_client.Connected)
                            try
                            {
                                _log.Debug("Connecting TlsCollector to host {0}:{1}", _host, _port);
                                // ReSharper disable AssignNullToNotNullAttribute
                                _client.Connect(_host, _port);
                                // ReSharper restore AssignNullToNotNullAttribute
                                _remoteStream = new SslStream(_client.GetStream(), false, TlsServerValidator,
                                                              TlsClientSelector) { WriteTimeout = 3600000 };

                                //_remoteStream.AuthenticateAsClient(_host, null, SslProtocols.Tls, true);
                                // ReSharper disable AssignNullToNotNullAttribute
                                _remoteStream.AuthenticateAsClient(_host);
                                // ReSharper restore AssignNullToNotNullAttribute
                                _log.Debug("TlsCollector connected to host {0}:{1}", _host, _port);
                            }
                            catch (ObjectDisposedException)
                            {
                                _client = null;
                                _remoteStream = null;
                                continue;
                            }
                            catch (Exception ex)
                            {
                                _log.Error("Error connecting TlsCollector to host {0}:{1}", _host, _port);
                                _log.Debug("Error details: {0}", ex.Message);
                                throw new LogbusException("Unable to log to remote TLS host", ex);
                            }

                        SyslogMessage[] msgs = _queue.Flush();
                        if (msgs == null || msgs.Length == 0) msgs = new[] { _queue.Dequeue() };

                        byte[] data;
                        using (MemoryStream ms = new MemoryStream(8192))
                        {
                            foreach (SyslogMessage msg in msgs)
                            {
                                byte[] payload = Encoding.UTF8.GetBytes(msg.ToRfc5424String());
                                foreach (char c in payload.Length.ToString(CultureInfo.InvariantCulture))
                                    ms.WriteByte((byte)c);

                                ms.WriteByte((byte)' ');

                                ms.Write(payload, 0, payload.Length);
                            }
                            data = ms.ToArray();
                        }

                        int offset = 0;
                        do
                        {
                            int len = Math.Min(data.Length - offset, 16384);
                            if (len <= 0) break;
                            _remoteStream.Write(data, offset, len);
                            offset += len;
                        } while (true);
                        _remoteStream.Flush();
                    }
                    catch (ObjectDisposedException)
                    {
                        _client = null;
                        _remoteStream = null;
                        _log.Error("TLS server {0}:{1} closed connection. Re-opening it", _host, _port);
                        continue;
                    }
                    catch (ThreadInterruptedException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _log.Alert("Unable to log to remote TLS host");
                        _log.Debug("Error details: {0}",
                                   (ex is LogbusException && ex.InnerException != null) ? ex.InnerException : ex);
                    }
            }
            catch (ThreadInterruptedException) { }
        }
    }
}