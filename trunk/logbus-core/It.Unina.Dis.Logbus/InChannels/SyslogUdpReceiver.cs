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
        ReceiverBase
    {
        /// <summary>
        /// Default port to listen
        /// </summary>
        public const int DEFAULT_PORT = 514;

        public const int WORKER_THREADS = 4;

        private Thread[] listener_threads, parser_threads;

        private Dictionary<string, string> config = new Dictionary<string, string>();
        private UdpClient client;

        private BlockingFifoQueue<byte[]>[] byte_queue;

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

        /// <summary>
        /// Implements IRunnable.Start
        /// </summary>
        protected override void OnStart()
        {
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

            listener_threads = new Thread[WORKER_THREADS];
            parser_threads = new Thread[WORKER_THREADS];
            for (int i = 0; i < WORKER_THREADS; i++)
            {
                byte_queue[i] = new BlockingFifoQueue<byte[]>();

                listener_threads[i] = new Thread(ListenerLoop);
                listener_threads[i].Name = string.Format("SyslogUdpReceiver[{1}].ListenerLoop[{0}]", i, Name);
                listener_threads[i].IsBackground = true;
                listener_threads[i].Priority = ThreadPriority.AboveNormal;
                listener_threads[i].Start(i);

                parser_threads[i] = new Thread(ParserLoop);
                parser_threads[i].Name = string.Format("SyslogUdpReceiver[{1}].ListenerLoop[{0}]", i, Name);
                parser_threads[i].IsBackground = true;
                parser_threads[i].Start(i);
            }
        }

        protected override void OnStop()
        {
            try
            {
                client.Close(); //Trigger SocketException if thread is blocked into listening
                for (int i = 0; i < WORKER_THREADS; i++)
                    listener_threads[i].Join();
                listener_threads = null;
            }
            catch { } //Really nothing?

            try
            {
                for (int i = 0; i < WORKER_THREADS; i++)
                    parser_threads[i].Interrupt();
                for (int i = 0; i < WORKER_THREADS; i++)
                    parser_threads[i].Join();
                parser_threads = null;
            }
            catch { }

        }

        #region IConfigurable Membri di

        /// <summary>
        /// Implements IConfigurable.GetConfigurationParameter
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override string GetConfigurationParameter(string key)
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
        public override void SetConfigurationParameter(string key, string value)
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
        public override IEnumerable<KeyValuePair<string, string>> Configuration
        {
            set
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                foreach (KeyValuePair<string, string> kvp in value)
                    SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }

        #endregion

        private void ParserLoop(object queue)
        {
            int queue_id = (int)queue;
            try
            {
                byte[] payload;
                while (true)
                {
                    payload = byte_queue[queue_id].Dequeue();

                    try
                    {
                        SyslogMessage new_message = SyslogMessage.Parse(payload);
                        ForwardMessage(new_message);
                    }
                    catch (FormatException ex)
                    {
                        ParseErrorEventArgs e = new ParseErrorEventArgs(payload, ex, false);
                        OnParseError(e);
                    }
                }
            }
            catch (ThreadInterruptedException)
            { }
            finally
            {
                byte[][] final_messages = byte_queue[queue_id].FlushAndDispose();
                foreach (byte[] payload in final_messages)
                {
                    try
                    {
                        SyslogMessage new_message = SyslogMessage.Parse(payload);
                        ForwardMessage(new_message);
                    }
                    catch (FormatException ex)
                    {
                        ParseErrorEventArgs e = new ParseErrorEventArgs(payload, ex, false);
                        OnParseError(e);
                    }
                }
            }
        }

        private void ListenerLoop(object queue)
        {
            int queue_id = (int)queue;
            IPEndPoint remote_endpoint = new IPEndPoint(IPAddress.Any, 0);
            while (Running)
            {
                try
                {
                    byte[] payload = client.Receive(ref remote_endpoint);

                    byte_queue[queue_id].Enqueue(payload);
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
    }
}
