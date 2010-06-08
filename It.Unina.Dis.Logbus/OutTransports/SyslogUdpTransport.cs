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

namespace It.Unina.Dis.Logbus.OutTransports
{
    internal sealed class SyslogUdpTransport
        : IOutboundTransport
    {

        #region Constructor
        public SyslogUdpTransport()
        {
            Disposed = false;
            Clients = new List<UdpClient>();
        }

        ~SyslogUdpTransport()
        {
            Dispose(false);
        }
        #endregion

        #region Support properties#
        private List<UdpClient> Clients
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

        private UdpClient FindClient(string clientid)
        {
            throw new NotImplementedException();
        }

        #region IOutboundTransport Membri di

        public void SubmitMessage(SyslogMessage message)
        {
            byte[] dgram = message.ToByteArray();

            foreach (UdpClient client in Clients)
            {
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

        public int ClientsSubscribed
        {
            get { return Clients.Count; }
        }

        public string SubscribeClient(IEnumerable<KeyValuePair<string, string>> inputInstructions, out IEnumerable<KeyValuePair<string, string>> outputInstructions)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            outputInstructions = null;

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

                UdpClient new_client = new UdpClient(ipstring, port);
                Clients.Add(new_client);

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
            throw new NotImplementedException();
        }

        public void UnsubscribeClient(string clientId)
        {
            throw new NotImplementedException();
        }

        public int SubscriptionTtl
        {
            get;
            private set;
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
                foreach (UdpClient client in Clients)
                {
                    try
                    {
                        client.Close();
                    }
                    catch (SocketException) { }
                }
            }
            Disposed = true;
        }
        #endregion

    }
}
