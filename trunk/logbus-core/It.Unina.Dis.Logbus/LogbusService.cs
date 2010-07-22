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
using System.Collections.Generic;
using System.Threading;
using It.Unina.Dis.Logbus.Filters;
using System.Collections;
using It.Unina.Dis.Logbus.Configuration;
using System.Configuration;
using It.Unina.Dis.Logbus.Utils;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.ComponentModel;

namespace It.Unina.Dis.Logbus
{
    public class LogbusService
        : MarshalByRefObject, ILogBus, IChannelManagement, IChannelSubscription
    {
        private Thread hubThread;

        private bool configured = false;



        #region Support fields
        //http://stackoverflow.com/questions/668440/handling-objectdisposedexception-correctly-in-an-idisposable-class-hierarchy
        protected bool Disposed
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns if Logbus is running or not
        /// </summary>
        public bool Running
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set;
        }

        /// <summary>
        /// If true, hub thread is going to stop
        /// </summary>
        private bool HubThreadStop
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        protected BlockingFifoQueue<SyslogMessage> Queue
        {
            get;
            set;
        }
        #endregion

        #region Constructor/destructor

        public LogbusService()
        {
            OutboundChannels = new List<IOutboundChannel>();
            InboundChannels = new List<IInboundChannel>();

            Configuration = ConfigurationHelper.CoreConfiguration;
        }

        public LogbusService(LogbusCoreConfiguration configuration)
            : this()
        {
            Configuration = configuration;
        }

        ~LogbusService()
        {
            Dispose(false);
        }
        #endregion

        #region Configuration

        /// <summary>
        /// Gets or sets configuration for Logbus
        /// </summary>
        public LogbusCoreConfiguration Configuration
        {
            get;
            set;
        }

        /// <summary>
        /// Automatically configures Logbus using the App.Config or Web.config's XML configuration
        /// </summary>
        /// <exception cref="InvalidOperationException">Logbus was already configured</exception>
        /// <exception cref="NotSupportedException">Empty configuration</exception>
        /// <exception cref="LogbusConfigurationException">Semantic error in configuration</exception>
        public void Configure()
        {
            if (configured) throw new InvalidOperationException("Logbus is already configured. If you want to re-configure the service, you need a new instance of the service");

            if (Configuration == null) throw new NotSupportedException("Cannot configure Logbus as configuration is empty");

            //Core filter: if not specified then it's always true
            if (Configuration.corefilter == null) MainFilter = new TrueFilter();
            else MainFilter = Configuration.corefilter;

            try
            {
                //Channel factory
                if (string.IsNullOrEmpty(Configuration.outChannelFactoryType))
                {
                    //Default factory
                    ChannelFactory = new OutChannels.SimpleOutChannelFactory();
                }
                else
                {
                    try
                    {
                        //Try to instantiate
                        Type factory_type = Type.GetType(Configuration.outChannelFactoryType, true, false);

                        if (!factory_type.IsAssignableFrom(typeof(IOutboundChannelFactory)))
                        {
                            LogbusConfigurationException ex = new LogbusConfigurationException("Given Outbound Channel Factory does not implement IOutboundChannelFactory");
                            ex.Data.Add("typeName", Configuration.outChannelFactoryType);
                            throw ex;
                        }

                        ChannelFactory = Activator.CreateInstance(factory_type) as IOutboundChannelFactory;
                    }
                    catch (TypeLoadException ex)
                    {
                        LogbusConfigurationException e = new LogbusConfigurationException("Cannot load type for Outbound Channel Factory", ex);
                        e.Data.Add("typeName", Configuration.outChannelFactoryType);
                        throw e;
                    }
                }

                //Inbound channels
                IList<IInboundChannel> channels = new List<IInboundChannel>();
                if (Configuration.inchannels != null)
                    foreach (InboundChannelDefinition def in Configuration.inchannels)
                    {
                        if (def == null)
                        {
                            LogbusConfigurationException ex = new LogbusConfigurationException("Found empty value in Inbound Channel definition");
                            throw ex;
                        }
                        try
                        {
                            if (string.IsNullOrEmpty(def.type))
                            {
                                LogbusConfigurationException ex = new LogbusConfigurationException("Type for Inbound channel cannot be empty");
                                ex.Data["ChannelDefinitionObject"] = def;
                                throw ex;
                            }

                            try
                            {
                                string typename = def.type;
                                if (typename.IndexOf('.') < 0)
                                {
                                    //This is probably a plain class name, overriding to It.Unina.Dis.Logbus.InChannels namespace
                                    string namespc = "It.Unina.Dis.Logbus.InChannels";
                                    string assemblyname = GetType().Assembly.GetName().ToString();
                                    typename = string.Format("{0}.{1}, {2}", namespc, typename, assemblyname);
                                }

                                Type in_chan_type = Type.GetType(typename, true, false);

                                if (!typeof(IInboundChannel).IsAssignableFrom(in_chan_type))
                                {
                                    LogbusConfigurationException ex = new LogbusConfigurationException("Specified type for Inbound channel does not implement IInboundChannel");
                                    ex.Data["TypeName"] = typename;
                                }
                                IInboundChannel channel = (IInboundChannel)Activator.CreateInstance(in_chan_type, true);
                                try
                                {
                                    if (def.param != null)
                                        foreach (KeyValuePair param in def.param)
                                        {
                                            channel.SetConfigurationParameter(param.name, param.value);
                                        }
                                }
                                catch (NullReferenceException ex)
                                {
                                    throw new LogbusConfigurationException("Inbound channel instance must expose a non-null Configuration array", ex);
                                }
                                catch (NotSupportedException ex)
                                {
                                    throw new LogbusConfigurationException("Configuration parameter is not supported by channel", ex);
                                }
                                catch (Exception ex)
                                {
                                    throw new LogbusConfigurationException("Error configuring inbound channel", ex);
                                }

                                channels.Add(channel);
                            }
                            catch (LogbusConfigurationException ex)
                            {
                                ex.Data["TypeName"] = def.type;
                                throw ex;
                            }
                            catch (TypeLoadException ex)
                            {
                                LogbusConfigurationException e = new LogbusConfigurationException("Type not found for Inbound channel", ex);
                                e.Data["TypeName"] = def.type;
                                throw e;
                            }
                            catch (Exception ex)
                            {
                                LogbusConfigurationException e = new LogbusConfigurationException("Cannot load specified type for Inbound channel", ex);
                                e.Data["TypeName"] = def.type;
                                throw e;
                            }
                        }
                        catch (LogbusConfigurationException ex)
                        {
                            if (!string.IsNullOrEmpty(def.name)) ex.Data["ChannelName"] = def.name;
                            throw ex;
                        }

                    }
                InboundChannels = channels;
                //Inbound channels end


                //Outbound transports begin
                if (Configuration.outtransports != null)
                {
                    //Set factory
                    if (!string.IsNullOrEmpty(Configuration.outtransports.factory))
                    {
                        try
                        {
                            Type factory_type = Type.GetType(Configuration.outtransports.factory);
                            if (!typeof(ITransportFactoryHelper).IsAssignableFrom(factory_type))
                                throw new LogbusConfigurationException("Custom transport factory must implement ITransportFactoryHelper");

                            TransportFactoryHelper = (ITransportFactoryHelper)Activator.CreateInstance(factory_type);
                        }
                        catch (LogbusConfigurationException) { throw; }
                        catch (TypeLoadException ex)
                        {
                            throw new LogbusConfigurationException("Custom transport factory type cannot be loaded", ex);
                        }
                        catch (Exception ex)
                        {
                            throw new LogbusConfigurationException("Cannot instantiate custom transport factory", ex);
                        }
                    }


                    //Add more custom transports
                    if (Configuration.outtransports.outtransport != null || Configuration.outtransports.scanassembly != null)
                        //Not yet implemented, not yet possible!
                        throw new NotImplementedException();
                }

                //If not previously added, add default now
                //For now, no other class is expected
                if (TransportFactoryHelper == null)
                    TransportFactoryHelper = new OutTransports.SimpleTransportHelper();


                //Tell that to the channel factory
                ChannelFactory.TransportFactoryHelper = TransportFactoryHelper;

                //Custom filters definition
                if (Configuration.customfilters != null)
                    throw new NotImplementedException();
            }
            catch (LogbusConfigurationException ex)
            {
                ex.ConfigurationObject = Configuration;
                throw;
            }
            catch (Exception e)
            {
                LogbusConfigurationException ex = new LogbusConfigurationException("Unable to configure Logbus, See inner exception for details", e);
                ex.ConfigurationObject = Configuration;

                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));

                throw ex;
            }
        }

        public void Configure(LogbusCoreConfiguration config)
        {
            Configuration = config;
            Configure();
        }
        #endregion

        #region ILogBus Membri di

        public virtual string[] GetAvailableTransports()
        {
            return TransportFactoryHelper.AvailableTransports;
        }

        public virtual IList<IOutboundChannel> OutboundChannels
        {
            get;
            protected set;
        }

        public virtual IList<IInboundChannel> InboundChannels
        {
            get;
            protected set;
        }

        public virtual IFilter MainFilter
        {
            get;
            set;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (Running) throw new InvalidOperationException("Logbus is already started");
            if (!configured)
            {
                try
                {
                    Configure();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Logbus is not yet configured", ex);
                }
            }

            if (Starting != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                Starting(this, e);
                if (e.Cancel) return;
            }

            //First of all, init fresh queue
            Queue = new BlockingFifoQueue<SyslogMessage>();

            try
            {
                IAsyncResult[] async_in = new IAsyncResult[InboundChannels.Count], async_out = new IAsyncResult[OutboundChannels.Count];
                int i;

                //Begin async start outbound channels
                i = 0;
                foreach (IRunnable chan in OutboundChannels)
                {
                    if (chan is IAsyncRunnable) async_out[i] = ((IAsyncRunnable)chan).BeginStart();
                    i++;
                }


                //Start main hub thread
                HubThreadStop = false;
                hubThread = new Thread(this.HubThreadLoop);
                hubThread.Name = "LogbusService.HubThreadLoop";
                hubThread.Priority = ThreadPriority.Normal;
                hubThread.IsBackground = true;
                hubThread.Start();

                //End async start/sync start outbound channels
                i = 0;
                foreach (IRunnable chan in OutboundChannels)
                {
                    if (chan is IAsyncRunnable) ((IAsyncRunnable)chan).EndStart(async_out[i]);
                    else chan.Start();
                    i++;
                }

                //Begin start async inbound channels and read messages
                i = 0;
                foreach (IInboundChannel chan in InboundChannels)
                {
                    chan.MessageReceived += channel_MessageReceived;
                    if (chan is IAsyncRunnable) async_in[i] = ((IAsyncRunnable)chan).BeginStart();
                    i++;
                }

                //End async start/sync start inbound channels
                i = 0;
                foreach (IRunnable chan in InboundChannels)
                {
                    if (chan is IAsyncRunnable) ((IAsyncRunnable)chan).EndStart(async_in[i]);
                    else chan.Start();
                    i++;
                }
            }
            catch (Exception ex)
            {
                LogbusException e = new LogbusException("Cannot start Logbus", ex);
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(e, true));
                throw e;

            }

            if (Started != null) Started(this, EventArgs.Empty);
            Running = true;
        }

        /// <remarks>Stop is synchronous</remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (!Running) throw new InvalidOperationException("Logbus is not started");

            if (Stopping != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                Stopping(this, e);
                if (e.Cancel) return;
            }

            try
            {
                IAsyncResult[] async_in = new IAsyncResult[InboundChannels.Count], async_out = new IAsyncResult[OutboundChannels.Count];
                int i;
                //Reverse-order stop

                //Begin async stop inbound channels
                i = 0;
                foreach (IInboundChannel chan in InboundChannels)
                {
                    chan.MessageReceived -= this.MessageReceived;
                    if (chan is IAsyncRunnable) async_in[i] = ((IAsyncRunnable)chan).BeginStop();
                    i++;
                }

                //Tell the thread to stop, the good way
                HubThreadStop = true;

                //Begin async stop out
                i = 0;
                foreach (IRunnable chan in OutboundChannels)
                {
                    if (chan is IAsyncRunnable) async_out[i] = ((IAsyncRunnable)chan).BeginStop();
                    i++;
                }

                //End async stop Stop inbound channels so we won't get new messages
                i = 0;
                foreach (IRunnable chan in InboundChannels)
                {
                    if (chan is IAsyncRunnable) ((IAsyncRunnable)chan).EndStop(async_in[i]);
                    else chan.Stop();
                    i++;
                }

                //Stop hub and let it flush messages
                if (hubThread.ThreadState != ThreadState.WaitSleepJoin)
                    hubThread.Join(5000); //Wait if thread is still doing something
                if (hubThread.ThreadState != ThreadState.Stopped)
                {
                    //Thread was locked. Going by brute force!!!
                    try
                    {
                        Thread.BeginCriticalRegion();
                        hubThread.Abort();
                    }
                    finally
                    {
                        Thread.EndCriticalRegion();
                    }
                    hubThread.Join(); //Giving it all the time it needs
                }

                //End async stp/sync stop out channels
                i = 0;
                foreach (IRunnable chan in OutboundChannels)
                {
                    if (chan is IAsyncRunnable) ((IAsyncRunnable)chan).EndStop(async_out[i]);
                    i++;
                }


                Running = false;
            }
            catch (Exception ex)
            {
                LogbusException e = new LogbusException("Could not stop Logbus. Hub is in an inconsistent state!", ex);

                if (Error != null) Error(this, new UnhandledExceptionEventArgs(e, true));

                throw e;
            }


            if (Stopped != null) Stopped(this, EventArgs.Empty);
        }


        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event SyslogMessageEventHandler MessageReceived;

        public event UnhandledExceptionEventHandler Error;

        public string[] AvailableTransports
        {
            get { return TransportFactoryHelper.AvailableTransports; }
        }

        public ITransportFactoryHelper TransportFactoryHelper
        {
            get;
            set;
        }

        public void SubmitMessage(SyslogMessage msg)
        {
            Queue.Enqueue(msg);
        }

        public void CreateChannel(string id, string name, IFilter filter, string description, long coalescenceWindow)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (id.Contains(":")) throw new ArgumentException("Cannot use colon (':') in channel ID");

            //First find if there is one with same ID
            foreach (IOutboundChannel chan in OutboundChannels)
                if (chan.ID == id)
                {
                    LogbusException ex = new LogbusException("Channel already exists");
                    ex.Data.Add("channelId", id);
                    throw ex;
                }

            IOutboundChannel new_chan = ChannelFactory.CreateChannel(name, description, filter);
            new_chan.CoalescenceWindowMillis = (ulong)coalescenceWindow;
            new_chan.ID = id;

            OutboundChannels.Add(new_chan);
            if (Running) new_chan.Start();
        }

        public void RemoveChannel(string id)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (id.Contains(":")) throw new ArgumentException("Invalid channel ID");

            IOutboundChannel to_remove = null;
            //Find the channel
            foreach (IOutboundChannel chan in OutboundChannels)
                if (chan.ID == id) { to_remove = chan; break; }

            if (to_remove == null)
            {
                LogbusException ex = new LogbusException("Channel does not exist");
                ex.Data.Add("channelId", id);
                throw ex;
            }

            OutboundChannels.Remove(to_remove);
            if (Running) to_remove.Stop();
            to_remove.Dispose();
        }

        public string SubscribeClient(string channelId, string transportId, IEnumerable<KeyValuePair<string, string>> transportInstructions, out IEnumerable<KeyValuePair<string, string>> clientInstructions)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(channelId)) throw new ArgumentNullException("channelId", "Channel ID cannot be null");
            if (channelId.Contains(":")) throw new ArgumentException("Invalid channel ID");

            //First find the channel
            IOutboundChannel channel = null;
            foreach (IOutboundChannel chan in OutboundChannels)
                if (chan.ID == channelId) { channel = chan; break; }

            if (channel == null)
            {
                LogbusException ex = new LogbusException("Channel does not exist");
                ex.Data.Add("channelId", channelId);
                throw ex;
            }

            try
            {
                return string.Format("{0}:{1}", channelId, channel.SubscribeClient(transportId, transportInstructions, out clientInstructions));
            }
            catch (LogbusException ex)
            {
                ex.Data.Add("channelId", channelId);
                throw;
            }
            catch (Exception e)
            {
                LogbusException ex = new LogbusException("Could not subscribe channel", e);
                ex.Data.Add("channelId", channelId);
                throw ex;
            }
        }

        public void RefreshClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("Client ID must not be null");
            int indexof = clientId.IndexOf(':');
            if (indexof < 0)
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-Logbus", clientId);
                throw ex;
            }

            string chan_name = clientId.Substring(0, indexof), chan_client_id = clientId.Substring(indexof + 1);
            if (string.IsNullOrEmpty(chan_name))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-Logbus", clientId);
                throw ex;
            }

            //First find the channel
            IOutboundChannel channel = null;
            foreach (IOutboundChannel chan in OutboundChannels)
                if (chan.ID == chan_name) { channel = chan; break; }

            if (channel == null)
            {
                LogbusException ex = new LogbusException("Channel does not exist");
                ex.Data.Add("client-Logbus", clientId);
                throw ex;
            }

            try
            {
                channel.RefreshClient(chan_client_id);
            }
            catch (NotSupportedException ex)
            {
                ex.Data.Add("client-Logbus", clientId);
                throw;
            }
            catch (LogbusException ex)
            {
                ex.Data.Add("client-Logbus", clientId);
                throw;
            }
            catch (Exception e)
            {
                LogbusException ex = new LogbusException("Unable to refresh client", e);
                ex.Data.Add("client-Logbus", clientId);
                throw;
            }
        }

        public void UnsubscribeClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("Client ID must not be null");
            int indexof = clientId.IndexOf(':');
            if (indexof < 0)
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-Logbus", clientId);
                throw ex;
            }

            string chan_name = clientId.Substring(0, indexof), chan_client_id = clientId.Substring(indexof + 1);
            if (string.IsNullOrEmpty(chan_name))
            {
                ArgumentException ex = new ArgumentException("Invalid client ID");
                ex.Data.Add("clientId-Logbus", clientId);
                throw ex;
            }

            //First find the channel
            IOutboundChannel channel = null;
            foreach (IOutboundChannel chan in OutboundChannels)
                if (chan.ID == chan_name) { channel = chan; break; }

            if (channel == null)
            {
                LogbusException ex = new LogbusException("Channel does not exist");
                ex.Data.Add("client-Logbus", clientId);
                throw ex;
            }

            try
            {
                channel.UnsubscribeClient(chan_client_id);
            }
            catch (LogbusException ex)
            {
                ex.Data.Add("client-Logbus", clientId);
                throw;
            }
            catch (Exception e)
            {
                LogbusException ex = new LogbusException("Unable to refresh client", e);
                ex.Data.Add("client-Logbus", clientId);
                throw;
            }
        }
        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            try
            {
                Stop();
            }
            catch { } //Don't propagate, ever
            finally
            {
                Disposed = true;
            }
        }
        #endregion

        #region Channel support
        protected IOutboundChannelFactory ChannelFactory
        {
            get;
            set;
        }

        private void channel_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            Queue.Enqueue(e.Message);
        }
        #endregion


        private void HubThreadLoop()
        {
            //Loop until end
            try
            {
                do
                {
                    //Get message
                    SyslogMessage new_message = Queue.Dequeue();

                    try
                    {
                        Thread.BeginCriticalRegion();
                        //Filter message
                        if (!MainFilter.IsMatch(new_message)) continue;

                        //Deliver to event listeners (SYNCHRONOUS: THREAD-BLOCKING!!!!!!!!!!!!!)
                        if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(new_message));

                        //Deliver to channels
                        //Theorically, it's as faster as channels can do
                        if (OutboundChannels != null)
                            foreach (IOutboundChannel chan in OutboundChannels)
                            {
                                //Idea for the future: use Thread Pool to asynchronously deliver messages
                                //Could lead to a threading disaster in case of large rates of messages
                                chan.SubmitMessage(new_message);
                            }
                    }
                    finally
                    {
                        Thread.EndCriticalRegion();
                    }
                } while (!HubThreadStop);
            }
            catch (ThreadAbortException) { }
            finally
            {
                //Someone is telling me to stop

                //Flush queue and then stop
                IEnumerable<SyslogMessage> left_messages = Queue.FlushAndDispose();
                foreach (SyslogMessage msg in left_messages)
                {
                    //Deliver to event listeners (SYNCHRONOUS: THREAD-BLOCKING!!!!!!!!!!!!!)
                    if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(msg));

                    //Deliver to channels
                    //Theorically, it's as faster as channels can do
                    foreach (IOutboundChannel chan in OutboundChannels)
                    {
                        //Idea for the future: use Thread Pool to asynchronously deliver messages
                        //Could lead to a threading disaster in case of large rates of messages
                        chan.SubmitMessage(msg);
                    }
                }

                Queue = null;
            }

        }

        #region IChannelManagement Membri di

        [MethodImpl(MethodImplOptions.Synchronized)]
        string[] IChannelManagement.ListChannels()
        {
            string[] ret = new string[OutboundChannels.Count];
            int i = 0;
            foreach (IOutboundChannel chan in OutboundChannels)
            {
                ret[i] = chan.ID;
                i++;
            }
            return ret;
        }

        void IChannelManagement.CreateChannel(It.Unina.Dis.Logbus.RemoteLogbus.ChannelCreationInformation description)
        {
            CreateChannel(description.id, description.title, description.filter, description.description, description.coalescenceWindow);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        It.Unina.Dis.Logbus.RemoteLogbus.ChannelInformation IChannelManagement.GetChannelInformation(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            IOutboundChannel chan = null;

            foreach (IOutboundChannel ch in OutboundChannels)
                if (ch.ID == id)
                {
                    chan = ch;
                    break;
                }

            if (chan == null) return null; //Really?

            return new It.Unina.Dis.Logbus.RemoteLogbus.ChannelInformation()
            {
                clients = chan.SubscribedClients.ToString(),
                coalescenceWindow = (long)chan.CoalescenceWindowMillis,
                description = chan.Description,
                filter = chan.Filter as FilterBase,
                id = chan.ID,
                title = chan.Name
            };
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        void IChannelManagement.DeleteChannel(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            IOutboundChannel chan = null;

            foreach (IOutboundChannel ch in OutboundChannels)
                if (ch.ID == id)
                {
                    chan = ch;
                    break;
                }

            if (chan == null) throw new LogbusException("Channel not found");

            if (chan.SubscribedClients > 0) throw new InvalidOperationException("Unable to delete channels to which there are still subscribed clients");
            chan.Stop();
            OutboundChannels.Remove(chan);
        }

        #endregion

        #region IChannelSubscription Membri di

        string[] IChannelSubscription.ListChannels()
        {
            return (this as IChannelManagement).ListChannels();
        }

        string[] IChannelSubscription.GetAvailableTransports()
        {
            return AvailableTransports;
        }

        It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionResponse IChannelSubscription.SubscribeChannel(It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionRequest request)
        {
            IEnumerable<KeyValuePair<string, string>> out_params;
            Dictionary<string, string> in_params = new Dictionary<string, string>();
            foreach (It.Unina.Dis.Logbus.RemoteLogbus.KeyValuePair kvp in request.param)
                in_params.Add(kvp.name, kvp.value);
            string clientid;
            try
            {
                clientid = SubscribeClient(request.channelid, request.transport, in_params, out out_params);
            }
            catch
            {
                throw;
            }

            It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionResponse ret = new It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionResponse();
            ret.clientid = clientid;

            List<It.Unina.Dis.Logbus.RemoteLogbus.KeyValuePair> lst = new List<It.Unina.Dis.Logbus.RemoteLogbus.KeyValuePair>();
            foreach (KeyValuePair<string, string> kvp in out_params)
                lst.Add(new It.Unina.Dis.Logbus.RemoteLogbus.KeyValuePair() { name = kvp.Key, value = kvp.Value });
            ret.param = lst.ToArray();

            return ret;
        }

        void IChannelSubscription.UnsubscribeChannel(string id)
        {
            try
            {
                UnsubscribeClient(id);
            }
            catch
            {
                throw;
            }
        }

        void IChannelSubscription.RefreshSubscription(string id)
        {
            try
            {
                RefreshClient(id);
            }
            catch
            {
                throw;
            }
        }

        #endregion
    }
}
