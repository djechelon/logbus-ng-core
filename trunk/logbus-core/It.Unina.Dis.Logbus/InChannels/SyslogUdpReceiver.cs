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
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using It.Unina.Dis.Logbus.Utils;
using System.ComponentModel;

namespace It.Unina.Dis.Logbus.InChannels
{
    /// <summary>
    /// Collects Syslog messages over UDP unicast channels
    /// </summary>
    /// <remarks>Implements RFC5426</remarks>
    public sealed class SyslogUdpReceiver :
        IInboundChannel, IAsyncRunnable
    {
        /// <summary>
        /// Default port to listen
        /// </summary>
        public const int DEFAULT_PORT = 514;

        /// <summary>
        /// Number of worker threads concurrently listening for datagrams
        /// </summary>
        public const int WORKER_THREADS = 4;

        private Dictionary<string, string> config = new Dictionary<string, string>();
        private UdpClient client;
        private volatile bool running;

        private Thread[] running_threads;

        #region Constructor/destructor

        /// <summary>
        /// Initializes a new instance of SyslogUdpReceiver
        /// </summary>
        public SyslogUdpReceiver()
        {
            startDelegate = new ThreadStart(Start);
            stopDelegate = new ThreadStart(Stop);
        }

        private bool Disposed
        {
            get;
            set;
        }

        /// <summary>
        /// Default destructor. Releases resources
        /// </summary>
        ~SyslogUdpReceiver()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);

            try
            {
                Stop();
            }
            catch (Exception) { }

            if (disposing)
            {
            }
            Disposed = true;
        }

        #endregion

        /// <summary>
        /// Port to listen on
        /// </summary>
        public int Port
        {
            get;
            set;
        }

        /// <summary>
        /// Interface to listen on
        /// </summary>
        public string IpAddress
        {
            get;
            set;
        }

        #region IInboundChannel Membri di

        /// <remarks/>
        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        /// <remarks/>
        public event EventHandler<ParseErrorEventArgs> ParseError;

        /// <remarks/>
        public string Name
        {
            get;
            set;
        }

        #endregion

        #region IDisposable Membri di

        /// <summary>
        /// Implements IDisposable.Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }


        #endregion

        private void RunnerLoop()
        {
            IPEndPoint remote_endpoint = new IPEndPoint(IPAddress.Any, 0);
            while (running)
            {
                try
                {
                    byte[] payload = client.Receive(ref remote_endpoint);
                    try
                    {
                        SyslogMessage new_message = SyslogMessage.Parse(payload);
                        if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(new_message));
                    }
                    catch (FormatException ex)
                    {
                        ParseErrorEventArgs e = new ParseErrorEventArgs(payload, ex, false);
                        if (ParseError != null) ParseError(this, e);
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

        #region IRunnable Membri di

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
        public void Start()
        {
            try
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                if (running) throw new InvalidOperationException("Listener is already running");

                if (Starting != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Starting(this, e);
                    if (e.Cancel) return;
                }

                if (Port == 0)
                {
                    Port = DEFAULT_PORT;
                }

                IPEndPoint local_ep;
                if (IpAddress == null) local_ep = new IPEndPoint(IPAddress.Any, Port);
                else local_ep = new IPEndPoint(IPAddress.Parse(IpAddress), Port);

                try
                {
                    client = new UdpClient(local_ep);
                }
                catch (IOException ex)
                {
                    throw new LogbusException("Cannot start UDP listener", ex);
                }

                running = true;

                running_threads = new Thread[WORKER_THREADS];
                for (int i = 0; i < WORKER_THREADS; i++)
                {
                    running_threads[i] = new Thread(RunnerLoop);
                    running_threads[i].IsBackground = true;
                    running_threads[i].Name = string.Format(CultureInfo.InvariantCulture, "UDPListener.RunnerLoop[{0}]", i);
                    running_threads[i].Start();
                }

                if (Started != null) Started(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        /// <summary>
        /// Implements IRunnable.Stop
        /// </summary>
        public void Stop()
        {
            try
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                if (!running) throw new InvalidOperationException("Listener is not running");

                if (Stopping != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Stopping(this, e);
                    if (e.Cancel) return;
                }

                running = false;

                try
                {
                    client.Close(); //Trigger SocketException if thread is blocked into listening
                    for (int i = 0; i < WORKER_THREADS; i++)
                        running_threads[i].Join();
                    running_threads = null;
                }
                catch (Exception) { } //Really nothing?

                if (Stopped != null) Stopped(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        #endregion

        #region IAsyncRunnable Membri di

        IAsyncResult IAsyncRunnable.BeginStart()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            return startDelegate.BeginInvoke(null, null);
        }

        void IAsyncRunnable.EndStart(IAsyncResult result)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            startDelegate.EndInvoke(result);
        }

        IAsyncResult IAsyncRunnable.BeginStop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            return stopDelegate.BeginInvoke(null, null);
        }

        void IAsyncRunnable.EndStop(IAsyncResult result)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            stopDelegate.EndInvoke(result);
        }

        private ThreadStart startDelegate, stopDelegate;

        #endregion

        #region IConfigurable Membri di

        /// <summary>
        /// Implements IConfigurable.GetConfigurationParameter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfigurationParameter(string key)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            switch (key)
            {
                case "ip":
                    return IpAddress;
                case "port":
                    return Port.ToString(CultureInfo.InvariantCulture);
                default:
                    {
                        throw new NotSupportedException("Configuration parameter is not supported");
                    }
            }
        }

        /// <summary>
        /// Implements IConfigurable.SetConfigurationParameter
        /// </summary>
        public void SetConfigurationParameter(string key, string value)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            switch (key)
            {
                case "ip":
                    {
                        IpAddress = value;
                        break;
                    }
                case "port":
                    {
                        Port = int.Parse(value);
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter is not supported");
                    }
            }
        }

        /// <summary>
        /// Implements IConfigurable.Configuration
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Configuration
        { 
            set
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                foreach (KeyValuePair<string, string> kvp in value)
                    SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }

        #endregion
    }
}
