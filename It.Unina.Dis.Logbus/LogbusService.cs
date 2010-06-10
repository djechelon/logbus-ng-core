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
                Configuration = ConfigurationManager.GetSection("logbus") as LogbusConfiguration;
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
        public LogbusConfiguration Configuration
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

                    //Outbound transports begin
                    if (Configuration.outtransports == null)
                    {
                        //Handling this by using a default factory
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
                LogbusConfigurationException ex = new LogbusConfigurationException("Unable to configure Logbus, See inner exception for details", e);
                ex.ConfigurationObject = Configuration;
                throw ex;
            }
        }

        public void Configure(LogbusConfiguration config)
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

        public virtual IOutboundTransportFactory TransportFactory
        {
            get;
            set;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
        }


        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event EventHandler<SyslogMessageEventArgs> ForwardMessage;

        public event UnhandledExceptionEventHandler Error;

        public string[] AvailableTransports
        {
            get { throw new NotImplementedException(); }
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
            Stop();
            Disposed = true;
        }
        #endregion

        #region Channel support
        private void MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
