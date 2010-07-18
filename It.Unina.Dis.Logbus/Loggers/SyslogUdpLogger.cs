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
namespace It.Unina.Dis.Logbus.Loggers
{
    internal sealed class SyslogUdpLogger
        : ILogger, IDisposable
    {

        #region Constrcutor
        public SyslogUdpLogger() { client = new UdpClient(); }

        public SyslogUdpLogger(string ip, int port)
            : this(new IPEndPoint(IPAddress.Parse(ip), port))
        {
        }

        public SyslogUdpLogger(IPAddress ip, int port)
            : this(new IPEndPoint(ip, port))
        {
        }

        public SyslogUdpLogger(IPEndPoint endpoint)
            : this()
        {
            RemoteEndPoint = endpoint;
        }

        ~SyslogUdpLogger()
        {
            Dispose(false);
        }
        #endregion

        public IPEndPoint RemoteEndPoint
        {
            get;
            set;
        }

        private UdpClient client;
        private IPAddress remote_addr;
        private int port;

        #region ILogCollector Membri di

        public void SubmitMessage(SyslogMessage message)
        {
            if (RemoteEndPoint == null)
            {
                if (port == 0 || remote_addr == null)
                    throw new InvalidOperationException("Logger is not configured");
                else
                {
                    RemoteEndPoint = new IPEndPoint(remote_addr, port);
                }
            }


            byte[] payload = Encoding.UTF8.GetBytes(message.ToRfc5424String());
            try
            {
                client.Send(payload, payload.Length, RemoteEndPoint);
            }
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
            if (disposing && client != null) client.Close();

            client = null;
            RemoteEndPoint = null;
        }
        #endregion

        #region IConfigurable Membri di

        public string GetConfigurationParameter(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key", "Key cannot be null");
            switch (key)
            {
                case "ip":
                    return (remote_addr == null) ? null : remote_addr.ToString();
                case "port":
                    return port.ToString(CultureInfo.InvariantCulture);
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
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key", "Key cannot be null");
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException("value", "Value cannot be null");
            switch (key)
            {
                case "ip":
                    {
                        try
                        {
                            remote_addr = IPAddress.Parse(value);
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
                            if (port < 0 || port > 65535) throw new ArgumentOutOfRangeException("value", port, "Port must be between 0 and 65535");
                        }
                        catch (ArgumentOutOfRangeException) { throw; }
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
