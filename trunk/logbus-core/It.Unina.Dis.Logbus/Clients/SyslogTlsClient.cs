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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.RemoteLogbus;
using It.Unina.Dis.Logbus.Utils;

namespace It.Unina.Dis.Logbus.Clients
{
    /// <summary>
    /// Reliable Logbus-ng client using Transport Layer Security as protocol
    /// </summary>
    internal class SyslogTlsClient
        : ClientBase
    {
        public const int START_PORT = 20686, END_PORT = 25686;

        private TcpListener _server;
        private string _localhost;

        private bool _running;
        private Thread _runningThread;
        private string _clientId;

        #region Constructor/Destructor

        /// <summary>
        /// Initializes a new instance of SyslogTlsClient for running on an exclusive channel
        /// </summary>
        /// <param name="filter">Filter for exclusive channel</param>
        /// <param name="manager">Reference to Channel Manager</param>
        /// <param name="subscription">Reference to Channel Subscriber</param>
        /// <exception cref="LogbusException">Thrown when an error prevents to create a new channel</exception>
        public SyslogTlsClient(FilterBase filter, IChannelManagement manager, IChannelSubscription subscription)
            : base(filter, manager, subscription)
        {
        }

        /// <summary>
        /// Initializes a new instance of SyslogTlsClient for running on a shared channel
        /// </summary>
        /// <param name="channelId">ID of channel to subscribe</param>
        /// <param name="subscription">Reference to Channel Subscriber</param>
        public SyslogTlsClient(string channelId, IChannelSubscription subscription)
            : base(channelId, subscription)
        {
        }

        ~SyslogTlsClient()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);

            if (disposing)
            {
                try
                {
                    _server.Stop();
                }
                catch (SocketException)
                {
                }
                _server = null;
            }
        }

        #endregion

        #region IRunnable Membri di

        public override void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().ToString());
            if (_running) throw new NotSupportedException("Client is already running");
            try
            {
                Log.Info("TLS client starting...");
                CancelEventArgs arg = new CancelEventArgs();
                OnStarting(arg);
                if (arg.Cancel)
                    return;

                int port;
                //Decide on which address to listen
                IPAddress localIp = GetIpAddress();
                _localhost = (IPAddress.IsLoopback(localIp)) ? "localhost" : Dns.GetHostName();


                for (int i = START_PORT; i <= END_PORT; i++)
                {
                    try
                    {
                        _server = new TcpListener(new IPEndPoint(localIp, i));
                        _server.Start(1);
                        break;
                    }
                    catch (SocketException)
                    {
                    }
                }
                //Unable to bind to one of the default ports.
                //Now pray your firewall is open to all TCP ports
                if (_server == null)
                {
                    _server = new TcpListener(new IPEndPoint(localIp, 0));
                    _server.Start();
                }

                EndPoint ep = _server.Server.LocalEndPoint;
                if (ep is IPEndPoint)
                {
                    IPEndPoint ipe = (IPEndPoint)ep;
                    port = ipe.Port;
                }
                else
                {
                    throw new NotSupportedException("Only IP networks are supported");
                }


                _runningThread = new Thread(RunnerLoop) { IsBackground = true };
                _runningThread.Start();


                ChannelSubscriptionRequest req = new ChannelSubscriptionRequest
                                                     {
                                                         channelid = ChannelId,
                                                         transport = "tls",
                                                         param = new[]
                                                                     {
                                                                         new KeyValuePair
                                                                             {
                                                                                 name = "port",
                                                                                 value =
                                                                                     port.ToString(
                                                                                         CultureInfo.InvariantCulture)
                                                                             },
                                                                         new KeyValuePair
                                                                             {name = "host", value = _localhost}
                                                                     }
                                                     };
                ChannelSubscriptionResponse res = ChannelSubscriber.SubscribeChannel(req);
                _clientId = res.clientid;

                _running = true;

                OnStarted(EventArgs.Empty);
                Log.Info("TLS client started...");
            }
            catch (LogbusException ex)
            {
                OnError(new UnhandledExceptionEventArgs(ex, true));
                Log.Error("Error starting TLS client");
                Log.Debug("Error details: {0}", ex.Message);

                throw;
            }
            catch (Exception ex)
            {
                OnError(new UnhandledExceptionEventArgs(ex, true));
                Log.Error("Error starting TLS client");
                Log.Debug("Error details: {0}", ex.Message);

                throw new LogbusException("Unable to Subscribe Channel", ex);
            }
        }

        public override void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().ToString());
            if (!_running) throw new NotSupportedException("Client is not running");

            try
            {
                Log.Info("TLS client stopping...");

                CancelEventArgs arg = new CancelEventArgs();
                OnStopping(arg);
                if (arg.Cancel)
                    return;

                try
                {
                    ChannelSubscriber.UnsubscribeChannel(_clientId);
                }
                catch (LogbusException)
                {
                }
                _clientId = null;

                try
                {
                    _server.Stop();
                }
                catch (SocketException)
                {
                } //Really nothing?

                _runningThread.Abort();
                _runningThread.Join();
                _runningThread = null;

                _running = false;

                OnStopped(EventArgs.Empty);
                Log.Info("TLS client stopped...");
            }
            catch (Exception ex)
            {
                OnError(new UnhandledExceptionEventArgs(ex, true));
                Log.Error("Error stopping TLS client");
                Log.Debug("Error details: {0}", ex.Message);
                if (ex is LogbusException) throw;
                throw new LogbusException("Unable to Unsubscribe Channel", ex);
            }
        }

        #endregion

        #region IDisposable Membri di

        public override void Dispose()
        {
            Dispose(true);
        }

        #endregion

        private void RunnerLoop()
        {
            try
            {
                try
                {
                    //Wait for first connection
                    Log.Debug("Listening on TLS socket {0}", _server.LocalEndpoint.ToString());
                    TcpClient client = _server.AcceptTcpClient();
                    using (SslStream stream = new SslStream(client.GetStream(), false, RemoteCertificateValidation,
                                                            LocalCertificateSelection))
                    {
                        stream.AuthenticateAsServer(CertificateUtilities.DefaultCertificate, false, SslProtocols.Tls,
                                                    true);

                        stream.ReadTimeout = 3600000; //1 hour

                        using (StreamReader sr = new StreamReader(stream, Encoding.UTF8, true))
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

                                SyslogMessage msg = SyslogMessage.Parse(new string(buffer));
                                OnMessageReceived(new SyslogMessageEventArgs(msg));
                            }
                    }
                }
                catch (Exception ex)
                {
                    OnError(new UnhandledExceptionEventArgs(ex, true));

                    Log.Error("Error receiving Syslog messages from Logbus-ng server");
                    Log.Debug("Error details: {0}", ex.Message);

                    new Thread(delegate()
                    {
                        try
                        {
                            Stop();
                        }
                        catch { }
                    }).Start();
                    return;
                }
                /*catch (FormatException) { Stop(); }
                catch (SocketException) { Stop(); }
                catch (IOException) { Stop(); }*/
            }
            catch (ThreadAbortException)
            {
            }
        }

        private bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain,
                                                 SslPolicyErrors sslPolicyErrors)
        {
            return true; //For now....
        }

        private X509Certificate LocalCertificateSelection(Object sender, string targetHost,
                                                          X509CertificateCollection localCertificates,
                                                          X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return CertificateUtilities.DefaultCertificate;
        }
    }
}