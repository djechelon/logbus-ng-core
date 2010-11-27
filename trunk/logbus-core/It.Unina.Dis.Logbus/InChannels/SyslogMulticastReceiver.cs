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
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace It.Unina.Dis.Logbus.InChannels
{
    /// <summary>
    /// Receives Syslog messages from a Multicast UDP socket
    /// </summary>
    internal class SyslogMulticastReceiver
        : SyslogUdpReceiver
    {

        private IPAddress _group;

        #region Constructor
        /// <remarks/>
        public SyslogMulticastReceiver()
        { }

        public SyslogMulticastReceiver(IPAddress group, int port)
            : base(port)
        {
            MulticastGroup = group;
        }
        #endregion

        public IPAddress MulticastGroup
        {
            get { return _group; }
            set
            {
                if (value != null && (value.GetAddressBytes()[0] < 224 || value.GetAddressBytes()[0] > 239))
                    throw new ArgumentOutOfRangeException("value", value.ToString(), "Invalid Multicast address");

                _group = value;
            }
        }


        public override string GetConfigurationParameter(string key)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            switch (key)
            {
                case "group":
                    {
                        return (MulticastGroup != null) ? MulticastGroup.ToString() : null;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter not supported");
                    }
            }
        }

        /// <remarks>Available configuration parameters:
        /// <list type="string">
        /// <item><b>group</b>: Multicast group to join</item>
        /// <item><b>port</b>: UDP port to listen to. Default 514</item>
        /// </list>
        /// </remarks>
        public override void SetConfigurationParameter(string key, string value)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            switch (key)
            {
                case "ip":
                case "host":
                    {
                        throw new NotSupportedException("Configuration parameter not supported");
                    }
                case "group":
                    {
                        IPAddress addr;
                        if (!IPAddress.TryParse(value, out addr)) throw new ArgumentException("Invalid IP address");
                        MulticastGroup = addr;
                        break;
                    }
                default:
                    {
                        base.SetConfigurationParameter(key, value);
                        break;
                    }
            }
        }

        protected override UdpClient InitClient()
        {
            UdpClient ret = base.InitClient();
            ret.JoinMulticastGroup(MulticastGroup);
            return ret;
        }

        public override string ToString()
        {
            return string.Format("SyslogMulticastReceiver:{0}:{1}", MulticastGroup, Port.ToString(CultureInfo.InvariantCulture));
        }

    }
}