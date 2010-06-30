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

namespace It.Unina.Dis.Logbus.InChannels
{
    /// <summary>
    /// Collects Syslog messages over UDP unicast channels
    /// </summary>
    /// <remarks>Implements RFC5426</remarks>
    public sealed class SyslogUdpReceiver :
        IInboundChannel
    {
        public const int DEFAULT_PORT = 514;


        private Dictionary<string, string> config = new Dictionary<string, string>();
        private UdpClient client;
        private BlockingFifoQueue<SyslogMessage> queue;


        protected bool Stopped
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        private Thread running_thread;

        #region Constructor/destructor
        public SyslogUdpReceiver() { }

        protected bool Disposed
        {
            get;
            private set;
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

        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException("this");
            if (running_thread != null && running_thread.IsAlive) throw new InvalidOperationException("Listener is already running");
            ///Configure

            IpAddress = (Configuration["ip"] != null) ? Configuration["ip"] : null;
            int portnum;
            if (Configuration["port"] == null)
            {
                Port = DEFAULT_PORT;
            }
            else
            {
                if (!int.TryParse(Configuration["port"], out portnum)) throw new LogbusException("Invalid UDP port");
                if (portnum < 1 || portnum > 65535) throw new LogbusException(string.Format("Invalid UDP port: {0}", portnum.ToString(CultureInfo.CurrentCulture)));
                Port = portnum;
            }

            try
            {
                client = new UdpClient(Port);
            }
            catch (IOException ex)
            {
                throw new LogbusException("Cannot start UDP listener", ex);
            }

            running_thread = new Thread(RunnerLoop);
            running_thread.IsBackground = true;
            running_thread.Name = "UDPListener.RunnerLoop";
            running_thread.Start();

        }

        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException("this");
            if (running_thread == null || Stopped) throw new InvalidOperationException("Listener is not running");

            Stopped = true;

            try
            {
                client.Close(); //Trigger SocketException if thread is blocked into listening
                running_thread.Join();
            }
            catch (Exception) { } //Really nothing?
        }

        /// <summary>
        /// Configurable parameters:
        /// <list type="string">
        /// <item>ip: IP address of interface to bind the UDP listener to</item>
        /// <item>port: UDP port on which to listen. Default is 514</item>
        /// </list>
        /// </summary>
        public IDictionary<string, string> Configuration
        {
            get
            {
                return config;
            }
        }

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
            Stopped = false;

            IPAddress address = (this.IpAddress == null) ? IPAddress.Any : IPAddress.Parse(this.IpAddress);
            IPEndPoint local_endpoint = new IPEndPoint(address, Port);
            IPEndPoint remote_endpoint = new IPEndPoint(IPAddress.Any, 0);

            while (!Stopped)
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
                    if (Stopped) //Yes, we are closing
                        return;
                    //else nothing yet
                }
                catch (Exception) { } //Really do nothing? Shouldn't we stop the service?
            }
        }
    }
}
