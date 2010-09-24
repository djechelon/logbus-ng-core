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
using It.Unina.Dis.Logbus.Utils;

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

        private Thread[] _listenerThreads, _parserThreads;

        private UdpClient _client;

        private BlockingFifoQueue<byte[]>[] _byteQueues;

        private bool _listen = false;

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

            IPEndPoint localEp;
            if (IpAddress == null) localEp = new IPEndPoint(IPAddress.Any, Port);
            else localEp = new IPEndPoint(IPAddress.Parse(IpAddress), Port);

            try
            {
                _client = new UdpClient(localEp);
            }
            catch (IOException ex)
            {
                throw new LogbusException("Cannot start UDP listener", ex);
            }
            _listen = true;
            _listenerThreads = new Thread[WORKER_THREADS];
            _parserThreads = new Thread[WORKER_THREADS];
            _byteQueues= new BlockingFifoQueue<byte[]>[WORKER_THREADS];
            for (int i = 0; i < WORKER_THREADS; i++)
            {
                _byteQueues[i] = new BlockingFifoQueue<byte[]>();

                _listenerThreads[i] = new Thread(ListenerLoop)
                                          {
                                              Name = string.Format("SyslogUdpReceiver[{1}].ListenerLoop[{0}]", i, Name),
                                              IsBackground = true,
                                              Priority = ThreadPriority.AboveNormal
                                          };
                _listenerThreads[i].Start(i);

                _parserThreads[i] = new Thread(ParserLoop)
                                        {
                                            Name = string.Format("SyslogUdpReceiver[{1}].ParserLoop[{0}]", i, Name),
                                            IsBackground = true
                                        };
                _parserThreads[i].Start(i);
            }
        }

        protected override void OnStop()
        {
            _listen = false;
            try
            {
                _client.Close(); //Trigger SocketException if thread is blocked into listening
                for (int i = 0; i < WORKER_THREADS; i++)
                    _listenerThreads[i].Join();
                _listenerThreads = null;
            }
            catch { } //Really nothing?

            try
            {
                for (int i = 0; i < WORKER_THREADS; i++)
                    _parserThreads[i].Interrupt();
                for (int i = 0; i < WORKER_THREADS; i++)
                    _parserThreads[i].Join();
                _parserThreads = null;
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
            int queueId = (int)queue;
            try
            {
                while (true)
                {
                    byte[] payload = _byteQueues[queueId].Dequeue();

                    try
                    {
                        SyslogMessage newMessage = SyslogMessage.Parse(payload);
                        ForwardMessage(newMessage);
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
                byte[][] finalMessages = _byteQueues[queueId].FlushAndDispose();
                foreach (byte[] payload in finalMessages)
                {
                    try
                    {
                        SyslogMessage newMessage = SyslogMessage.Parse(payload);
                        ForwardMessage(newMessage);
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
            int queueId = (int)queue;
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            while (_listen)
            {
                try
                {
                    byte[] payload = _client.Receive(ref remoteEndpoint);

                    _byteQueues[queueId].Enqueue(payload);
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
