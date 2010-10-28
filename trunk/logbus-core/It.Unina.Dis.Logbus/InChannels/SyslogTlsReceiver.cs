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

using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
using System;
using System.Globalization;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using It.Unina.Dis.Logbus.Configuration;

namespace It.Unina.Dis.Logbus.InChannels
{
    /// <summary>
    /// Syslog TLS receiver (RFC 5425)
    /// </summary>
    /// <remarks>Configuration parameters:
    /// <list>
    /// <item><c>ip</c>: IP to bind (default any)</item>
    /// <item><c>port</c>: port to bind (default 6514)</item>
    /// <item><c>ip</c></item>
    /// </list>
    /// </remarks>
    internal sealed class SyslogTlsReceiver
        : ReceiverBase
    {
        /// <summary>
        /// Default port for TLS transport as defined in RFC 5425
        /// </summary>
        public const int TLS_PORT = 6514;

        /// <summary>
        /// Number of worker threads concurrently listening for datagrams
        /// </summary>
        public const int WORKER_THREADS = 4;

        private TcpListener _listener;
        private string _certificatePath;
        private Thread[] _listenerThreads;
        private readonly List<TcpClient> _clients = new List<TcpClient>();

        /// <summary>
        /// Port to listen on
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Interface to listen on
        /// </summary>
        public string IpAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the SSL certificate for the current server
        /// </summary>
        public X509Certificate2 Certificate
        {
            get;
            set;
        }

        /// <summary>
        /// Implements ReceiverBase.OnStart
        /// </summary>
        protected override void OnStart()
        {
            if (Port == 0)
            {
                Port = TLS_PORT;
            }

            IPEndPoint localEp = IpAddress == null ? new IPEndPoint(IPAddress.Any, Port) : new IPEndPoint(IPAddress.Parse(IpAddress), Port);

            if (Certificate == null)
                Certificate = Utils.CertificateUtilities.DefaultCertificate;

            try
            {
                _listener = new TcpListener(localEp);
                _listener.Start();
            }
            catch (IOException ex)
            {
                throw new LogbusException("Cannot start TLS/TCP listener", ex);
            }

            _listenerThreads = new Thread[WORKER_THREADS];
            for (int i = 0; i < WORKER_THREADS; i++)
            {
                _listenerThreads[i] = new Thread(ListenerLoop)
                                          {
                                              Name = string.Format("SyslogTlsReceiver[{1}].ListenerLoop[{0}]", i, Name),
                                              IsBackground = true
                                          };
                _listenerThreads[i].Start();
            }
        }

        /// <summary>
        /// Implements ReceiverBase.OnStart
        /// </summary>
        protected override void OnStop()
        {
            lock (_clients)
                foreach (TcpClient client in _clients)
                {
                    try
                    {
                        client.Close();
                    }
                    catch { }
                }

            try
            {
                _listener.Stop();
                for (int i = 0; i < WORKER_THREADS; i++)
                    _listenerThreads[i].Join();
                _listenerThreads = null;
            }
            catch (Exception) { } //Really nothing?
        }

        #region Configuration
        /// <summary>
        /// Implements IConfigurable.GetConfigurationParameter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string GetConfigurationParameter(string key)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            switch (key)
            {
                case "ip":
                    return IpAddress;
                case "port":
                    return Port.ToString(CultureInfo.InvariantCulture);
                case "certificate":
                    return _certificatePath;
                default:
                    {
                        throw new NotSupportedException("Configuration parameter is not supported");
                    }
            }
        }

        /// <summary>
        /// Implements IConfigurable.SetConfigurationParameter
        /// </summary>
        public override void SetConfigurationParameter(string key, string value)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            switch (key)
            {
                case "ip":
                    {
                        IpAddress = value;
                        break;
                    }
                case "port":
                    {
                        Port = int.Parse(value);
                        break;
                    }
                case "certificate":
                    {

                        try
                        {
                            Certificate = Utils.CertificateUtilities.LoadCertificate(value);
                        }
                        catch (LogbusException ex)
                        {
                            throw new LogbusConfigurationException("Invalid certificate configuration", ex);
                        }
                        _certificatePath = value;
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter is not supported");
                    }
            }
        }

        /// <summary>
        /// Implements IConfigurable.Configuration
        /// </summary>
        public override IEnumerable<KeyValuePair<string, string>> Configuration
        {
            set
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                foreach (KeyValuePair<string, string> kvp in value)
                    SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }
        #endregion

        /// <summary>
        /// Accepts clients
        /// </summary>
        private void ListenerLoop()
        {
            while (Running)
                try
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    Thread clientThread = new Thread(ProcessClient)
                                              {
                                                  Name =
                                                      string.Format("TlsListener.ProcessClient[{0}]",
                                                                    client.Client.RemoteEndPoint.ToString()),
                                                  IsBackground = true
                                              };
                    clientThread.Start(client);
                }
                catch (SocketException) { }
        }

        /// <summary>
        /// Threads spawned after connection use this loop
        /// </summary>
        /// <param name="clientObj"></param>
        private void ProcessClient(object clientObj)
        {
            using (TcpClient client = (TcpClient)clientObj)
            {
                lock (_clients) _clients.Add(client);
                // A client has connected. Create the 
                // SslStream using the client's network stream.
                using (SslStream sslStream = new SslStream(client.GetStream(), false))
                    // Authenticate the server but don't require the client to authenticate.
                    try
                    {
                        sslStream.AuthenticateAsServer(Certificate, false, SslProtocols.Tls, true);

                        sslStream.ReadTimeout = 3600000; //1 hour

                        using (StreamReader sr = new StreamReader(sslStream, Encoding.UTF8, true))
                            while (true)
                            {
                                StringBuilder sb = new StringBuilder();
                                do
                                {
                                    char nextChar = (char)sr.Read();
                                    if (char.IsDigit(nextChar)) sb.Append(nextChar);
                                    else if (nextChar == ' ') break;
                                    else throw new FormatException("Invalid TLS encoding of Syslog message");
                                } while (true);

                                int charLen = int.Parse(sb.ToString(), CultureInfo.InvariantCulture);

                                char[] buffer = new char[charLen];
                                if (sr.Read(buffer, 0, charLen) != charLen)
                                {
                                    throw new FormatException("Invalid TLS encoding of Syslog message");
                                }

                                ForwardMessage(SyslogMessage.Parse(new string(buffer)));
                            }

                    }
                    catch { return; }
                    finally
                    {
                        lock (_clients) _clients.Remove(client);
                    }
            }

        }

    }
}
