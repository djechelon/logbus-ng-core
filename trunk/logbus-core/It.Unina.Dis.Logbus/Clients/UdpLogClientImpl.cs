﻿/*
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

namespace It.Unina.Dis.Logbus.Clients
{
    internal sealed class UdpLogClientImpl
        : ILogClient
    {

        private Timer refresh_timer;
        private SyslogUdpReceiver Receiver { get; set; }
        private String Id { get; set; }
        private long ChannelTTL = 0;
        private const long MAX_REFRESH_TIME = 20000;
        private FilterBase filter;
        private bool ExclusiveUsage { get; set; }

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
                Id = string.Format("{0}{1}", Thread.CurrentThread.GetHashCode(), Randomizer.RandomAlphanumericString(5));
            } while (channel_ids.Contains(Id));

            Init();
        }

        public UdpLogClientImpl(string channel_id, IChannelSubscription subscription)
        {
            ExclusiveUsage = false;

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
                    id = Id
                };

                try
                {
                    ChannelManager.CreateChannel(info);
                }
                catch (Exception ex)
                {
                    throw new LogbusException("Unable to create a new channel", ex);
                }

                Receiver = new SyslogUdpReceiver()
                {
                    IpAddress = null,
                    Name = "UdpListner",
                    Port = getAvailablePort()
                };
                Receiver.MessageReceived += MessageReceived;
            }
        }

        private int getAvailablePort()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            return ep.Port;
        }

        private String getIPAddress()
        {
            System.Net.IPAddress[] a = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    if ((a[i].ToString().Contains("localhost")) || (a[i].ToString().Contains("127.0")))
                        continue;
                    else
                        return a[i].ToString();
                }
                else if (a[i].AddressFamily == AddressFamily.InterNetworkV6 && !a[i].IsIPv6LinkLocal && !a[i].IsIPv6SiteLocal) return a[i].ToString();
            }
            throw new LogbusException("Unable to determine the IP address of current host");
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
                ChannelSubscriber.RefreshSubscription(Id);
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
            try
            {
                if (Starting != null)
                {
                    CancelEventArgs arg = new CancelEventArgs();
                    Starting(this, arg);
                    if (arg.Cancel)
                        return;
                }
                ChannelSubscriptionRequest req = new ChannelSubscriptionRequest()
                {
                    channelid = Id,
                    transport = "udp",
                    param = new KeyValuePair[2] { new KeyValuePair() { name = "port", value = Receiver.Port.ToString() }, new KeyValuePair() { name = "ip", value = getIPAddress() } }
                };
                ChannelSubscriptionResponse res = ChannelSubscriber.SubscribeChannel(req);
                ChannelTTL = Int32.Parse(res.param[0].value);
                long refreshTime = ChannelTTL - (ChannelTTL * 20 / 100);
                refresh_timer = new Timer(RefreshChannel, null, Timeout.Infinite, (refreshTime < MAX_REFRESH_TIME) ? refreshTime : MAX_REFRESH_TIME);
                Receiver.Start();

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
            try
            {
                if (Stopping != null)
                {
                    CancelEventArgs arg = new CancelEventArgs();
                    Stopping(this, arg);
                    if (arg.Cancel)
                        return;
                }
                ChannelSubscriber.UnsubscribeChannel(Id);
                ChannelTTL = 0;
                if (refresh_timer != null)
                    refresh_timer.Dispose();
                Receiver.Stop();
                if (Stopped != null)
                    Stopped(this, EventArgs.Empty);
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
            ChannelManager.DeleteChannel(Id);
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
                Receiver.Dispose();
                Receiver = null;
            }
            Disposed = true;
        }

        #endregion
    }
}