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
using System.Text;
namespace It.Unina.Dis.Logbus.Loggers
{
    internal class SyslogUdpLogger
        : ILogCollector, IDisposable
    {

        #region Constrcutor
        public SyslogUdpLogger(string ip, int port)
            : this(new IPEndPoint(IPAddress.Parse(ip), port))
        {
        }

        public SyslogUdpLogger(IPAddress ip, int port)
            : this(new IPEndPoint(ip, port))
        {
        }

        public SyslogUdpLogger(IPEndPoint endpoint)
        {
            RemoteEndPoint = endpoint;
            client = new UdpClient();
        }

        ~SyslogUdpLogger()
        {
            Dispose(false);
        }
        #endregion

        public IPEndPoint RemoteEndPoint
        {
            get;
            set;
        }

        private UdpClient client;

        #region ILogCollector Membri di

        public void SubmitMessage(SyslogMessage message)
        {
            byte[] payload = Encoding.UTF8.GetBytes(message.ToRfc5424String());
            try
            {
                client.Send(payload, payload.Length, RemoteEndPoint);
            }
            catch (Exception ex)
            {
                throw new LogbusException("Unable to send Syslog message", ex);
            }
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            if (disposing && client != null) client.Close();
            
            client = null;
            RemoteEndPoint = null;
        }
        #endregion
    }
}
