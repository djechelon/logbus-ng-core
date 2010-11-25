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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using It.Unina.Dis.Logbus.Collectors;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Loggers;
using It.Unina.Dis.Logbus.OutChannels;
using It.Unina.Dis.Logbus.OutTransports;
using It.Unina.Dis.Logbus.RemoteLogbus;
using It.Unina.Dis.Logbus.Utils;
using KeyValuePair = It.Unina.Dis.Logbus.Configuration.KeyValuePair;
using System.Reflection;
using System.Globalization;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Default implementation of ILogBus
    /// </summary>
    internal class LogbusService
        : MarshalByRefObject, ILogBus, IChannelManagement, IChannelSubscription
    {
        private const int DEFAULT_JOIN_TIMEOUT = 5000;

        private Thread[] _hubThreads;
        private Thread _forwardingThread;
        private const int WORKER_THREADS = 4;
        private bool _configured, _forwardingEnabled;
        private readonly Timer _statistics;
        private int _currentQueue, _processedMessages;

        private List<IInboundChannel> _inChans;
        private readonly List<IOutboundChannel> _outChans;
        private IPlugin[] _plugins;

        /// <summary>
        /// Returns if Logbus is running or not
        /// </summary>
        private volatile bool _running;

        /// <summary>
        /// If true, hub thread is going to stop
        /// </summary>
        private volatile bool _hubThreadStop;

        protected IFifoQueue<SyslogMessage>[] Queues;
        protected IFifoQueue<SyslogMessage> ForwardingQueue;
        private readonly ReaderWriterLock _outLock = new ReaderWriterLock(), _inLock = new ReaderWriterLock();
        protected ILogCollector Forwarder;

        //http://stackoverflow.com/questions/668440/handling-objectdisposedexception-correctly-in-an-idisposable-class-hierarchy
        protected bool Disposed { get; private set; }

        #region Constructor/destructor

        /// <summary>
        /// Initializes the LogbusService
        /// </summary>
        public LogbusService()
        {
            LogbusSingletonHelper.Instance = this;

            _outChans = new List<IOutboundChannel>();
            _inChans = new List<IInboundChannel>();
            Log = LoggerHelper.GetLogger(WellKnownLogger.Logbus);

            //Init fresh queues
            _currentQueue = int.MinValue;
            Queues = new IFifoQueue<SyslogMessage>[WORKER_THREADS];
            for (int i = 0; i < WORKER_THREADS; i++)
                Queues[i] = new FastFifoQueue<SyslogMessage>(2048);

            _statistics = new Timer(LogStatistics, null, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));
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

        #region MarshalByRefObject Membri di

        /// <remarks/>
        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion

        #region Configuration

        /// <summary>
        /// Gets or sets configuration for Logbus
        /// </summary>
        public LogbusCoreConfiguration Configuration { get; set; }

        /// <summary>
        /// Automatically configures Logbus using the App.Config or Web.config's XML configuration
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="System.InvalidOperationException">Logbus was already configured</exception>
        /// <exception cref="System.NotSupportedException">Empty configuration</exception>
        /// <exception cref="It.Unina.Dis.Logbus.Configuration.LogbusConfigurationException">Semantic error in configuration</exception>
        public void Configure()
        {
            if (_configured)
                throw new InvalidOperationException(
                    "Logbus is already configured. If you want to re-configure the service, you need a new instance of the service");
            if (Configuration == null) Configuration = ConfigurationHelper.CoreConfiguration;
            if (Configuration == null)
                throw new NotSupportedException("Cannot configure Logbus as configuration is empty");

            //Core filter: if not specified then it's always true
            MainFilter = Configuration.corefilter ?? new TrueFilter();

            try
            {
                //Channel factory
                if (string.IsNullOrEmpty(Configuration.outChannelFactoryType))
                {
                    //Default factory
                    ChannelFactory = new SimpleOutChannelFactory();
                }
                else
                {
                    try
                    {
                        //Try to instantiate
                        Type factoryType = Type.GetType(Configuration.outChannelFactoryType, true, false);

                        if (!factoryType.IsAssignableFrom(typeof(IOutboundChannelFactory)))
                        {
                            LogbusConfigurationException ex =
                                new LogbusConfigurationException(
                                    "Given Outbound Channel Factory does not implement IOutboundChannelFactory");
                            ex.Data.Add("typeName", Configuration.outChannelFactoryType);
                            throw ex;
                        }

                        ChannelFactory = Activator.CreateInstance(factoryType) as IOutboundChannelFactory;
                    }
                    catch (TypeLoadException ex)
                    {
                        LogbusConfigurationException e =
                            new LogbusConfigurationException("Cannot load type for Outbound Channel Factory", ex);
                        e.Data.Add("typeName", Configuration.outChannelFactoryType);
                        throw e;
                    }
                }
                if (ChannelFactory is ILogSupport)
                    ((ILogSupport)ChannelFactory).Log = LoggerHelper.GetLogger(WellKnownLogger.Logbus);

                //Inbound channels
                List<IInboundChannel> channels = new List<IInboundChannel>();
                if (Configuration.inchannels != null)
                    foreach (InboundChannelDefinition def in Configuration.inchannels)
                    {
                        if (def == null)
                        {
                            LogbusConfigurationException ex =
                                new LogbusConfigurationException("Found empty value in Inbound Channel definition");
                            throw ex;
                        }

                        if (string.IsNullOrEmpty(def.type))
                        {
                            LogbusConfigurationException ex =
                                new LogbusConfigurationException("Type for Inbound channel cannot be empty");
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
                                LogbusConfigurationException ex =
                                    new LogbusConfigurationException(
                                        "Specified type for Inbound channel does not implement IInboundChannel");
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
                                throw new LogbusConfigurationException(
                                    "Inbound channel instance must expose a non-null Configuration array", ex);
                            }
                            catch (NotSupportedException ex)
                            {
                                throw new LogbusConfigurationException(
                                    "Configuration parameter is not supported by channel", ex);
                            }
                            catch (Exception ex)
                            {
                                throw new LogbusConfigurationException("Error configuring inbound channel", ex);
                            }

                            if (channel is ILogSupport)
                                ((ILogSupport)channel).Log = LoggerHelper.GetLogger(WellKnownLogger.Logbus);

                            channels.Add(channel);
                        }
                        catch (LogbusConfigurationException ex)
                        {
                            ex.Data["TypeName"] = def.type;
                            throw;
                        }
                        catch (TypeLoadException ex)
                        {
                            LogbusConfigurationException e =
                                new LogbusConfigurationException("Type not found for Inbound channel", ex);
                            e.Data["TypeName"] = def.type;
                            throw e;
                        }
                        catch (Exception ex)
                        {
                            LogbusConfigurationException e =
                                new LogbusConfigurationException("Cannot load specified type for Inbound channel", ex);
                            e.Data["TypeName"] = def.type;
                            throw e;
                        }
                    }
                _inChans = channels;
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
                                throw new LogbusConfigurationException(
                                    "Custom transport factory must implement ITransportFactoryHelper");

                            TransportFactoryHelper = (ITransportFactoryHelper)Activator.CreateInstance(factoryType);
                        }
                        catch (LogbusConfigurationException)
                        {
                            throw;
                        }
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
                    if (Configuration.outtransports.outtransport != null ||
                        Configuration.outtransports.scanassembly != null)
                        //Not yet implemented, not yet possible!
                        throw new NotImplementedException("Not yet supported");
                }

                //If not previously added, add default now
                //For now, no other class is expected
                if (TransportFactoryHelper == null)
                    TransportFactoryHelper = new SimpleTransportHelper();
                //Add logger
                if (TransportFactoryHelper is ILogSupport)
                    ((ILogSupport)TransportFactoryHelper).Log = LoggerHelper.GetLogger(WellKnownLogger.Logbus);

                //Tell that to the channel factory
                ChannelFactory.TransportFactoryHelper = TransportFactoryHelper;


                //Custom filters definition
                if (Configuration.customfilters != null)
                {
                    if (Configuration.customfilters.scanassembly != null)
                        foreach (AssemblyToScan definition in Configuration.customfilters.scanassembly)
                        {
                            if (string.IsNullOrEmpty(definition.assembly))
                                throw new LogbusConfigurationException(
                                    "Empty assembly name: cannot scan for custom filters");

                            Assembly toScan;
                            try
                            {
                                toScan = Assembly.Load(definition.assembly);
                            }
                            catch (Exception ex)
                            {
                                //Try with code base
                                try
                                {
                                    toScan = Assembly.LoadFile(definition.codebase);
                                }
                                catch
                                {
                                    throw new LogbusConfigurationException("Unable to scan assembly {0} for custom filters", ex);
                                }
                            }
                            CustomFilterHelper.Instance.ScanAssemblyAndRegister(toScan);
                        }
                    if (Configuration.customfilters.customfilter != null)
                    {
                        foreach (CustomFilterDefinition definition in Configuration.customfilters.customfilter)
                        {
                            CustomFilterHelper.Instance.RegisterCustomFilter(definition.name, definition.type, definition.description);
                        }
                    }
                }

                //Forwarding configuration
                if (Configuration.forwardto != null && Configuration.forwardto.Length > 0)
                {
                    _forwardingEnabled = true;

                    List<ILogCollector> collectors = new List<ILogCollector>();
                    foreach (ForwarderDefinition def in Configuration.forwardto)
                    {
                        ILogCollector collector = CollectorHelper.CreateCollector(def);
                        if (collector is ILogSupport)
                            ((ILogSupport)collector).Log = LoggerHelper.GetLogger(WellKnownLogger.Logbus);
                        collectors.Add(collector);
                    }
                    Forwarder = new MultiCollector { Collectors = collectors.ToArray() };
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
                                LogbusConfigurationException ex =
                                    new LogbusConfigurationException("Requested type is not compatible with IPlugin");
                                ex.Data.Add("type", pluginType);
                                throw ex;
                            }

                            IPlugin plugin = (IPlugin)Activator.CreateInstance(pluginType);
                            activePlugins.Add(plugin);
                            plugin.Log = LoggerHelper.GetLogger(WellKnownLogger.Logbus);
                            plugin.Register(this);
                        }
                        catch (LogbusConfigurationException)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            LogbusConfigurationException e =
                                new LogbusConfigurationException("Unable to configure plugins", ex);
                            e.Data.Add("pluginType", def.type);
                            throw e;
                        }
                        _plugins = activePlugins.ToArray();
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
                LogbusConfigurationException ex =
                    new LogbusConfigurationException("Unable to configure Logbus, See inner exception for details", e) { ConfigurationObject = Configuration };

                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));

                throw ex;
            }
            _configured = true;
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
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            return TransportFactoryHelper.AvailableTransports;
        }

        /// <summary>
        /// List of available outbound channels
        /// </summary>
        protected virtual List<IOutboundChannel> OutboundChannels
        {
            get
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                return _outChans;
            }
        }

        IList<IOutboundChannel> ILogBus.OutboundChannels
        {
            get
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                return _outChans.AsReadOnly();
            }
        }

        /// <summary>
        /// Implements ILogBus.InboundChannels
        /// </summary>
        protected virtual IList<IInboundChannel> InboundChannels
        {
            get
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                return _inChans;
            }
        }

        IList<IInboundChannel> ILogBus.InboundChannels
        {
            get
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                return _inChans.AsReadOnly();
            }
        }

        /// <summary>
        /// Implements ILogBus.MainFilter
        /// </summary>
        public virtual IFilter MainFilter { get; set; }

        /// <summary>
        /// Implements IRunnable.Running
        /// </summary>
        bool IRunnable.Running
        {
            get { return _running; }
        }

        /// <summary>
        /// Implements ILogBus.OutChannelCreated
        /// </summary>
        public event EventHandler<OutChannelCreationEventArgs> OutChannelCreated;

        /// <summary>
        /// Implements ILogBus.OutChannelDeleted
        /// </summary>
        public event EventHandler<OutChannelDeletionEventArgs> OutChannelDeleted;

        /// <summary>
        /// Implements ILogBus.Plugins
        /// </summary>
        IEnumerable<IPlugin> ILogBus.Plugins
        {
            get
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                if (_plugins == null) return new IPlugin[0];
                int plugincount = _plugins.Length;

                IPlugin[] ret = new IPlugin[plugincount];
                Array.Copy(_plugins, ret, plugincount);
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
                if (_running) throw new InvalidOperationException("Logbus is already started");
                if (!_configured)
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
                    IAsyncResult[] asyncIn = new IAsyncResult[InboundChannels.Count],
                                   asyncOut = new IAsyncResult[OutboundChannels.Count];
                    int i;

                    //Begin async start outbound channels
                    _outLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                    try
                    {
                        i = 0;
                        foreach (IRunnable chan in OutboundChannels)
                        {
                            if (!chan.Running && chan is IAsyncRunnable) asyncOut[i] = ((IAsyncRunnable)chan).BeginStart();
                            i++;
                        }
                    }
                    finally
                    {
                        _outLock.ReleaseReaderLock();
                    }


                    _hubThreadStop = false;

                    //Start hub threads
                    _hubThreads = new Thread[WORKER_THREADS];
                    for (i = 0; i < WORKER_THREADS; i++)
                    {
                        _hubThreads[i] = new Thread(HubThreadLoop)
                                             {
                                                 Name = string.Format("LogbusService.HubThreadLoop[{0}]", i),
                                                 Priority = ThreadPriority.Normal,
                                                 IsBackground = true
                                             };
                        _hubThreads[i].Start(Queues[i]);
                    }

                    if (_forwardingEnabled)
                    {
                        ForwardingQueue = new FastFifoQueue<SyslogMessage>(4096);
                        _forwardingThread = new Thread(ForwardLoop)
                                                {
                                                    IsBackground = true,
                                                    Priority = ThreadPriority.Normal,
                                                    Name = "LogbusService.ForwardLoop"
                                                };
                        _forwardingThread.Start();
                    }

                    //End async start/sync start outbound channels
                    _outLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                    try
                    {
                        i = 0;
                        foreach (IRunnable chan in OutboundChannels)
                        {
                            if (!chan.Running)
                                if (chan is IAsyncRunnable) ((IAsyncRunnable)chan).EndStart(asyncOut[i]);
                                else chan.Start();
                            i++;
                        }
                    }
                    finally
                    {
                        _outLock.ReleaseReaderLock();
                    }


                    //Begin start async inbound channels and read messages
                    _inLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                    try
                    {
                        i = 0;
                        foreach (IInboundChannel chan in InboundChannels)
                        {
                            chan.MessageReceived += ChannelMessageReceived;
                            if (!chan.Running && chan is IAsyncRunnable) asyncIn[i] = ((IAsyncRunnable)chan).BeginStart();
                            i++;
                        }
                    }
                    finally
                    {
                        _inLock.ReleaseReaderLock();
                    }

                    //End async start/sync start inbound channels
                    _inLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                    try
                    {
                        i = 0;
                        foreach (IRunnable chan in InboundChannels)
                        {
                            if (!chan.Running)
                                if (chan is IAsyncRunnable) ((IAsyncRunnable)chan).EndStart(asyncIn[i]);
                                else chan.Start();
                            i++;
                        }
                    }
                    finally
                    {
                        _inLock.ReleaseReaderLock();
                    }
                }
                catch (Exception ex)
                {
                    LogbusException e = new LogbusException("Cannot start Logbus", ex);
                    if (Error != null) Error(this, new UnhandledExceptionEventArgs(e, true));
                    throw e;
                }

                if (Started != null) Started(this, EventArgs.Empty);
                _running = true;
                Log.Info("LogbusService started");
            }
            catch (Exception ex)
            {
                Log.Error("LogbusService start failed");
                Log.Debug("Error details: {0}", ex.Message);
                LogbusException e = new LogbusException("Could not start Logbus", ex);
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(e, true));
                throw e;
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
                if (!_running) throw new InvalidOperationException("Logbus is not started");

                if (Stopping != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Stopping(this, e);
                    if (e.Cancel) return;
                }

                IAsyncResult[] asyncIn = new IAsyncResult[InboundChannels.Count],
                               asyncOut = new IAsyncResult[OutboundChannels.Count];
                //Reverse-order stop
                int i;

                //Begin async stop inbound channels
                _inLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    i = 0;
                    foreach (IInboundChannel chan in InboundChannels)
                    {
                        chan.MessageReceived -= MessageReceived;
                        if (chan.Running && chan is IAsyncRunnable) asyncIn[i] = ((IAsyncRunnable)chan).BeginStop();
                        i++;
                    }
                }
                finally
                {
                    _inLock.ReleaseReaderLock();
                }


                //Tell the thread to stop, the good way
                _hubThreadStop = true;

                //Begin async stop out
                _outLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    i = 0;
                    foreach (IRunnable chan in OutboundChannels)
                    {
                        if (chan.Running && chan is IAsyncRunnable) asyncOut[i] = ((IAsyncRunnable)chan).BeginStop();
                        i++;
                    }
                }
                finally
                {
                    _outLock.ReleaseReaderLock();
                }


                //End async stop Stop inbound channels so we won't get new messages
                _inLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    i = 0;
                    foreach (IRunnable chan in InboundChannels)
                    {
                        if (chan.Running)
                            if (chan is IAsyncRunnable) ((IAsyncRunnable)chan).EndStop(asyncIn[i]);
                            else chan.Stop();
                        i++;
                    }
                }
                finally
                {
                    _inLock.ReleaseReaderLock();
                }


                //Stop hub and let it flush messages
                for (i = 0; i < WORKER_THREADS; i++)
                {
                    _hubThreads[i].Interrupt();
                }
                for (i = 0; i < WORKER_THREADS; i++)
                {
                    _hubThreads[i].Join(DEFAULT_JOIN_TIMEOUT);
                }

                if (_forwardingEnabled)
                {
                    _forwardingThread.Interrupt();
                    _forwardingThread.Join(DEFAULT_JOIN_TIMEOUT);
                }

                //End async stp/sync stop out channels
                _outLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    i = 0;
                    foreach (IRunnable chan in OutboundChannels)
                    {
                        if (chan.Running && chan is IAsyncRunnable) ((IAsyncRunnable)chan).EndStop(asyncOut[i]);
                        i++;
                    }
                }
                finally
                {
                    _outLock.ReleaseReaderLock();
                }

                _running = false;

                if (Stopped != null) Stopped(this, EventArgs.Empty);
                Log.Info("LogbusService stopped");
            }
            catch (Exception ex)
            {
                Log.Error("LogbusService stop failed");
                Log.Debug("Error details: {0}", ex.Message);
                LogbusException e = new LogbusException("Could not stop Logbus", ex);
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(e, true));
                throw e;
            }
        }

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
        public ITransportFactoryHelper TransportFactoryHelper { get; set; }

        /// <summary>
        /// Implements ILogCollector.SubmitMessage
        /// </summary>
        /// <param name="msg"></param>
        public void SubmitMessage(SyslogMessage msg)
        {
            //Round-robin queues. Need to compute modulo twice in order to deal with negative indexes
            Queues[(((Interlocked.Increment(ref _currentQueue)) % WORKER_THREADS) + WORKER_THREADS) % WORKER_THREADS].
                Enqueue(msg);
        }

        /// <summary>
        /// Implements ILogBus.CreateChannel
        /// </summary>
        public void CreateChannel(string channelId, string channelName, IFilter channelFilter, string channelDescription,
                                  long coalescenceWindow)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (channelId.Contains(":")) throw new ArgumentException("Cannot use colon (':') in channel ID");

            //Prepare channel
            IOutboundChannel newChan = ChannelFactory.CreateChannel(channelName, channelDescription, channelFilter);
            newChan.CoalescenceWindowMillis = (ulong)coalescenceWindow;
            newChan.ID = channelId;

            //First find if there is one with same ID
            if (GetOutboundChannel(channelId) != null)
            {
                LogbusException ex = new LogbusException("Channel already exists");
                ex.Data.Add("channelId", channelId);
                throw ex;
            }

            _outLock.AcquireWriterLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                OutboundChannels.Add(newChan);
            }
            finally
            {
                _outLock.ReleaseWriterLock();
            }

            if (_running) newChan.Start();
            Log.Info(string.Format("New channel created: {0}", channelId));

            if (OutChannelCreated != null) OutChannelCreated(this, new OutChannelCreationEventArgs(newChan));
        }

        /// <summary>
        /// Implements ILogBus.RemoveChannel
        /// </summary>
        public void RemoveChannel(string id)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            if (id.Contains(":")) throw new ArgumentException("Invalid channel ID");

            IOutboundChannel toRemove = null;
            //Find the channel
            _outLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                foreach (IOutboundChannel chan in OutboundChannels)
                    if (chan.ID == id)
                    {
                        toRemove = chan;
                        break;
                    }

                if (toRemove == null)
                {
                    Log.Warning(string.Format("Failed to remove channel {0}", id));
                    LogbusException ex = new LogbusException("Channel does not exist");
                    ex.Data.Add("channelId", id);
                    throw ex;
                }

                try
                {
                    LockCookie ck = _outLock.UpgradeToWriterLock(DEFAULT_JOIN_TIMEOUT);

                    try
                    {
                        OutboundChannels.Remove(toRemove);
                    }
                    finally
                    {
                        _outLock.DowngradeFromWriterLock(ref ck);
                    }

                    try
                    {
                        toRemove.Dispose();
                    }
                    catch (InvalidOperationException)
                    {
                    }

                    Log.Info(string.Format("Channel removed: {0}", id));

                    if (OutChannelDeleted != null)
                        OutChannelDeleted(this, new OutChannelDeletionEventArgs(id));
                }
                catch (Exception ex)
                {
                    if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, false));
                    Log.Error("Unable to delete channel {0}", id);
                    Log.Debug("Error details: {0}", ex.Message);
                    throw;
                }
            }
            finally
            {
                _outLock.ReleaseReaderLock();
            }

            toRemove.Dispose();
        }

        /// <summary>
        /// Implements ILogBus.SubscribeClient
        /// </summary>
        public string SubscribeClient(string channelId, string transportId,
                                      IEnumerable<KeyValuePair<string, string>> transportInstructions,
                                      out IEnumerable<KeyValuePair<string, string>> clientInstructions)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(channelId))
                throw new ArgumentNullException("channelId", "Channel ID cannot be null");
            if (channelId.Contains(":")) throw new ArgumentException("Invalid channel ID");

            Log.Info("New client subscribing to channel {0} via transport \"{1}\"", channelId, transportId);
            try
            {
                //First find the channel
                IOutboundChannel channel = GetOutboundChannel(channelId);

                if (channel == null)
                {
                    LogbusException ex = new LogbusException("Channel does not exist");
                    ex.Data.Add("channelId", channelId);
                    throw ex;
                }

                if (ClientSubscribing != null)
                {
                    ClientSubscribingEventArgs e = new ClientSubscribingEventArgs(channel, transportId,
                                                                                  transportInstructions);
                    ClientSubscribing(this, e);
                    if (e.Cancel)
                    {
                        throw new LogbusException(
                            string.Format("You cannot subscribe to this channel for the following reasons: {0}",
                                          string.Join(", ", e.ReasonForCanceling)));
                    }
                }

                try
                {
                    string clientId = string.Format("{0}:{1}", channelId,
                                                    channel.SubscribeClient(transportId, transportInstructions,
                                                                            out clientInstructions));
                    if (ClientSubscribed != null)
                    {
                        IDictionary<string, string> clientInstro = new Dictionary<string, string>();
                        foreach (KeyValuePair<string, string> pair in clientInstructions)
                        {
                            clientInstro.Add(pair);
                        }
                        ClientSubscribedEventArgs e = new ClientSubscribedEventArgs(channel, transportId,
                                                                                    transportInstructions, clientId,
                                                                                    clientInstro);
                        ClientSubscribed(this, e);
                        clientInstructions = clientInstro;
                    }

                    Log.Info("A new client subscribed channel {0} with ID {1}", channel.ID, clientId);

                    return clientId;
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
            catch (Exception ex)
            {
                Exception realEx = (ex is LogbusException) ? ex.InnerException : ex;
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(realEx, false));

                Log.Error("Unable to subscribe client to channel {0}", channelId);
                Log.Debug("Error details: {0}", realEx.Message);

                throw;
            }
        }

        /// <summary>
        /// Implements ILogBus.RefreshClient
        /// </summary>
        public void RefreshClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException("clientId", "Client ID must not be null");

            try
            {
                int indexof = clientId.IndexOf(':');
                if (indexof < 0)
                {
                    ArgumentException ex = new ArgumentException("Invalid client ID");
                    ex.Data.Add("clientId-Logbus", clientId);
                    throw ex;
                }

                string chanName = clientId.Substring(0, indexof), chanClientId = clientId.Substring(indexof + 1);
                if (string.IsNullOrEmpty(chanName))
                {
                    ArgumentException ex = new ArgumentException("Invalid client ID");
                    ex.Data.Add("clientId-Logbus", clientId);
                    throw ex;
                }

                //First find the channel
                IOutboundChannel channel = GetOutboundChannel(chanName);

                if (channel == null)
                {
                    LogbusException ex = new LogbusException("Channel does not exist");
                    ex.Data.Add("client-Logbus", clientId);
                    throw ex;
                }

                try
                {
                    channel.RefreshClient(chanClientId);
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
            catch (Exception ex)
            {
                Exception realEx = (ex is LogbusException) ? ex.InnerException : ex;
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(realEx, false));

                Log.Error("Unable to refresh client {0}", clientId);
                Log.Debug("Error details: {0}", realEx.Message);

                throw;
            }
        }

        /// <summary>
        /// Implements ILogBus.UnsubscribeClient
        /// </summary>
        public void UnsubscribeClient(string clientId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException("clientId", "Client ID must not be null");

            try
            {
                int indexof = clientId.IndexOf(':');
                if (indexof < 0)
                {
                    ArgumentException ex = new ArgumentException("Invalid client ID");
                    ex.Data.Add("clientId-Logbus", clientId);
                    throw ex;
                }

                string chanName = clientId.Substring(0, indexof), chanClientId = clientId.Substring(indexof + 1);
                if (string.IsNullOrEmpty(chanName))
                {
                    ArgumentException ex = new ArgumentException("Invalid client ID");
                    ex.Data.Add("clientId-Logbus", clientId);
                    throw ex;
                }

                //First find the channel
                IOutboundChannel channel = GetOutboundChannel(chanName);

                if (channel == null)
                {
                    LogbusException ex = new LogbusException("Channel does not exist");
                    ex.Data.Add("client-Logbus", clientId);
                    throw ex;
                }

                try
                {
                    channel.UnsubscribeClient(chanClientId);

                    if (ClientUnsubscribed != null)
                    {
                        ClientUnsubscribedEventArgs e = new ClientUnsubscribedEventArgs(channel, clientId);
                        ClientUnsubscribed(this, e);
                    }

                    Log.Info("Client {0} unsubscribed from channel {1}", clientId, channel.ID);
                }
                catch (LogbusException ex)
                {
                    ex.Data.Add("client-Logbus", clientId);
                    throw;
                }
                catch (Exception e)
                {
                    LogbusException ex = new LogbusException("Unable to unsubscribe client", e);
                    ex.Data.Add("client-Logbus", clientId);
                    throw;
                }
            }
            catch (Exception ex)
            {
                Exception realEx = (ex is LogbusException) ? ex.InnerException : ex;
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(realEx, false));

                Log.Error("Unable to unsubscribe client {0}", clientId);
                Log.Debug("Error details: {0}", realEx.Message);

                throw;
            }
        }

        /// <summary>
        /// Implements ILogBus.AddOutboundChannel
        /// </summary>
        void ILogBus.AddOutboundChannel(IOutboundChannel channel)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            _outLock.AcquireWriterLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                OutboundChannels.Add(channel);
            }
            finally
            {
                _outLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Implements ILogBus.RemoveOutboundChannel
        /// </summary>
        public void RemoveOutboundChannel(string channelId)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            RemoveChannel(channelId);
        }

        /// <summary>
        /// Implements ILogBus.AddInboundChannel
        /// </summary>
        void ILogBus.AddInboundChannel(IInboundChannel channel)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (channel == null) throw new ArgumentNullException("channel");

            try
            {
                _inLock.AcquireWriterLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    InboundChannels.Add(channel);
                    if (_running)
                        try
                        {
                            channel.Start();
                        }
                        catch (InvalidOperationException)
                        {
                        }
                }
                finally
                {
                    _inLock.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                throw new LogbusException("Unable to add inbound channel", ex);
            }
        }

        /// <summary>
        /// Implements ILogBus.RemoveInboundChannel
        /// </summary>
        void ILogBus.RemoveInboundChannel(IInboundChannel channel)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (channel == null) throw new ArgumentNullException("channel");


            try
            {
                _inLock.AcquireWriterLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    InboundChannels.Remove(channel);
                    try
                    {
                        channel.Stop();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
                finally
                {
                    _inLock.ReleaseWriterLock();
                }
            }
            catch (ApplicationException ex)
            {
                throw new LogbusException("Unable to add inbound channel", ex);
            }
        }

        /// <summary>
        /// Implements ILogBus.GetOutboundChannel
        /// </summary>
        public IOutboundChannel GetOutboundChannel(string channelId)
        {
            _outLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                foreach (IOutboundChannel chan in OutboundChannels)
                    if (chan.ID == channelId) return chan;
                return null;
            }
            finally
            {
                _outLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Implements ILogBus.ClientSubscribing
        /// </summary>
        public event EventHandler<ClientSubscribingEventArgs> ClientSubscribing;

        /// <summary>
        /// Implements ILogBus.ClientSubscribed
        /// </summary>
        public event EventHandler<ClientSubscribedEventArgs> ClientSubscribed;

        /// <summary>
        /// Implements ILogBus.ClientUnsubscribed
        /// </summary>
        public event EventHandler<ClientUnsubscribedEventArgs> ClientUnsubscribed;

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            if (Disposed) return;
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (Disposed) return;

            GC.SuppressFinalize(this);

            try
            {
                Stop(); //There could be problems with this
            }
            catch
            {
            }

            try
            {
                if (_plugins != null)
                    foreach (IPlugin plugin in _plugins)
                        if (plugin != null)
                            try
                            {
                                plugin.Unregister();
                                if (disposing) plugin.Dispose();
                            }
                            catch
                            {
                            }
                _statistics.Dispose();
            }
            catch
            {
            } //Don't propagate, ever
            finally
            {
                Disposed = true;
            }
        }

        #endregion

        #region Channel support

        protected IOutboundChannelFactory ChannelFactory { get; set; }

        private void ChannelMessageReceived(object sender, SyslogMessageEventArgs e)
        {
            SubmitMessage(e.Message);
        }

        #endregion

        #region IChannelManagement Membri di

        string[] IChannelManagement.ListChannels()
        {
            _outLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
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
            finally
            {
                _outLock.ReleaseReaderLock();
            }
        }

        void IChannelManagement.CreateChannel(ChannelCreationInformation description)
        {
            if (description == null) throw new ArgumentNullException("description");
            CreateChannel(description.id, description.title, description.filter, description.description,
                          description.coalescenceWindow);
        }

        ChannelInformation IChannelManagement.GetChannelInformation(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            IOutboundChannel chan = null;

            _outLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                foreach (IOutboundChannel ch in OutboundChannels)
                    if (ch.ID == id)
                    {
                        chan = ch;
                        break;
                    }
            }
            finally
            {
                _outLock.ReleaseReaderLock();
            }

            if (chan == null) return null; //Really?

            return new ChannelInformation
                       {
                           clients = chan.SubscribedClients.ToString(),
                           coalescenceWindow = (long)chan.CoalescenceWindowMillis,
                           description = chan.Description,
                           filter = chan.Filter as FilterBase,
                           id = chan.ID,
                           title = chan.Name
                       };
        }

        void IChannelManagement.DeleteChannel(string id)
        {
            RemoveChannel(id);
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

        ChannelSubscriptionResponse IChannelSubscription.SubscribeChannel(ChannelSubscriptionRequest request)
        {
            IEnumerable<KeyValuePair<string, string>> outParams;
            Dictionary<string, string> inParams = new Dictionary<string, string>();
            foreach (RemoteLogbus.KeyValuePair kvp in request.param)
                inParams.Add(kvp.name, kvp.value);
            string clientid;
            try
            {
                clientid = SubscribeClient(request.channelid, request.transport, inParams, out outParams);
            }
            catch (LogbusException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LogbusException("Unable to subscribe to channel", ex);
            }

            ChannelSubscriptionResponse ret = new ChannelSubscriptionResponse { clientid = clientid };

            List<RemoteLogbus.KeyValuePair> lst = new List<RemoteLogbus.KeyValuePair>();
            foreach (KeyValuePair<string, string> kvp in outParams)
                lst.Add(new RemoteLogbus.KeyValuePair { name = kvp.Key, value = kvp.Value });
            ret.param = lst.ToArray();

            return ret;
        }

        void IChannelSubscription.UnsubscribeChannel(string id)
        {
            UnsubscribeClient(id);
        }

        void IChannelSubscription.RefreshSubscription(string id)
        {
            RefreshClient(id);
        }

        string[] IChannelSubscription.GetAvailableFilters()
        {
            return CustomFilterHelper.Instance.GetAvailableCustomFilters();
        }

        FilterDescription IChannelSubscription.DescribeFilter(string filterid)
        {
            return CustomFilterHelper.Instance.DescribeFilter(filterid);
        }

        #endregion

        #region Support

        private void HubThreadLoop(object queue)
        {
            IFifoQueue<SyslogMessage> localQueue = (IFifoQueue<SyslogMessage>)queue;
            //Loop until end
            try
            {
                uint consecutiveFailsEvent = 0, consecutiveFailsChan = 0; //I don't care about overflowing
                do
                {
                    //Get message
                    SyslogMessage newMessage = localQueue.Dequeue();

                    //Filter message
                    if (!MainFilter.IsMatch(newMessage)) continue;

                    //Deliver to event listeners (SYNCHRONOUS: THREAD-BLOCKING!!!!!!!!!!!!!)
                    if (MessageReceived != null)
                        try
                        {
                            MessageReceived(this, new SyslogMessageEventArgs(newMessage));
                            consecutiveFailsEvent = 0;
                        }
                        catch (Exception ex)
                        {
                            consecutiveFailsEvent++;
                            if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, false));

                            if (consecutiveFailsEvent < 5) //Avoiding a potential infinite loop if a permanent error is present in the target
                            {
                                Log.Error("Unable to forward a log message to event listeners");
                                Log.Debug("Error details: {0}", ex.Message);
                            }
                        }


                    //Deliver to channels
                    //Theorically, it's as faster as channels can do
                    if (OutboundChannels != null)
                    {
                        _outLock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                        try
                        {
                            foreach (IOutboundChannel chan in OutboundChannels)
                            {
                                //Idea for the future: use Thread Pool to asynchronously deliver messages
                                //Could lead to a threading disaster in case of large rates of messages
                                try
                                {
                                    chan.SubmitMessage(newMessage);
                                }
                                catch (Exception ex)
                                {
                                    consecutiveFailsChan++;
                                    if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, false));

                                    if (consecutiveFailsChan < 5) //Avoiding a potential infinite loop if a permanent error is present in the target
                                    {
                                        Log.Error("Unable to forward a log message to event listeners");
                                        Log.Debug("Error details: {0}", ex.Message);
                                    }
                                }
                                consecutiveFailsChan = 0;
                            }
                        }
                        finally
                        {
                            _outLock.ReleaseReaderLock();
                        }
                    }
                    
                    Interlocked.Increment(ref _processedMessages);
                    
                    if (_forwardingEnabled && ForwardingQueue != null) ForwardingQueue.Enqueue(newMessage);
                } while (!_hubThreadStop);
            }
            catch (ThreadInterruptedException)
            {
            }
            finally
            {
                //Someone is telling me to stop
                try
                {
                    //Flush queue and then stop
                    IEnumerable<SyslogMessage> leftMessages = localQueue.Flush();
                    foreach (SyslogMessage msg in leftMessages)
                    {
                        if (!MainFilter.IsMatch(msg)) continue;

                        //Deliver to event listeners (SYNCHRONOUS: THREAD-BLOCKING!!!!!!!!!!!!!)
                        if (MessageReceived != null)
                            try
                            {
                                MessageReceived(this, new SyslogMessageEventArgs(msg));
                            }
                            catch (Exception ex)
                            {
                                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, false));
                            }

                        //Deliver to channels
                        //Theorically, it's as faster as channels can do
                        if (OutboundChannels != null)
                            foreach (IOutboundChannel chan in OutboundChannels)
                            {
                                //Idea for the future: use Thread Pool to asynchronously deliver messages
                                //Could lead to a threading disaster in case of large rates of messages
                                try
                                {
                                    chan.SubmitMessage(msg);
                                }
                                catch (Exception ex)
                                {
                                    if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, false));
                                }
                            }
                    }
                }
                catch (ThreadInterruptedException) { } //Residual
                catch { } //There's nothing we can do, anyway
            }
        }

        private void ForwardLoop()
        {
            try
            {
                uint consecutiveFails = 0;
                do
                {
                    //Get message
                    SyslogMessage newMessage = ForwardingQueue.Dequeue();

                    try
                    {
                        Forwarder.SubmitMessage(newMessage);
                        consecutiveFails = 0;
                    }
                    catch (Exception ex)
                    {
                        consecutiveFails++;
                        if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, false));

                        if (consecutiveFails < 5) //Avoiding a potential infinite loop if a permanent error is present in the logger
                        {
                            Log.Error("Unable to forward a log message");
                            Log.Debug("Error details: {0}", ex.Message);
                        }
                    }

                } while (!_hubThreadStop);
            }
            catch (ThreadInterruptedException) { }
        }

        private ILog Log { get; set; }

        private void LogStatistics(object state)
        {
            string[] queuesStatus = new string[WORKER_THREADS], states = new string[WORKER_THREADS];
            for (int i = 0; i < WORKER_THREADS; i++)
            {
                queuesStatus[i] = Queues[i].Count.ToString(CultureInfo.CurrentUICulture);
                states[i] = Enum.GetName(typeof(ThreadState), _hubThreads[i].ThreadState);
            }
            Log.Debug("During the last minute LogbusService processed {0} messages. There are ({1}) items in queues. Threads status ({2}).",
                Interlocked.Exchange(ref _processedMessages, 0).ToString(),
                string.Format("({0})", string.Join(",", queuesStatus)),
                string.Format("({0})", string.Join(",", queuesStatus))
                );
        }

        #endregion
    }
}