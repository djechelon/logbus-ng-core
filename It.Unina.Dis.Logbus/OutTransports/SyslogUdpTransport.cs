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
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using It.Unina.Dis.Logbus.Loggers;

namespace It.Unina.Dis.Logbus.OutTransports
{
    internal sealed class SyslogUdpTransport :
        IOutboundTransport, ILogSupport
    {
        private readonly Timer _clearTimer;
        private readonly Dictionary<String, UdpClientExpire> _clients;
        private volatile bool _disposed;
        private readonly ReaderWriterLock _listlock;
        private const int DEFAULT_JOIN_TIMEOUT = 5000;

        #region Constructor/Destructor

        public SyslogUdpTransport(long timeToLive)
        {
            SubscriptionTtl = timeToLive;
            _disposed = false;
            _clients = new Dictionary<String, UdpClientExpire>();
            _clearTimer = new Timer(TimeoutExpiredClients, null, SubscriptionTtl, SubscriptionTtl);
            _listlock = new ReaderWriterLock();
        }

        ~SyslogUdpTransport()
        {
            Dispose(false);
        }

        #endregion

        #region IOutboundTransport Membri di

        private void TimeoutExpiredClients(Object status)
        {
            if (_disposed)
                return;

            List<string> toRemove = new List<string>();

            _listlock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                foreach (KeyValuePair<string, UdpClientExpire> udpClientExpire in _clients)
                {
                    DateTime expire = udpClientExpire.Value.LastRefresh;
                    TimeSpan diff = DateTime.Now.Subtract(expire);
                    if (diff.TotalMilliseconds > SubscriptionTtl)
                    {
                        toRemove.Add(udpClientExpire.Key);
                        Log.Notice("Client {0} timed out", udpClientExpire.Key);
                    }
                }

                if (toRemove.Count > 0)
                {
                    LockCookie ck = _listlock.UpgradeToWriterLock(DEFAULT_JOIN_TIMEOUT);
                    try
                    {
                        foreach (string key in toRemove)
                        {
                            _clients.Remove(key);
                        }
                    }
                    finally
                    {
                        _listlock.DowngradeFromWriterLock(ref ck);
                    }
                }
            }
            finally
            {
                _listlock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Implements ILogCollector.SubmitMessage
        /// </summary>
        public void SubmitMessage(SyslogMessage message)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            byte[] dgram = message.ToByteArray();

            _listlock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                Dictionary<string, UdpClientExpire>.Enumerator enumeratore = _clients.GetEnumerator();

                while (enumeratore.MoveNext())
                {
                    UdpClient client = enumeratore.Current.Value.Client;
                    if (client != null)
                        try
                        {
                            client.BeginSend(dgram, dgram.Length, null, null);
                        }
                        catch (SocketException)
                        {
                            //What to do????
                            //For now, ignore and drop message
                        }
                }
            }
            finally
            {
                _listlock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Implements IOutboundTransport.SubscribedClients
        /// </summary>
        public int SubscribedClients
        {
            get
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                _listlock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    return _clients.Count;
                }
                finally
                {
                    _listlock.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputInstructions">List of input parameters (required):
        /// <list>
        /// <item>ip: IP address of client, or of another machine that will receive logs</item>
        /// <item>port: UDP port on which the destination will be listening</item>
        /// </list>
        /// </param>
        /// <param name="outputInstructions">List of client instructions:
        /// <list>
        /// <item>ttl: Time To Live in milliseconds</item>
        /// </list>
        /// </param>
        /// <returns></returns>
        public string SubscribeClient(IEnumerable<KeyValuePair<string, string>> inputInstructions,
                                      out IEnumerable<KeyValuePair<string, string>> outputInstructions)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            outputInstructions = new Dictionary<string, string>();
            ((Dictionary<string, string>) outputInstructions).Add("ttl", SubscriptionTtl.ToString());

            try
            {
                string ipstring = null, portstring = null;
                foreach (KeyValuePair<string, string> kvp in inputInstructions)
                {
                    if (kvp.Key.Equals("ip"))
                        ipstring = kvp.Value;
                    if (kvp.Key.Equals("port"))
                        portstring = kvp.Value;
                }

                if (string.IsNullOrEmpty(ipstring))
                    throw new TransportException("Field \"ip\" is required for subscription");
                int port;
                if (!int.TryParse(portstring, NumberStyles.Integer, CultureInfo.InvariantCulture, out port))
                    throw new TransportException("Invalid port number");
                if (port < 1 || port > 65535)
                    throw new TransportException("Invalid port number");

                IPAddress client;
                if (!IPAddress.TryParse(ipstring, out client))
                    throw new TransportException("Invalid IP address");

                string clientid = ipstring + ":" + port;
                UdpClientExpire newClient = new UdpClientExpire
                                                {Client = new UdpClient(ipstring, port), LastRefresh = DateTime.Now};
                _listlock.AcquireWriterLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    _clients.Add(clientid, newClient);
                }
                finally
                {
                    _listlock.ReleaseWriterLock();
                }

                return clientid;
            }
            catch (TransportException ex)
            {
                ex.Data["input"] = inputInstructions;
                throw;
            }
            catch (Exception ex)
            {
                TransportException e = new TransportException("Unable to subscribe client", ex);
                if (inputInstructions != null)
                    e.Data["input"] = inputInstructions;

                throw e;
            }
        }

        /// <summary>
        /// Implements IOutboundTransport.RequiresRefres
        /// </summary>
        public bool RequiresRefresh
        {
            get { return true; }
        }

        /// <summary>
        /// Implements IOutboundTransport.RefreshClient
        /// </summary>
        public void RefreshClient(string clientId)
        {
            _listlock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                if (_clients.ContainsKey(clientId))
                    _clients[clientId].LastRefresh = DateTime.Now;
                else
                    throw new ClientNotSubscribedException();
            }
            finally
            {
                _listlock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Implements IOutboundTransport.UnsubscribeClient
        /// </summary>
        public void UnsubscribeClient(string clientId)
        {
            _listlock.AcquireReaderLock(DEFAULT_JOIN_TIMEOUT);
            try
            {
                if (!_clients.ContainsKey(clientId))
                    throw new ClientNotSubscribedException();

                LockCookie ck = _listlock.UpgradeToWriterLock(DEFAULT_JOIN_TIMEOUT);
                try
                {
                    UdpClientExpire client = _clients[clientId];
                    _clients.Remove(clientId);

                    client.Client.Close();
                }
                catch (Exception ex)
                {
                    throw new TransportException("Unable to unsubscribe client", ex);
                }
                finally
                {
                    _listlock.DowngradeFromWriterLock(ref ck);
                }
            }
            finally
            {
                _listlock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Implements IOutboundTransport.SubscriptionTtl
        /// </summary>
        public long SubscriptionTtl { get; set; }

        #endregion

        #region IDisposable Membri di

        /// <summary>
        /// Implements IDisposable.Dispose
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            _disposed = true;

            _clearTimer.Dispose();
            if (disposing)
            {
                foreach (KeyValuePair<string, UdpClientExpire> kvp in _clients)
                    if (kvp.Value != null && kvp.Value.Client != null)
                        try
                        {
                            kvp.Value.Client.Close();
                        }
                        catch (SocketException)
                        {
                        }
            }
        }

        #endregion

        /// <summary>
        /// Support class for holding the last refresh of every client
        /// </summary>
        private class UdpClientExpire
        {
            public UdpClientExpire()
            {
                LastRefresh = DateTime.Now;
            }

            /// <summary>
            /// UDP socket to client
            /// </summary>
            public UdpClient Client;

            /// <summary>
            /// Last time client invoked RefreshSubscription
            /// </summary>
            public DateTime LastRefresh;
        }

        #region ILogSupport Membri di

        public ILog Log { private get; set; }

        #endregion
    }
}