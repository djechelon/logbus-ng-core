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

using It.Unina.Dis.Logbus.InChannels;
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
    internal sealed class UdpLogClientImpl
        : ILogClient
    {

        private Timer refresh_timer;
        private UdpClient client;
        private long ChannelTTL = 0;
        private const long MAX_REFRESH_TIME = 20000;
        private FilterBase filter;
        private bool ExclusiveUsage { get; set; }
        private bool Running { get; set; }
        private Thread running_thread;
        private string clientId, channelId;

        #region Constructor/Destructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="manager"></param>
        /// <param name="subscription"></param>
        /// <exception cref="LogbusException">Thrown when an error prevents to create a new channel</exception>
        public UdpLogClientImpl(FilterBase filter, IChannelManagement manager, IChannelSubscription subscription)
        {
            ExclusiveUsage = true;
            ChannelManager = manager;
            ChannelSubscriber = subscription;
            this.filter = filter;

            ArrayList channel_ids = new ArrayList(manager.ListChannels());
            do
            {
                channelId = string.Format("{0}{1}", Thread.CurrentThread.GetHashCode(), Randomizer.RandomAlphanumericString(5));
            } while (channel_ids.Contains(channelId));

            Init();
        }

        public UdpLogClientImpl(string channelId, IChannelSubscription subscription)
        {
            ExclusiveUsage = false;
            this.channelId = channelId;
            ChannelSubscriber = subscription;

            Init();
        }

        private void Init()
        {
            if (ExclusiveUsage)
            {
                //Create channel
                ChannelCreationInformation info = new ChannelCreationInformation()
                {
                    coalescenceWindow = 0,
                    description = "Channel created by LogCollector",
                    filter = filter,
                    title = "AutoChannel",
                    id = channelId
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

        private void RefreshChannel(Object Status)
        {
            try
            {
                ChannelSubscriber.RefreshSubscription(Status as string);
            }
            catch (Exception ex)
            {
                throw new LogbusException("Unable to Refresh", ex);
            }
        }

        private bool Disposed
        {
            get;
            set;
        }

        #region IRunnable Membri di

        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

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
                IPAddress local_ip = getIPAddress();
                client = new UdpClient(new IPEndPoint(local_ip, 0));
                EndPoint ep = client.Client.LocalEndPoint;
                if (ep is IPEndPoint)
                {
                    IPEndPoint ipe = (IPEndPoint)ep;
                    port = ipe.Port;
                }
                else
                {
                    throw new NotSupportedException("Only IP networks are supported");
                }

                running_thread = new Thread(RunnerLoop);
                running_thread.IsBackground = true;
                running_thread.Start();
                

                ChannelSubscriptionRequest req = new ChannelSubscriptionRequest()
                {
                    channelid = channelId,
                    transport = "udp",
                    param = new KeyValuePair[2] { new KeyValuePair() { name = "port", value = port.ToString(CultureInfo.InvariantCulture) }, new KeyValuePair() { name = "ip", value = local_ip.ToString() } }
                };
                ChannelSubscriptionResponse res = ChannelSubscriber.SubscribeChannel(req);
                clientId = res.clientid;
                ChannelTTL = Int32.Parse(res.param[0].value);
                long refreshTime = ChannelTTL - (ChannelTTL * 20 / 100);
                refresh_timer = new Timer(RefreshChannel, channelId, Timeout.Infinite, (refreshTime < MAX_REFRESH_TIME) ? refreshTime : MAX_REFRESH_TIME);

                Running = true;

                if (Started != null)
                    Started(this, EventArgs.Empty);
            }
            catch (LogbusException ex)
            {
                if (Error != null)
                    Error(this, new UnhandledExceptionEventArgs(ex, true));

                throw ex;
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
                if (refresh_timer != null)
                    refresh_timer.Dispose();

                try
                {
                    ChannelSubscriber.UnsubscribeChannel(clientId);
                }
                catch (LogbusException) { }
                clientId = null;
                
                try
                {
                    client.Close(); //Trigger SocketException if thread is blocked into listening
                    running_thread.Join();
                    running_thread = null;
                }
                catch (Exception) { } //Really nothing?

                Running = false;

                if (Stopped != null)
                    Stopped(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new UnhandledExceptionEventArgs(ex, true));

                if (ex is LogbusException) throw ex;
                throw new LogbusException("Unable to Unsubscribe Channel", ex);
            }
        }

        #endregion

        #region ILogSource Membri di

        public event SyslogMessageEventHandler MessageReceived;

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            Dispose(true);
        }

        private void DestroyChannel()
        {
            ChannelManager.DeleteChannel(channelId);
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
                client.Close();
                client = null;
            }
            Disposed = true;
        }

        #endregion

        private void RunnerLoop()
        {
            IPEndPoint remote_endpoint = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                try
                {
                    byte[] payload = client.Receive(ref remote_endpoint);
                    try
                    {
                        SyslogMessage new_message = SyslogMessage.Parse(payload);
                        if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(new_message));
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
                    //return;
                    //else nothing yet
                }
                catch (Exception) { } //Really do nothing? Shouldn't we stop the service?
            }
        }

        /// <summary>
        /// Selects the best IP address to use for listening
        /// </summary>
        /// <returns></returns>
        private IPAddress getIPAddress()
        {
            /* Maybe we don't currently need this
            //Try to get some information on the remote Logbus host
            if (ChannelSubscriber is ChannelSubscription)
            {
                try
                {
                    ChannelSubscription cs = ChannelSubscriber as ChannelSubscription;
                    string hostname = Regex.Match(cs.Url, "^(?<protocol>https?)://(?<host>[-A-Z0-9.]+)(?<port>:[0-9]{1,5})?(?<file>/[-A-Z0-9+&@#/%=~_|!:,.;]*)?(?<parameters>\\?[-A-Z0-9+&@#/%=~_|!:,.;]*)?",
                        RegexOptions.IgnoreCase).Groups["host"].Value;
                    IPAddress host_ip = Dns.GetHostAddresses(hostname)[0];

                    //If we are contacting a local host, tell to use loopback
                    if (host_ip.Equals(IPAddress.Loopback)) return IPAddress.Loopback;

                    //Just force a routing table lookup, we don't need more
                    UdpClient fake_client = new UdpClient();
                    fake_client.Connect(hostname, 65534);
                    return ((IPEndPoint)fake_client.Client.LocalEndPoint).Address;

                }
                catch { } //Never mind...
            }
            //Else never mind...
            */

            /*
             *  This mechanism might not work in the following scenario:
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

            //All IPs available on this machine
            System.Net.IPAddress[] a = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());

            IPAddress preferred_v4 = null, preferred_v6 = null, ret = null;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    if (a[i].ToString().Contains("localhost"))
                        continue;
                    else if (a[i].GetAddressBytes()[0] == 0 ||
                        a[i].GetAddressBytes()[0] == 255 ||
                        a[i].GetAddressBytes()[0] == 240 ||
                        (a[i].GetAddressBytes()[0] == 203 && a[i].GetAddressBytes()[1] == 0 && a[i].GetAddressBytes()[2] == 113) ||
                        (a[i].GetAddressBytes()[0] == 198 && a[i].GetAddressBytes()[1] == 51 && a[i].GetAddressBytes()[2] == 100) ||
                        (a[i].GetAddressBytes()[0] == 198 && ((a[i].GetAddressBytes()[1] | 1) & 18) != 0) ||
                        (a[i].GetAddressBytes()[0] == 192 && a[i].GetAddressBytes()[1] == 0 && a[i].GetAddressBytes()[2] == 2) ||
                        (a[i].GetAddressBytes()[0] == 192 && a[i].GetAddressBytes()[1] == 0 && a[i].GetAddressBytes()[2] == 0) ||
                        (a[i].GetAddressBytes()[0] == 198 && a[i].GetAddressBytes()[1] == 51 && a[i].GetAddressBytes()[2] == 100))
                        //No network interface should return this
                        continue;
                    else if (a[i].GetAddressBytes()[0] == 255)
                        //No network interface should return this
                        continue;
                    else if (a[i].GetAddressBytes()[0] == 127)
                        //Local address. We don't want to use it
                        continue;
                    else if ((a[i].GetAddressBytes()[0] == 192 && a[i].GetAddressBytes()[1] == 168) ||
                        a[i].GetAddressBytes()[0] == 169 && a[i].GetAddressBytes()[1] == 254 ||
                        a[i].GetAddressBytes()[0] == 10)
                    {
                        //Local-scope address. While we don't like these, they could be our only choice
                        if (preferred_v4 == null) preferred_v4 = a[i];
                    }
                    else if (preferred_v4 == null)
                        preferred_v4 = a[i];
                }
                else if (a[i].AddressFamily == AddressFamily.InterNetworkV6)
                    if (a[i].IsIPv6LinkLocal)
                        //We dont' like link local addresses
                        continue;
                    else if (preferred_v6 == null) preferred_v6 = a[i];
            }

            //Workaround: we must currently prefer IPv4 over IPv6
            if (preferred_v4 != null) ret = preferred_v4;
            if (preferred_v6 != null && ret == null) ret = preferred_v6;

            if (ret != null) return ret;
            throw new LogbusException("Unable to determine the IP address of current host");
        }

    }
}
