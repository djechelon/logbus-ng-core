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
using It.Unina.Dis.Logbus.RemoteLogbus;
using System.Threading;
using System.ComponentModel;
using System.Net.Sockets;
using It.Unina.Dis.Logbus.Filters;
using System.Collections;
using It.Unina.Dis.Logbus.Utils;
using System.Globalization;
using System.Text.RegularExpressions;

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
    internal sealed class UdpLogClientImpl
        : ILogClient
    {

        public const int START_PORT = 20686, END_PORT = 25686;
        private const long MAX_REFRESH_TIME = 20000;

        private Timer _refreshTimer;
        private UdpClient _client;
        private long _channelTtl;

        private readonly FilterBase _filter;
        private bool ExclusiveUsage { get; set; }
        private bool Running { get; set; }
        private Thread _runningThread;
        private string _clientId;
        private readonly string _channelId;

        #region Constructor/Destructor
        /// <summary>
        /// Initializes a new instance of UdpLogClientImpl for running on an exclusive channel
        /// </summary>
        /// <param name="filter">Filter for exclusive channel</param>
        /// <param name="manager">Reference to Channel Manager</param>
        /// <param name="subscription">Reference to Channel Subscriber</param>
        /// <exception cref="LogbusException">Thrown when an error prevents to create a new channel</exception>
        public UdpLogClientImpl(FilterBase filter, IChannelManagement manager, IChannelSubscription subscription)
        {
            _channelTtl = 0;
            ExclusiveUsage = true;
            ChannelManager = manager;
            ChannelSubscriber = subscription;
            _filter = filter;

            string[] chIdsString = manager.ListChannels();
            ArrayList channelIds = new ArrayList(chIdsString ?? new string[0]);

            do
            {
                _channelId = string.Format("{0}{1}", Thread.CurrentThread.GetHashCode(), Randomizer.RandomAlphanumericString(5));
            } while (channelIds.Contains(_channelId));

            Init();
        }

        /// <summary>
        /// Initializes a new instance of UdpLogClientImpl for running on a shared channel
        /// </summary>
        /// <param name="channelId">ID of channel to subscribe</param>
        /// <param name="subscription">Reference to Channel Subscriber</param>
        public UdpLogClientImpl(string channelId, IChannelSubscription subscription)
        {
            _channelTtl = 0;
            ExclusiveUsage = false;
            _channelId = channelId;
            ChannelSubscriber = subscription;

            Init();
        }

        private void Init()
        {
            if (ExclusiveUsage)
            {
                //Create channel
                ChannelCreationInformation info = new ChannelCreationInformation
                                                      {
                                                          coalescenceWindow = 0,
                                                          description = "Channel created by LogCollector",
                                                          filter = _filter,
                                                          title = "AutoChannel",
                                                          id = _channelId
                                                      };

                try
                {
                    ChannelManager.CreateChannel(info);
                }
                catch (Exception ex)
                {
                    throw new LogbusException("Unable to create a new channel", ex);
                }
            }
        }

        ~UdpLogClientImpl()
        {
            Dispose(false);
        }

        #endregion

        #region Public properties

        public IChannelManagement ChannelManager
        { get; set; }

        public IChannelSubscription ChannelSubscriber
        { get; set; }

        #endregion

        private void RefreshChannel(Object status)
        {
            try
            {
                ChannelSubscriber.RefreshSubscription(status as string);
            }
            catch (Exception ex)
            {
                //Really kill the application?
                throw new LogbusException("Unable to Refresh", ex);
            }
        }

        private bool Disposed
        {
            get;
            set;
        }

        #region IRunnable Membri di

        public event EventHandler<CancelEventArgs> Starting;

        public event EventHandler<CancelEventArgs> Stopping;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event UnhandledExceptionEventHandler Error;

        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().ToString());
            if (Running) throw new NotSupportedException("Client is already running");
            try
            {
                if (Starting != null)
                {
                    CancelEventArgs arg = new CancelEventArgs();
                    Starting(this, arg);
                    if (arg.Cancel)
                        return;
                }
                int port;
                //Decide on which address to listen
                IPAddress localIp = GetIpAddress();
                for (int i = START_PORT; i <= END_PORT; i++)
                {
                    try
                    {
                        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp) { ExclusiveAddressUse = true };
                        socket.Bind(new IPEndPoint(localIp, i));

                        _client = new UdpClient { Client = socket };
                        break;
                    }
                    catch (SocketException)
                    { }
                }
                //Unable to bind to one of the default ports.
                //Now pray your firewall is open to all UDP ports
                if (_client == null) _client = new UdpClient(new IPEndPoint(localIp, 0));

                EndPoint ep = _client.Client.LocalEndPoint;
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


                ChannelSubscriptionRequest req = new ChannelSubscriptionRequest()
                {
                    channelid = _channelId,
                    transport = "udp",
                    param = new KeyValuePair[] 
                    { 
                        new KeyValuePair() { name = "port", value = port.ToString(CultureInfo.InvariantCulture) }, 
                        new KeyValuePair() { name = "ip", value = localIp.ToString() } 
                    }
                };
                ChannelSubscriptionResponse res = ChannelSubscriber.SubscribeChannel(req);
                _clientId = res.clientid;
                _channelTtl = MAX_REFRESH_TIME;
                foreach (KeyValuePair kvp in res.param)
                    if (kvp.name == "ttl")
                        if (long.TryParse(kvp.value, out _channelTtl)) break;
                long refreshTime = Math.Min(_channelTtl * 4 / 5, MAX_REFRESH_TIME); //80% of the max TTL, but not over max TTL

                _refreshTimer = new Timer(RefreshChannel, _clientId, refreshTime, refreshTime);

                Running = true;

                if (Started != null)
                    Started(this, EventArgs.Empty);
            }
            catch (LogbusException ex)
            {
                if (Error != null)
                    Error(this, new UnhandledExceptionEventArgs(ex, true));

                throw;
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new UnhandledExceptionEventArgs(ex, true));

                throw new LogbusException("Unable to Subscribe Channel", ex);
            }
        }

        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().ToString());
            if (!Running) throw new NotSupportedException("Client is not running");
            try
            {
                if (Stopping != null)
                {
                    CancelEventArgs arg = new CancelEventArgs();
                    Stopping(this, arg);
                    if (arg.Cancel)
                        return;
                }

                //Stop refreshing
                if (_refreshTimer != null)
                    _refreshTimer.Dispose();

                try
                {
                    ChannelSubscriber.UnsubscribeChannel(_clientId);
                }
                catch (LogbusException) { }
                _clientId = null;

                try
                {
                    _client.Close(); //Trigger SocketException if thread is blocked into listening
                    _runningThread.Interrupt();
                    _runningThread.Join();
                    _runningThread = null;
                }
                catch (SocketException) { } //Really nothing?

                Running = false;

                if (Stopped != null)
                    Stopped(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new UnhandledExceptionEventArgs(ex, true));

                if (ex is LogbusException) throw;
                throw new LogbusException("Unable to Unsubscribe Channel", ex);
            }
        }

        #endregion

        #region ILogSource Membri di

        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            Dispose(true);
        }

        private void DestroyChannel()
        {
            ChannelManager.DeleteChannel(_channelId);
        }

        public void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            try
            {
                Stop();
                if (ExclusiveUsage)
                    DestroyChannel();
            }
            catch { }

            if (disposing)
            {
                _client.Close();
                _client = null;
            }
            Disposed = true;
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
                        if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(newMessage));
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
                catch (Exception) { } //Really do nothing? Shouldn't we stop the service?
            }
        }

        /// <summary>
        /// Selects the best IP address to use for listening
        /// </summary>
        /// <returns></returns>
        private IPAddress GetIpAddress()
        {

            /*
             * If client is using OUR proxy, ie. no one re-implemented the
             * subscription interface with another mechanism like CORBA or
             * Remoting, we know where the Logbus web listener is located.
             * 
             * By creating a fake UDP client, we want C# to activate the
             * system's routing tables (unavailable from language) to find
             * the interface that is used to connect to Logbus. In scenarios
             * in which a machine is connected both to LAN(s) and WAN, Logbus
             * might either be in one of the LANs or in the WAN. If we chose
             * the WAN address (see below) as favourite address, we might
             * do an error, as that address won't work in the case Logbus
             * server has only LAN access.
             * 
             * This method to find the local address doesn't work in the
             * following scenario:
             * Logbus HTTP listener and the node that will actually send
             * the Syslog messages are deployed into different machines
             * that run in different subnets, especially if they have WAN
             * access. We will fix this situation too, but it's very rare.
            */
            if (ChannelSubscriber is ChannelSubscription)
            {
                try
                {
                    ChannelSubscription cs = ChannelSubscriber as ChannelSubscription;
                    string hostname = Regex.Match(cs.Url, "^(?<protocol>https?)://(?<host>[-A-Z0-9.]+)(?<port>:[0-9]{1,5})?(?<file>/[-A-Z0-9+&@#/%=~_|!:,.;]*)?(?<parameters>\\?[-A-Z0-9+&@#/%=~_|!:,.;]*)?",
                        RegexOptions.IgnoreCase).Groups["host"].Value;
                    IPAddress hostIp = Dns.GetHostAddresses(hostname)[0];

                    //If we are contacting a local host, tell to use loopback
                    if (hostIp.Equals(IPAddress.Loopback)) return IPAddress.Loopback;

                    //Just force a routing table lookup, we don't need more
                    UdpClient fakeClient = new UdpClient();
                    fakeClient.Connect(hostname, 65534);
                    return ((IPEndPoint)fakeClient.Client.LocalEndPoint).Address;

                }
                catch { } //Never mind...
            }
            //Else try to find the best WAN address to use


            /*
             *  The following mechanism might not work in the following scenario:
             *  -Client node is connected to LAN and WAN with 2 network cards
             *  -Logbus is listening on a computer that can access only LAN
             *  
             *  In this case, client tries to use WAN address for listening,
             *  but Logbus won't be able to send datagrams to it.
             *  This is a structural bug in the getIPAddress logic that may be
             *  solved by the above commented code, but...
             *  
             *  The above code, that implements a backup choice algorithm
             *  that forces the system's routing table to be activated, works
             *  by assuming that Logbus HTTP endpoint coincides with the host
             *  that actually sends UDP datagrams. While this happens 99% of
             *  times, it's not the only possible scenario.
            */

            return NetworkUtils.GetMyIPAddress();
        }

    }
}