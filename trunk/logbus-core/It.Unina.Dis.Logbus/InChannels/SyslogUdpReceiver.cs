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

namespace It.Unina.Dis.Logbus.InChannels
{
    public class SyslogUdpReceiver :
        IInboundChannel
    {
        private Dictionary<string, string> config = new Dictionary<string, string>();
        private UdpClient client;
        private bool disposed = false;
        private bool stopped = true;
        private object objLock = new object();

        private Thread running_thread;

        #region Constructor/destructor
        public SyslogUdpReceiver() { }

        ~SyslogUdpReceiver()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);

            Stop();
            if (disposing)
            {
            }
            disposed = true;
        }

        #endregion


        public int Port
        {
            get;
            set;
        }


        #region IInboundChannel Membri di

        public void Start()
        {
            if (disposed) throw new ObjectDisposedException("this");
            if (running_thread != null && running_thread.IsAlive) throw new InvalidOperationException("Listener is already running");
            ///Configure

            IpAddress = (Configuration["ip"] != null) ? Configuration[IpAddress] : null;
            if (Configuration["port"] == null) throw new InvalidOperationException("UDP port not set");
            int portnum;
            if (int.TryParse(Configuration["port"], out portnum)) throw new InvalidOperationException("Invalid UDP port");
            if (Port < 1 || Port > 65535) throw new InvalidOperationException(string.Format("Invalid UDP port: {0}", Port.ToString(CultureInfo.CurrentCulture)));
            Port = portnum;

            try
            {
                client = new UdpClient(Port);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("Cannot start UDP listener", ex);
            }

            running_thread = new Thread(RunnerLoop);
            running_thread.IsBackground = true;
            running_thread.Name = "UDPListener.RunnerLoop";
            running_thread.Start();

        }

        public void Stop()
        {
            if (disposed) throw new ObjectDisposedException("this");
            if (running_thread == null || !running_thread.IsAlive) throw new InvalidOperationException("Listener is not running");

            lock (objLock) stopped = true;

            running_thread.Join();
        }

        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

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

        public string IpAddress
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
            lock (objLock) stopped = false;

            IPAddress address = (this.IpAddress == null) ? IPAddress.Any : IPAddress.Parse(this.IpAddress);
            IPEndPoint ep = new IPEndPoint(address, Port);
            while (!stopped)
            {
                byte[] payload = client.Receive(ref ep);
                try
                {
                    SyslogMessage new_message = SyslogMessage.Parse(payload);
                    if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(new_message));
                }
                catch (Exception) { }
            }
        }
    }
}
