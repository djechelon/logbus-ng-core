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
using System.Net.Sockets;
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

namespace It.Unina.Dis.Logbus.Collectors
{
    internal sealed class SyslogUdpCollector
        : ILogCollector, IConfigurable, IDisposable
    {

        #region Constrcutor
        public SyslogUdpCollector()
        {
            _client = new UdpClient();
        }

        public SyslogUdpCollector(string host, int port)
            : this(new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], port))
        {
        }

        public SyslogUdpCollector(IPAddress ip, int port)
            : this(new IPEndPoint(ip, port))
        {
        }

        public SyslogUdpCollector(IPEndPoint endpoint)
            : this()
        {
            RemoteEndPoint = endpoint;
        }

        ~SyslogUdpCollector()
        {
            Dispose(false);
        }
        #endregion

        public IPEndPoint RemoteEndPoint { get; set; }

        private UdpClient _client;
        private IPAddress _remoteAddr;
        private int _port;
        private IAsyncResult _result;

        #region ILogCollector Membri di

        public void SubmitMessage(SyslogMessage message)
        {
            if (RemoteEndPoint == null)
            {
                if (_port == 0 || _remoteAddr == null)
                    throw new InvalidOperationException("Logger is not configured");

                RemoteEndPoint = new IPEndPoint(_remoteAddr, _port);
            }


            byte[] payload = Encoding.UTF8.GetBytes(message.ToRfc5424String());
            try
            {
                _result = _client.BeginSend(payload, payload.Length, RemoteEndPoint, null, null);
            }
            catch (IOException)
            { }
            catch (SocketException)
            { }
            catch (Exception ex)
            {
                throw new LogbusException("Unable to send Syslog message", ex);
            }
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);

            if (_client != null && _result != null)
                _client.EndSend(_result);

            if (disposing && _client != null)
                _client.Close();

            _client = null;
            RemoteEndPoint = null;
        }
        #endregion

        #region IConfigurable Membri di

        public string GetConfigurationParameter(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key", "Key cannot be null");
            switch (key)
            {
                case "ip":
                    return (_remoteAddr == null) ? null : _remoteAddr.ToString();
                case "port":
                    return _port.ToString(CultureInfo.InvariantCulture);
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
                case "ip":
                    {
                        try
                        {
                            _remoteAddr = IPAddress.Parse(value);
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                _remoteAddr = Dns.GetHostEntry(value).AddressList[0];
                                break;
                            }
                            catch { }
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
