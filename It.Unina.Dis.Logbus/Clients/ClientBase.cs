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
using System.Collections;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Loggers;
using It.Unina.Dis.Logbus.RemoteLogbus;
using It.Unina.Dis.Logbus.Utils;

namespace It.Unina.Dis.Logbus.Clients
{
    /// <summary>
    /// Base class for Logbus-ng clients
    /// </summary>
    public abstract class ClientBase
        : ILogClient, ILogSupport
    {
        private readonly bool _exclusiveUsage;
        private readonly FilterBase _filter;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of SyslogUdpClient for running on an exclusive channel
        /// </summary>
        /// <param name="filter">Filter for exclusive channel</param>
        /// <param name="manager">Reference to Channel Manager</param>
        /// <param name="subscription">Reference to Channel Subscriber</param>
        /// <exception cref="LogbusException">Thrown when an error prevents to create a new channel</exception>
        protected ClientBase(FilterBase filter, IChannelManagement manager, IChannelSubscription subscription)
        {
            _exclusiveUsage = true;

            ChannelManager = manager;
            ChannelSubscriber = subscription;
            _filter = filter;

            string[] chIdsString = manager.ListChannels();
            ArrayList channelIds = new ArrayList(chIdsString ?? new string[0]);

            do
            {
                ChannelId = string.Format("{0}{1}", Thread.CurrentThread.GetHashCode(),
                                          Randomizer.RandomAlphanumericString(5));
            } while (channelIds.Contains(ChannelId));

            Init();
        }

        /// <summary>
        /// Initializes a new instance of SyslogUdpClient for running on a shared channel
        /// </summary>
        /// <param name="channelId">ID of channel to subscribe</param>
        /// <param name="subscription">Reference to Channel Subscriber</param>
        protected ClientBase(string channelId, IChannelSubscription subscription)
        {
            _exclusiveUsage = false;
            ChannelId = channelId;
            ChannelSubscriber = subscription;

            Init();
        }

        /// <summary>
        /// Destroys ClientBase
        /// </summary>
        ~ClientBase()
        {
            Dispose(false);
        }

        /// <remarks/>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed) return;

            GC.SuppressFinalize(this);

            try
            {
                if (Running)
                    Stop();
            }
            catch (Exception ex)
            {
                try
                {
                    Log.Warning("Failed to clean stop Logbus client {0}", GetHashCode());
                    Log.Debug("Error details: {0}", ex.Message);
                }
                catch { }
            }

            if (_exclusiveUsage)
                try
                {
                    DestroyChannel();
                }
                catch (Exception ex)
                {
                    Log.Warning("Unable to dispose of channel {0}", ChannelId);
                    Log.Debug("Error details: {0}", ex.Message);
                }

            Disposed = true;
        }

        #endregion

        /// <summary>
        /// Whether the object has been disposed of or not
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <summary>
        /// ID of channel this object is subscribing to
        /// </summary>
        protected string ChannelId { get; private set; }

        /// <summary>
        /// Selects the best IP address to use for listening
        /// </summary>
        /// <returns></returns>
        protected IPAddress GetIpAddress()
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
                    string hostname =
                        Regex.Match(cs.Url,
                                    "^(?<protocol>https?)://(?<host>[-A-Z0-9.]+)(?<port>:[0-9]{1,5})?(?<file>/[-A-Z0-9+&@#/%=~_|!:,.;]*)?(?<parameters>\\?[-A-Z0-9+&@#/%=~_|!:,.;]*)?",
                                    RegexOptions.IgnoreCase).Groups["host"].Value;
                    IPAddress hostIp = Dns.GetHostAddresses(hostname)[0];

                    //If we are contacting a local host, tell to use loopback
                    if (hostIp.Equals(IPAddress.Loopback)) return IPAddress.Loopback;

                    //Just force a routing table lookup, we don't need more
                    UdpClient fakeClient = new UdpClient();
                    fakeClient.Connect(hostname, 65534);
                    return ((IPEndPoint)fakeClient.Client.LocalEndPoint).Address;
                }
                catch
                {
                } //Never mind...
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

        private void Init()
        {
            Log = LoggerHelper.GetLogger(WellKnownLogger.Client);

            if (_exclusiveUsage)
            {
                //Create channel
                ChannelCreationInformation info = new ChannelCreationInformation
                                                      {
                                                          coalescenceWindow = 0,
                                                          description = "Channel created by LogCollector",
                                                          filter = _filter,
                                                          title = "AutoChannel",
                                                          id = ChannelId
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

        private void DestroyChannel()
        {
            ChannelManager.DeleteChannel(ChannelId);
        }

        #region Public properties

        /// <summary>
        /// Gets or set the Channel Management proxy
        /// </summary>
        public IChannelManagement ChannelManager { get; set; }

        /// <summary>
        /// Gets or set the Channel Subscription proxy
        /// </summary>
        public IChannelSubscription ChannelSubscriber { get; set; }

        #endregion

        #region Firing events

        /// <summary>
        /// Fires the Starting event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnStarting(CancelEventArgs e)
        {
            if (Starting != null)
                Starting(this, e);
        }

        /// <summary>
        /// Fires the Started event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnStarted(EventArgs e)
        {
            if (Started != null)
                Started(this, e);
        }

        /// <summary>
        /// Fires the Stopping event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnStopping(CancelEventArgs e)
        {
            if (Stopping != null)
                Stopping(this, e);
        }

        /// <summary>
        /// Fires the Stopped event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnStopped(EventArgs e)
        {
            if (Stopped != null)
                Stopped(this, e);
        }

        /// <summary>
        /// Fires the MessageReceived event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnMessageReceived(SyslogMessageEventArgs e)
        {
            try
            {
                if (MessageReceived != null)
                    MessageReceived(this, e);
            }
            catch { }

        }

        /// <summary>
        /// Fires the Error event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected virtual void OnError(UnhandledExceptionEventArgs e)
        {
            if (Error != null)
                Error(this, e);
        }

        #endregion

        #region IRunnable Membri di

        /// <summary>
        /// Implements IRunnable.Running
        /// </summary>
        public virtual bool Running { get; protected set; }

        /// <summary>
        /// Implements IRunnable.Starting
        /// </summary>
        public event EventHandler<CancelEventArgs> Starting;

        /// <summary>
        /// Implements IRunnable.Stopping
        /// </summary>
        public event EventHandler<CancelEventArgs> Stopping;

        /// <summary>
        /// Implements IRunnable.Started
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Implements IRunnable.Stopped
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Implements IRunnable.Error
        /// </summary>
        public event UnhandledExceptionEventHandler Error;

        /// <summary>
        /// Implements IRunnable.Start
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Implements IRunnable.Stop
        /// </summary>
        public abstract void Stop();

        #endregion

        #region ILogSource Membri di

        /// <summary>
        /// Implements ILogSource.MessageReceived
        /// </summary>
        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        #endregion

        #region IDisposable Membri di

        /// <summary>
        /// Implements IDisposable.Dispose
        /// </summary>
        public void Dispose()
        {
            if (Disposed) return;

            Dispose(true);
        }

        #endregion

        #region ILogSupport Membri di

        /// <summary>
        /// Implements ILogSupport.Log
        /// </summary>
        public ILog Log { protected get; set; }

        #endregion
    }
}