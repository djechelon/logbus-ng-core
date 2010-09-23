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
using It.Unina.Dis.Logbus.Collectors;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Utils;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using It.Unina.Dis.Logbus.Loggers;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Default implementation of ILogBus
    /// </summary>
    internal class LogbusService
        : MarshalByRefObject, ILogBus, IChannelManagement, IChannelSubscription
    {
        private Thread[] hubThreads;
        private Thread forwarding_thread;
        private const int WORKER_THREADS = 4;
        private bool configured = false, forwarding_enabled = false;

        private List<IInboundChannel> in_chans;
        private List<IOutboundChannel> out_chans;
        private IPlugin[] plugins;

        /// <summary>
        /// Returns if Logbus is running or not
        /// </summary>
        private volatile bool running;

        /// <summary>
        /// If true, hub thread is going to stop
        /// </summary>
        private volatile bool hubThreadStop;

        protected BlockingFifoQueue<SyslogMessage>[] queues;
        protected BlockingFifoQueue<SyslogMessage> forwarding_queue;

        protected ILogCollector forwarder;

        //http://stackoverflow.com/questions/668440/handling-objectdisposedexception-correctly-in-an-idisposable-class-hierarchy
        protected bool Disposed
        {
            get;
            private set;
        }

        #region Constructor/destructor

        /// <summary>
        /// Initializes the LogbusService
        /// </summary>
        internal LogbusService()
        {
            LogbusSingletonHelper.Instance = this;

            out_chans = new List<IOutboundChannel>();
            in_chans = new List<IInboundChannel>();
            Log = new SimpleLogImpl(SyslogFacility.Internally, this);

            //Init fresh queues
            queues = new BlockingFifoQueue<SyslogMessage>[WORKER_THREADS];
            for (int i = 0; i < WORKER_THREADS; i++)
                queues[i] = new BlockingFifoQueue<SyslogMessage>();
        }

        /// <summary>
        /// Initializes the LogbusService with a given configuration
        /// </summary>
        /// <param name="configuration"></param>
        internal LogbusService(LogbusCoreConfiguration configuration)
            : this()
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Destructor for LogbusService
        /// </summary>
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
        /// <exception cref="System.InvalidOperationException">Logbus was already configured</exception>
        /// <exception cref="System.NotSupportedException">Empty configuration</exception>
        /// <exception cref="It.Unina.Dis.Logbus.Configuration.LogbusConfigurationException">Semantic error in configuration</exception>
        public void Configure()
        {
            if (configured) throw new InvalidOperationException("Logbus is already configured. If you want to re-configure the service, you need a new instance of the service");
            if (Configuration == null) Configuration = ConfigurationHelper.CoreConfiguration;
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
                        Type factoryType = Type.GetType(Configuration.outChannelFactoryType, true, false);

                        if (!factoryType.IsAssignableFrom(typeof(IOutboundChannelFactory)))
                        {
                            LogbusConfigurationException ex = new LogbusConfigurationException("Given Outbound Channel Factory does not implement IOutboundChannelFactory");
                            ex.Data.Add("typeName", Configuration.outChannelFactoryType);
                            throw ex;
                        }

                        ChannelFactory = Activator.CreateInstance(factoryType) as IOutboundChannelFactory;
                    }
                    catch (TypeLoadException ex)
                    {
                        LogbusConfigurationException e = new LogbusConfigurationException("Cannot load type for Outbound Channel Factory", ex);
                        e.Data.Add("typeName", Configuration.outChannelFactoryType);
                        throw e;
                    }
                }
                if (ChannelFactory is ILogSupport)
                    ((ILogSupport)ChannelFactory).Log = new SimpleLogImpl(SyslogFacility.Internally, this);

                //Inbound channels
                List<IInboundChannel> channels = new List<IInboundChannel>();
                if (Configuration.inchannels != null)
                    foreach (InboundChannelDefinition def in Configuration.inchannels)
                    {
                        if (def == null)
                        {
                            LogbusConfigurationException ex = new LogbusConfigurationException("Found empty value in Inbound Channel definition");
                            throw ex;
                        }

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
                                const string namespc = "It.Unina.Dis.Logbus.InChannels";
                                string assemblyname = GetType().Assembly.GetName().ToString();
                                typename = string.Format("{0}.{1}, {2}", namespc, typename, assemblyname);
                            }

                            Type inChanType = Type.GetType(typename, true, false);

                            if (!typeof(IInboundChannel).IsAssignableFrom(inChanType))
                            {
                                LogbusConfigurationException ex = new LogbusConfigurationException("Specified type for Inbound channel does not implement IInboundChannel");
                                ex.Data["TypeName"] = typename;
                            }
                            IInboundChannel channel = (IInboundChannel)Activator.CreateInstance(inChanType, true);
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

                            if (channel is ILogSupport) ((ILogSupport)channel).Log = new SimpleLogImpl(SyslogFacility.Internally, this);

                            channels.Add(channel);
                        }
                        catch (LogbusConfigurationException ex)
                        {
                            ex.Data["TypeName"] = def.type;
                            throw;
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
                in_chans = channels;
                //Inbound channels end


                //Outbound transports begin
                if (Configuration.outtransports != null)
                {
                    //Set factory
                    if (!string.IsNullOrEmpty(Configuration.outtransports.factory))
                    {
                        try
                        {
                            Type factoryType = Type.GetType(Configuration.outtransports.factory);
                            if (!typeof(ITransportFactoryHelper).IsAssignableFrom(factoryType))
                                throw new LogbusConfigurationException("Custom transport factory must implement ITransportFactoryHelper");

                            TransportFactoryHelper = (ITransportFactoryHelper)Activator.CreateInstance(factoryType);
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
                //Add logger
                if (TransportFactoryHelper is ILogSupport)
                    ((ILogSupport)TransportFactoryHelper).Log = new SimpleLogImpl(
                        SyslogFacility.Internally, this);

                //Tell that to the channel factory
                ChannelFactory.TransportFactoryHelper = TransportFactoryHelper;

                //Custom filters definition
                if (Configuration.customfilters != null)
                    throw new NotImplementedException();

                //Forwarding configuration
                if (Configuration.forwardto != null && Configuration.forwardto.Length > 0)
                {
                    forwarding_enabled = true;

                    List<ILogCollector> collectors = new List<ILogCollector>();
                    foreach (ForwarderDefinition def in Configuration.forwardto)
                    {
                        ILogCollector collector = CollectorHelper.CreateByDefinition(def);
                        if (collector is ILogSupport)
                            ((ILogSupport)collector).Log = new SimpleLogImpl(SyslogFacility.Internally, this);
                        collectors.Add(collector);
                    }
                    forwarder = new MultiCollector() { Collectors = collectors.ToArray() };
                }

                //Plugin configuration
                if (Configuration.plugins != null && Configuration.plugins.Length > 0)
                {
                    List<IPlugin> activePlugins = new List<IPlugin>();
                    foreach (PluginDefinition def in Configuration.plugins)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(def.type))
                                throw new LogbusConfigurationException("Empty type specified for plugin definition");

                            Type pluginType = Type.GetType(def.type, true, false);

                            if (!typeof(IPlugin).IsAssignableFrom(pluginType))
                            {
                                LogbusConfigurationException ex = new LogbusConfigurationException("Requested type is not compatible with IPlugin");
                                ex.Data.Add("type", pluginType);
                                throw ex;
                            }

                            IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);
                            activePlugins.Add(plugin);
                            plugin.Log = new SimpleLogImpl(SyslogFacility.Internally, this);
                            plugin.Register(this);
                        }
                        catch (LogbusConfigurationException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            LogbusConfigurationException e = new LogbusConfigurationException("Unable to configure plugins", ex);
                            e.Data.Add("pluginType", def.type);
                        }
                        plugins = activePlugins.ToArray();
                    }
                }
            }
            catch (LogbusConfigurationException ex)
            {
                ex.ConfigurationObject = Configuration;
                throw;
            }
            catch (Exception e)
            {
                LogbusConfigurationException ex = new LogbusConfigurationException("Unable to configure Logbus, See inner exception for details", e) { ConfigurationObject = Configuration };

                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));

                throw ex;
            }
        }

        /// <summary>
        /// Configures Logbus service with the given configuration object
        /// </summary>
        /// <param name="config">Configuration values</param>
        public void Configure(LogbusCoreConfiguration config)
        {
            Configuration = config;
            Configure();
        }
        #endregion

        #region ILogBus Membri di

        /// <summary>
        /// Implements ILogBus.GetAvailableTransports
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetAvailableTransports()
        {
            return TransportFactoryHelper.AvailableTransports;
        }

        /// <summary>
        /// List of available outbound channels
        /// </summary>
        protected virtual List<IOutboundChannel> OutboundChannels
        {
            get { return out_chans; }
        }

        IList<IOutboundChannel> ILogBus.OutboundChannels
        {
            get
            {
                return out_chans.AsReadOnly();
            }
        }

        /// <summary>
        /// Implements ILogBus.InboundChannels
        /// </summary>
        protected virtual IList<IInboundChannel> InboundChannels
        {
            get { return in_chans; }
        }

        IList<IInboundChannel> ILogBus.InboundChannels
        {
            get
            {
                return in_chans.AsReadOnly();
            }
        }

        /// <summary>
        /// Implements ILogBus.MainFilter
        /// </summary>
        public virtual IFilter MainFilter
        {
            get;
            set;
        }

        /// <summary>
        /// Implements ILogBus.OutChannelCreated
        /// </summary>
        public event EventHandler<It.Unina.Dis.Logbus.OutChannels.OutChannelCreationEventArgs> OutChannelCreated;

        /// <summary>
        /// Implements ILogBus.OutChannelDeleted
        /// </summary>
        public event EventHandler<It.Unina.Dis.Logbus.OutChannels.OutChannelDeletionEventArgs> OutChannelDeleted;

        /// <summary>
        /// Impleemnts ILogBus.Plugins
        /// </summary>
        IEnumerable<IPlugin> ILogBus.Plugins
        {
            get
            {
                if (plugins == null) return new IPlugin[0];
                int plugincount = plugins.Length;

                IPlugin[] ret = new IPlugin[plugincount];
                Array.Copy(plugins, ret, plugincount);
                return ret;
            }
        }

        /// <summary>
        /// Implements IRunnable.Start
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            try
            {
                Log.Info("LogbusService starting");
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                if (running) throw new InvalidOperationException("Logbus is already started");
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

                    hubThreadStop = false;

                    //Start hub threads
                    hubThreads = new Thread[WORKER_THREADS];
                    for (i = 0; i < WORKER_THREADS; i++)
                    {
                        hubThreads[i] = new Thread(this.HubThreadLoop)
                        {
                            Name = string.Format("LogbusService.HubThreadLoop[{0}]", i),
                            Priority = ThreadPriority.Normal,
                            IsBackground = true
                        };
                        hubThreads[i].Start(queues[i]);
                    }

                    if (forwarding_enabled)
                    {
                        forwarding_queue = new BlockingFifoQueue<SyslogMessage>();
                        forwarding_thread = new Thread(ForwardLoop)
                            {
                                IsBackground = true,
                                Priority = ThreadPriority.Normal,
                                Name = "LogbusService.ForwardLoop"
                            };
                        forwarding_thread.Start();
                    }

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
                running = true;
                Log.Info("LogbusService started");
            }
            catch
            {
                Log.Error("LogbusService start failed");
                throw;
            }
        }

        /// <summary>Implements IRunnable.Stop</summary>
        /// <remarks>Stop is synchronous</remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            try
            {
                Log.Info("LogbusService stopping");
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                if (!running) throw new InvalidOperationException("Logbus is not started");

                if (Stopping != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Stopping(this, e);
                    if (e.Cancel) return;
                }

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
                hubThreadStop = true;

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
                for (i = 0; i < WORKER_THREADS; i++)
                {
                    hubThreads[i].Interrupt();
                }
                for (i = 0; i < WORKER_THREADS; i++)
                {
                    hubThreads[i].Join(); //Giving it all the time it needs
                }

                if (forwarding_enabled)
                {
                    forwarding_thread.Interrupt();

                    forwarding_thread.Join(); //Giving it all the time it needs
                }

                //End async stp/sync stop out channels
                i = 0;
                foreach (IRunnable chan in OutboundChannels)
                {
                    if (chan is IAsyncRunnable) ((IAsyncRunnable)chan).EndStop(async_out[i]);
                    i++;
                }


                running = false;

                if (Stopped != null) Stopped(this, EventArgs.Empty);
                Log.Info("LogbusService stopped");
            }
            catch (Exception ex)
            {
                Log.Error("LogbusService stop failed");

                LogbusException e = new LogbusException("Could not stop Logbus", ex);
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(e, true));
                throw e;
            }
        }

        /// <summary>
        /// Implements IRunnable.Starting
        /// </summary>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        /// <summary>
        /// Implements IRunnable.Stopping
        /// </summary>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        /// <summary>
        /// Implements IRunnable.Started
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Implements IRunnable.Starting
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Implements ILogSource.MessageReceived
        /// </summary>
        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        /// <summary>
        /// Implements IRunnable.Error
        /// </summary>
        public event UnhandledExceptionEventHandler Error;

        /// <summary>
        /// Implements ILogBus.AvailableTransports
        /// </summary>
        public string[] AvailableTransports
        {
            get { return TransportFactoryHelper.AvailableTransports; }
        }

        /// <summary>
        /// Implements ILogBus.TransportFactoryHelper
        /// </summary>
        public ITransportFactoryHelper TransportFactoryHelper
        {
            get;
            set;
        }

        /// <summary>
        /// Implements ILogCollector.SubmitMessage
        /// </summary>
        /// <param name="msg"></param>
        public void SubmitMessage(SyslogMessage msg)
        {
            //Pick a random queue. Don't need to use the Random class
            queues[Environment.TickCount % WORKER_THREADS].Enqueue(msg);
        }

        /// <summary>
        /// Implements ILogBus.CreateChannel
        /// </summary>
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
            if (running) new_chan.Start();
            Log.Info(string.Format("New channel created: {0}", id));

            if (OutChannelCreated != null) OutChannelCreated(this, new It.Unina.Dis.Logbus.OutChannels.OutChannelCreationEventArgs(new_chan));
        }

        /// <summary>
        /// Implements ILogBus.DeleteChannel
        /// </summary>
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
                Log.Warning(string.Format("Failed to remove channel {0}", id));
                LogbusException ex = new LogbusException("Channel does not exist");
                ex.Data.Add("channelId", id);
                throw ex;
            }

            OutboundChannels.Remove(to_remove);
            if (running) to_remove.Stop();
            to_remove.Dispose();
            Log.Info(string.Format("Channel removed: {0}", id));

            if (OutChannelDeleted != null) OutChannelDeleted(this, new It.Unina.Dis.Logbus.OutChannels.OutChannelDeletionEventArgs(id));
        }

        /// <summary>
        /// Implements ILogBus.SubscribeClient
        /// </summary>
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

        /// <summary>
        /// Implements ILogBus.RefreshClient
        /// </summary>
        public void RefreshClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId)) throw new ArgumentNullException("clientId", "Client ID must not be null");
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

        /// <summary>
        /// Implements ILogBus.UnsubscribeClient
        /// </summary>
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
                try
                {
                    Stop(); //There could be problems with this
                }
                catch { }

                if (plugins != null)
                    foreach (IPlugin plugin in plugins)
                        if (plugin != null)
                            try
                            {
                                plugin.Unregister();
                                if (disposing) plugin.Dispose();
                            }
                            catch { }
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
            SubmitMessage(e.Message);
        }
        #endregion

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
            if (description == null) throw new ArgumentNullException("description");
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

        string[] IChannelSubscription.GetAvailableFilters()
        {
            return CustomFilterHelper.Instance.GetAvailableCustomFilters();
        }

        It.Unina.Dis.Logbus.RemoteLogbus.FilterDescription IChannelSubscription.DescribeFilter(string filterid)
        {
            return CustomFilterHelper.Instance.DescribeFilter(filterid);
        }
        #endregion

        #region Support

        private void HubThreadLoop(object queue)
        {
            BlockingFifoQueue<SyslogMessage> localQueue = (BlockingFifoQueue<SyslogMessage>)queue;
            //Loop until end
            try
            {
                do
                {
                    //Get message
                    SyslogMessage newMessage = localQueue.Dequeue();

                    try
                    {
                        Thread.BeginCriticalRegion();
                        //Filter message
                        if (!MainFilter.IsMatch(newMessage)) continue;

                        //Deliver to event listeners (SYNCHRONOUS: THREAD-BLOCKING!!!!!!!!!!!!!)
                        if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(newMessage));

                        //Deliver to channels
                        //Theorically, it's as faster as channels can do
                        if (OutboundChannels != null)
                            foreach (IOutboundChannel chan in OutboundChannels)
                            {
                                //Idea for the future: use Thread Pool to asynchronously deliver messages
                                //Could lead to a threading disaster in case of large rates of messages
                                chan.SubmitMessage(newMessage);
                            }

                        if (forwarding_enabled && forwarding_queue != null) forwarding_queue.Enqueue(newMessage);
                    }
                    finally
                    {
                        Thread.EndCriticalRegion();
                    }
                } while (!hubThreadStop);
            }
            catch (ThreadInterruptedException) { }
            finally
            {
                //Someone is telling me to stop

                //Flush queue and then stop
                IEnumerable<SyslogMessage> leftMessages = localQueue.Flush();
                foreach (SyslogMessage msg in leftMessages)
                {
                    //Deliver to event listeners (SYNCHRONOUS: THREAD-BLOCKING!!!!!!!!!!!!!)
                    if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(msg));

                    //Deliver to channels
                    //Theorically, it's as faster as channels can do
                    if (OutboundChannels != null)
                        foreach (IOutboundChannel chan in OutboundChannels)
                        {
                            //Idea for the future: use Thread Pool to asynchronously deliver messages
                            //Could lead to a threading disaster in case of large rates of messages
                            chan.SubmitMessage(msg);
                        }
                }
            }
        }

        private void ForwardLoop()
        {
            try
            {
                do
                {
                    //Get message
                    SyslogMessage newMessage = forwarding_queue.Dequeue();

                    forwarder.SubmitMessage(newMessage);
                } while (!hubThreadStop);
            }
            catch (ThreadInterruptedException) { }
        }

        private ILog Log
        {
            get;
            set;
        }
        #endregion
    }
}
