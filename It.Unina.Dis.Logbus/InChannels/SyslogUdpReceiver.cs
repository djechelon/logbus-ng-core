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
        public const int DEFAULT_PORT = 514;


        private Dictionary<string, string> config = new Dictionary<string, string>();
        private UdpClient client;


        private bool Running
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private Thread running_thread;

        #region Constructor/destructor
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

        public event SyslogMessageEventHandler MessageReceived;

        public event ParseErrorEventHandler ParseError;


        public string Name
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


        #endregion

        private void RunnerLoop()
        {
            Running = true;

            IPEndPoint remote_endpoint = new IPEndPoint(IPAddress.Any, 0);
            while (Running)
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

        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event UnhandledExceptionEventHandler Error;

        public void Start()
        {
            try
            {
                if (Disposed) throw new ObjectDisposedException("this");
                if (running_thread != null && running_thread.IsAlive) throw new InvalidOperationException("Listener is already running");

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

                running_thread = new Thread(RunnerLoop);
                running_thread.IsBackground = true;
                running_thread.Name = "UDPListener.RunnerLoop";
                running_thread.Start();

                if (Started != null) Started(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (Error != null) Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                if (Disposed) throw new ObjectDisposedException("this");
                if (running_thread == null || !Running) throw new InvalidOperationException("Listener is not running");

                if (Stopping != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Stopping(this, e);
                    if (e.Cancel) return;
                }

                Running = false;

                try
                {
                    client.Close(); //Trigger SocketException if thread is blocked into listening
                    running_thread.Join();
                    running_thread = null;
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

        public string GetConfigurationParameter(string key)
        {
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

        public void SetConfigurationParameter(string key, string value)
        {
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

        public IEnumerable<KeyValuePair<string, string>> Configuration
        {
            set
            {
                foreach (KeyValuePair<string, string> kvp in value)
                    SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }

        #endregion
    }
}
