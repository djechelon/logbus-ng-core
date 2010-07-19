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

using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace It.Unina.Dis.Logbus.OutTransports
{
    internal sealed class SyslogUdpTransport
        : IOutboundTransport
    {

        #region Constructor
        public SyslogUdpTransport()
        {
            Disposed = false;
            Clients = new Dictionary<String, UdpClientExpire>();
            clear_timer = new Timer(ClearList, null, SubscriptionTtl, SubscriptionTtl);
        }

        ~SyslogUdpTransport()
        {
            Dispose(false);
        }
        #endregion

        #region Support properties#
        private Dictionary<String, UdpClientExpire> Clients
        {
            get;
            set;
        }

        private bool Disposed
        {
            get;
            set;
        }
        #endregion

        private Timer clear_timer;

        private UdpClient FindClient(string clientid)
        {
            if (Clients.ContainsKey(clientid))
                return Clients[clientid].Client;
            else
                return null;
        }

        #region IOutboundTransport Membri di

        private void ClearList(Object Status)
        {
            lock (Clients)
            {
                Dictionary<String, UdpClientExpire>.Enumerator enumeratore = Clients.GetEnumerator();
                while (enumeratore.MoveNext())
                {
                    DateTime expire = enumeratore.Current.Value.LastRefresh.Value;
                    TimeSpan diff = DateTime.Now.Subtract(expire);
                    if (diff.TotalMilliseconds > SubscriptionTtl)
                        UnsubscribeClient(enumeratore.Current.Key);
                }
            }
        }

        public void SubmitMessage(SyslogMessage message)
        {
            byte[] dgram = message.ToByteArray();
            lock (Clients)
            {
                Dictionary<String, UdpClientExpire>.Enumerator enumeratore = Clients.GetEnumerator();

                while (enumeratore.MoveNext())
                {
                    UdpClient client = enumeratore.Current.Value.Client;
                    if (client != null)
                        try
                        {
                            client.Send(dgram, dgram.Length);
                        }
                        catch (SocketException)
                        {
                            //What to do????
                            //For now, ignore and lose message
                        }
                }
            }
        }

        public int SubscribedClients
        {
            get { return Clients.Count; }
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
        public string SubscribeClient(IEnumerable<KeyValuePair<string, string>> inputInstructions, out IEnumerable<KeyValuePair<string, string>> outputInstructions)
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().FullName);

            outputInstructions = new Dictionary<string, string>();
            ((Dictionary<string, string>)outputInstructions).Add("ttl", this.SubscriptionTtl.ToString());

            try
            {
                string ipstring = null, portstring = null;
                foreach (KeyValuePair<string, string> kvp in inputInstructions)
                {
                    if (kvp.Key.Equals("ip")) ipstring = kvp.Value;
                    if (kvp.Key.Equals("port")) portstring = kvp.Value;
                }

                if (string.IsNullOrEmpty(ipstring)) throw new TransportException("Field \"ip\" is required for subscription");
                int port;
                if (!int.TryParse(portstring, NumberStyles.Integer, CultureInfo.InvariantCulture, out port)) throw new TransportException("Invalid port number");
                if (port < 1 || port > 65535) throw new TransportException("Invalid port number");

                IPAddress client;
                if (!IPAddress.TryParse(ipstring, out client)) throw new TransportException("Invalid IP address");

                string clientid = ipstring + ":" + port;
                UdpClientExpire new_client = new UdpClientExpire() { Client = new UdpClient(ipstring, port), LastRefresh = DateTime.Now };
                lock (Clients)
                    Clients.Add(clientid, new_client);

                return clientid;
            }

            catch (TransportException ex)
            {
                ex.Data["input"] = inputInstructions;
                throw ex;
            }
            catch (Exception ex)
            {
                TransportException e = new TransportException("Unable to subscribe client", ex);
                if (inputInstructions != null)
                    e.Data["input"] = inputInstructions;

                throw e;
            }
        }

        public bool RequiresRefresh
        {
            get { return true; }
        }

        public void RefreshClient(string clientId)
        {
            lock (Clients)
                if (Clients.ContainsKey(clientId))
                    Clients[clientId].LastRefresh = DateTime.Now;
                else
                    throw new TransportException("Client not subscribed");
        }

        public void UnsubscribeClient(string clientId)
        {
            lock (Clients)
            {
                try
                {
                    Clients[clientId].Client.Close();
                    Clients.Remove(clientId);
                }
                catch (Exception ex)
                {
                    throw new TransportException("Unable to unsubscribe client", ex);
                }
            }
        }

        public long SubscriptionTtl
        {
            get;
            protected internal set;
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            if (disposing)
            {
                lock (Clients)
                {
                    Dictionary<String, UdpClientExpire>.Enumerator enumeratore = Clients.GetEnumerator();
                    while (enumeratore.MoveNext())
                    {
                        UdpClient client = enumeratore.Current.Value.Client;
                        if (client != null)
                            try
                            {
                                client.Close();
                            }
                            catch (SocketException) { }
                    }
                }
            }
            Disposed = true;
        }
        #endregion

    }
}
