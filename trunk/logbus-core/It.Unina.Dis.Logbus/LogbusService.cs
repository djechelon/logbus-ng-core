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
    public class LogbusService : MarshalByRefObject, ILogBus//, ILogbusController
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
            try
            {
                //Try to auto-configure. If fails, skip for now. Somebody MUST then provide proper configuration
                Configuration = ConfigurationManager.GetSection("logbus") as LogbusCoreConfiguration;
            }
            catch { }
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
                            Type in_chan_type = Type.GetType(def.type, true, false);

                            if (!in_chan_type.IsAssignableFrom(typeof(IInboundChannel)))
                            {
                                LogbusConfigurationException ex = new LogbusConfigurationException("Specified type for Inbound channel does not implement IInboundChannel");
                                ex.Data["TypeName"] = def.type;
                            }
                            IInboundChannel channel = (IInboundChannel)Activator.CreateInstance(in_chan_type, true);
                            try
                            {
                                foreach (KeyValuePair param in def.param)
                                {
                                    channel.Configuration[param.name] = param.value;
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

                    InboundChannels = channels;
                    //Inbound channels end
                }

                //Outbound transports begin
                //For now, no other class is expected
                TransportFactoryHelper = new OutTransports.SimpleTransportHelper();

                //Add default transport factories
                TransportFactoryHelper.AddFactory("udp", new OutTransports.SyslogUdpTransportFactory());
                TransportFactoryHelper.AddFactory("multicast", new OutTransports.SyslogMulticastTransportFactory());
                if (Configuration.outtransports != null)
                {
                    //Add more custom transports

                    //Not yet implemented, not yet possible!
                    throw new NotImplementedException();
                }

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
            //return (TransportFactory != null) ? TransportFactory.GetAvailableTransports() : new string[0];
            throw new NotImplementedException();
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

            if (Starting != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                Starting(this, e);
                if (e.Cancel) return;
            }

            throw new NotImplementedException();

            try
            {
                //Start outbound channels
                foreach (IOutboundChannel chan in OutboundChannels)
                {
                    chan.Start();
                }

                //Start main hub thread
                HubThreadStop = false;
                hubThread = new Thread(this.HubThreadLoop);
                hubThread.Name = "LogbusService.HubThreadLoop";
                hubThread.Priority = ThreadPriority.Normal;
                hubThread.IsBackground = true;
                hubThread.Start();


                //Start inbound channels and read messages
                foreach (IInboundChannel chan in InboundChannels)
                {
                    chan.MessageReceived += this.MessageReceived;
                    chan.Start();
                }
            }
            catch (Exception ex)
            {
                LogbusException e = new LogbusException("Cannot start Logbus", ex);
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(e, true));
                throw e;

            }

            if (Started != null) Started(this, EventArgs.Empty);
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
                //Reverse-order stop

                //Stop inbound channels so we won't get new messages
                foreach (IInboundChannel chan in InboundChannels)
                {
                    chan.Stop();
                    chan.MessageReceived -= this.MessageReceived;
                }

                //Stop hub and let it flush messages

                //Tell the thread to stop, the good way
                HubThreadStop = true;
                hubThread.Join(5000); //Wait
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

                    //Finally stop the out channels
                    foreach (IOutboundChannel chan in OutboundChannels)
                        chan.Stop();
                }
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

        public event EventHandler<SyslogMessageEventArgs> ForwardMessage;

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

        private void MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            Queue.Enqueue(e.Message);
        }
        #endregion


        private void HubThreadLoop()
        {
            //First of all, init fresh queue
            Queue = new BlockingFifoQueue<SyslogMessage>();

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
                        if (ForwardMessage != null) ForwardMessage(this, new SyslogMessageEventArgs(new_message));

                        //Deliver to channels
                        //Theorically, it's as faster as channels can do
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
                    if (ForwardMessage != null) ForwardMessage(this, new SyslogMessageEventArgs(msg));

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
    }
}
