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
using System.Net.Sockets;
using System.Threading;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.OutTransports;
using It.Unina.Dis.Logbus.RemoteLogbus;
#if MONO
using System.Net.NetworkInformation;
#endif

namespace It.Unina.Dis.Logbus.Clients
{
    /// <summary>
    /// Default implementation for ILogClient
    /// </summary>
    /// <remarks>As default, the class tries to bind a free UDP port from 20686 to 25686. This grants you to keep alive up to 5000 UDP client instances.
    /// You shall set your firewall/NAT to properly allow/forward UDP traffic towards the above ports. If ports are exhausted (other clients or other applications
    /// bind to all the default 5000 ports) then the UDP client will ask the operating system to assign it a random port number. If this happens, you must be sure
    /// that your firewall doesn't block incoming UDP traffic.
    /// </remarks>
    internal sealed class SyslogUdpClient
        : ClientBase
    {
        public const int START_PORT = 20686, END_PORT = 25686;
        private const long MAX_REFRESH_TIME = 60000;

        private Timer _refreshTimer;
        private UdpClient _client;
        private long _channelTtl;


        private string _clientId;
        private bool _running;
        private Thread _runningThread;

        #region Constructor/Destructor

        /// <summary>
        /// Initializes a new instance of SyslogUdpClient for running on an exclusive channel
        /// </summary>
        /// <param name="filter">Filter for exclusive channel</param>
        /// <param name="manager">Reference to Channel Manager</param>
        /// <param name="subscription">Reference to Channel Subscriber</param>
        /// <exception cref="LogbusException">Thrown when an error prevents to create a new channel</exception>
        public SyslogUdpClient(FilterBase filter, IChannelManagement manager, IChannelSubscription subscription)
            : base(filter, manager, subscription)
        {
        }

        /// <summary>
        /// Initializes a new instance of SyslogUdpClient for running on a shared channel
        /// </summary>
        /// <param name="channelId">ID of channel to subscribe</param>
        /// <param name="subscription">Reference to Channel Subscriber</param>
        public SyslogUdpClient(string channelId, IChannelSubscription subscription)
            : base(channelId, subscription)
        {
        }

        ~SyslogUdpClient()
        {
            Dispose(false);
        }

        #endregion

        private void RefreshChannel(Object status)
        {
            string clientId = (string) status;
            try
            {
                ChannelSubscriber.RefreshSubscription(clientId);
            }
            catch (ClientNotSubscribedException)
            {
                Log.Notice("Client {0} on channel {1} expired. Trying to subscribe again", clientId, ChannelId);

                Stop();

                //Might fail in case of network problems
                Start();
            }
            catch (Exception ex)
            {
                OnError(new UnhandledExceptionEventArgs(ex, false));

                Log.Warning("Unable to refresh subscription of client {0} on channel {1}", _clientId, ChannelId);
                Log.Debug("Error details: {0}", ex.Message);

                try
                {
                    //Take some rest
                    Thread.Sleep((int) (_channelTtl/20));

                    ChannelSubscriber.RefreshSubscription(clientId);
                }
                catch (Exception e)
                {
                    OnError(new UnhandledExceptionEventArgs(e, true));

                    Log.Error(
                        "Unable to refresh subscription of client {0} on channel {1} for the second consecutive time",
                        _clientId, ChannelId);
                    Log.Debug("Error details: {0}", e.Message);

                    try
                    {
                        Stop();
                    }
                    catch
                    {
                    }
                }
            }
        }

        #region IRunnable Membri di

        public override void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().ToString());
            if (_running) throw new NotSupportedException("Client is already running");
            try
            {
                CancelEventArgs arg = new CancelEventArgs();
                OnStarting(arg);
                if (arg.Cancel)
                    return;

                bool supported = false;
                foreach (string transport in ChannelSubscriber.GetAvailableTransports())
                    if (transport == "udp")
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

                /* Workaround to Mono bug 643475, https://bugzilla.novell.com/show_bug.cgi?id=643475
                 * fixed since Mono master 512d3f2 and probably available from release next to 2.6.7
                 * */
#if MONO
                IPEndPoint[] eps = IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
#endif
                for (int i = START_PORT; i <= END_PORT; i++)
                {
#if MONO
                    //Workaround to Mono bug 64375
                    bool found = false;
                    for (int c = 0; c < eps.Length; c++)
                    {
                        if (eps[c].Port == i && (eps[c].Address.Equals(IPAddress.Any) || eps[c].Address.Equals(localIp)))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        socket.Bind(new IPEndPoint(localIp, i));
                        _client = new UdpClient { Client = socket };
                        break;
                    }
#else
                    try
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                                            {ExclusiveAddressUse = true};
                        socket.Bind(new IPEndPoint(localIp, i));

                        _client = new UdpClient {Client = socket};
                        break;
                    }
                    catch (SocketException)
                    {
                    }
#endif
                }
                //Unable to bind to one of the default ports.
                //Now pray your firewall is open to all UDP ports
                if (_client == null) _client = new UdpClient(new IPEndPoint(localIp, 0));

                EndPoint ep = _client.Client.LocalEndPoint;
                if (ep is IPEndPoint)
                {
                    IPEndPoint ipe = (IPEndPoint) ep;
                    port = ipe.Port;
                }
                else
                {
                    throw new NotSupportedException("Only IP networks are supported");
                }

                _runningThread = new Thread(RunnerLoop) {IsBackground = true};
                _runningThread.Start();


                ChannelSubscriptionRequest req = new ChannelSubscriptionRequest
                                                     {
                                                         channelid = ChannelId,
                                                         transport = "udp",
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
                                                                             {name = "ip", value = localIp.ToString()}
                                                                     }
                                                     };
                ChannelSubscriptionResponse res = ChannelSubscriber.SubscribeChannel(req);
                _clientId = res.clientid;
                _channelTtl = MAX_REFRESH_TIME;
                foreach (KeyValuePair kvp in res.param)
                    if (kvp.name == "ttl")
                        if (long.TryParse(kvp.value, out _channelTtl)) break;
                long refreshTime = Math.Min(_channelTtl*4/5, MAX_REFRESH_TIME);
                    //80% of the max TTL, but not over max TTL

                _refreshTimer = new Timer(RefreshChannel, _clientId, refreshTime, refreshTime);

                _running = true;

                OnStarted(EventArgs.Empty);
            }
            catch (LogbusException ex)
            {
                OnError(new UnhandledExceptionEventArgs(ex, true));

                throw;
            }
            catch (Exception ex)
            {
                OnError(new UnhandledExceptionEventArgs(ex, true));

                throw new LogbusException("Unable to subscribe channel", ex);
            }
        }

        public override void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().ToString());
            if (!_running) throw new NotSupportedException("Client is not running");
            try
            {
                CancelEventArgs arg = new CancelEventArgs();
                OnStopping(arg);
                if (arg.Cancel)
                    return;


                //Stop refreshing
                if (_refreshTimer != null)
                    _refreshTimer.Dispose();

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
                    _client.Close(); //Trigger SocketException if thread is blocked into listening
                }
                catch (SocketException)
                {
                } //Really nothing?

                _runningThread.Interrupt();
                _runningThread.Join();
                _runningThread = null;

                _running = false;

                OnStopped(EventArgs.Empty);
            }
            catch (Exception ex)
            {
                OnError(new UnhandledExceptionEventArgs(ex, true));

                if (ex is LogbusException) throw;

                throw new LogbusException("Unable to unsubscribe channel", ex);
            }
        }

        #endregion

        #region IDisposable Membri di

        public override void Dispose()
        {
            Dispose(true);
            base.Dispose();
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            try
            {
                Stop();
            }
            catch
            {
            }

            if (disposing)
            {
                _client.Close();
                _client = null;
            }

            //Implicit base.Finalize()
        }

        #endregion

        private void RunnerLoop()
        {
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] payload = _client.Receive(ref remoteEndpoint);
                    try
                    {
                        SyslogMessage newMessage = SyslogMessage.Parse(payload);
                        OnMessageReceived(new SyslogMessageEventArgs(newMessage));
                    }
                    catch (FormatException)
                    {
                        //Skip
                    }
                }
                catch (SocketException)
                {
                    //We are closing, or an I/O error occurred
                    //if (Stopped) //Yes, we are closing
                    return;
                }
                catch (Exception)
                {
                } //Really do nothing? Shouldn't we stop the service?
            }
        }
    }
}