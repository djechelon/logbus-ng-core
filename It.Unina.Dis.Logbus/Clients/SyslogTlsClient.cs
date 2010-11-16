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
        private const int DEFAULT_JOIN_TIMEOUT = 5000;

        private TcpListener _server;
        private string _localhost;

        private bool _running;
        private Thread _listenerThread, _processingThread;
        private string _clientId;
        private readonly IFifoQueue<byte[]> _queue = new BlockingFifoQueue<byte[]>();

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

        #endregion

        #region IRunnable Membri di

        public override void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().ToString());
            if (_running) throw new NotSupportedException("Client is already running");
            try
            {
                Log.Info("TLS client starting");
                CancelEventArgs arg = new CancelEventArgs();
                OnStarting(arg);
                if (arg.Cancel)
                    return;

                bool supported = false;
                foreach (string transport in ChannelSubscriber.GetAvailableTransports())
                    if (transport == "tls")
                    {
                        supported = true;
                        break;
                    }
                if (!supported)
                    throw new NotSupportedException(
                        "Remote Logbus-ng node does not support TLS protocol for delivery. You must use a different client");


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


                _listenerThread = new Thread(ListenerLoop);
                _listenerThread.Start();
                _processingThread = new Thread(ProcessLoop);
                _processingThread.Start();


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
                                                                             {
                                                                                 name = "host",
                                                                                 value = _localhost
                                                                             },
                                                                         new KeyValuePair
                                                                             {
                                                                                 name = "ip",
                                                                                 value = GetIpAddress().ToString()
                                                                             }
                                                                     }
                                                     };
                ChannelSubscriptionResponse res = ChannelSubscriber.SubscribeChannel(req);
                _clientId = res.clientid;

                _running = true;

                OnStarted(EventArgs.Empty);
                Log.Info("TLS client started");
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

                throw new LogbusException("Unable to subscribe channel", ex);
            }
        }

        public override void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().ToString());
            if (!_running) throw new NotSupportedException("Client is not running");

            try
            {
                Log.Info("TLS client stopping");

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

                _processingThread.Interrupt();
                if (!_listenerThread.Join(DEFAULT_JOIN_TIMEOUT))
                {
                    _listenerThread.Abort();
                    _listenerThread.Join();
                }
                _listenerThread = null;
                _processingThread.Join();
                _processingThread = null;

                _running = false;

                OnStopped(EventArgs.Empty);
                Log.Info("TLS client stopped");
            }
            catch (Exception ex)
            {
                OnError(new UnhandledExceptionEventArgs(ex, true));
                Log.Error("Error stopping TLS client");
                Log.Debug("Error details: {0}", ex.Message);
                if (ex is LogbusException) throw;
                throw new LogbusException("Unable to unsubscribe channel", ex);
            }
        }

        #endregion

        private void ProcessLoop()
        {
            try
            {
                while (true)
                {
                    byte[] data = _queue.Dequeue();

                    try
                    {
                        SyslogMessage msg = SyslogMessage.Parse(data);

                        OnMessageReceived(new SyslogMessageEventArgs(msg));
                    }
                    catch (FormatException ex)
                    {
                        OnError(new UnhandledExceptionEventArgs(ex, false));
                        continue;
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                return;
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
        }

        private void ListenerLoop()
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


                    while (true)
                    {
                        StringBuilder sb = new StringBuilder();
                        do
                        {
                            char nextChar = (char)stream.ReadByte();
                            if (char.IsDigit(nextChar)) sb.Append(nextChar);
                            else if (nextChar == ' ') break;
                            else throw new FormatException("Invalid TLS encoding of Syslog message");
                        } while (true);

                        int charLen = int.Parse(sb.ToString(), CultureInfo.InvariantCulture);

                        byte[] buffer = new byte[charLen];
                        int position = 0;
                        do
                        {
                            int read = stream.Read(buffer, position, charLen);
                            position += read;
                            charLen -= read;
                        } while (charLen > 0);

                        _queue.Enqueue(buffer);
                    }
                }


                /*catch (FormatException) { Stop(); }
                catch (SocketException) { Stop(); }
                catch (IOException) { Stop(); }*/
            }
            catch (SocketException ex)
            {
                if (!Running) return; //OK

                OnError(new UnhandledExceptionEventArgs(ex, true));
                Log.Error("Error receiving data from Logbus-ng server");
                Log.Debug("Error details: {0}", ex.Message);
            }
            catch (ThreadAbortException)
            {
                return;
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