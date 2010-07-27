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
using System.Collections.Generic;
namespace It.Unina.Dis.Logbus.OutTransports
{
    [Design.TransportFactory("udp", Name = "RFC5426 transport", Description = "Syslog over UDP transport according to RFC5426")]
    internal sealed class SyslogUdpTransportFactory
        : IOutboundTransportFactory
    {

        private long defaultTTL = 20000;

        #region IOutboundTransportFactory Membri di

        IOutboundTransport IOutboundTransportFactory.CreateTransport()
        {
            return new SyslogUdpTransport(defaultTTL);
        }

        string IConfigurable.GetConfigurationParameter(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            switch (key)
            {
                case "ttl":
                    {
                        return defaultTTL.ToString(CultureInfo.InvariantCulture);
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter not supported by UDP transport");
                    }
            }
        }

        void IConfigurable.SetConfigurationParameter(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            switch (key)
            {
                case "ttl":
                    {
                        if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out defaultTTL)) throw new InvalidOperationException("Invalid value for ttl");
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter not supported by UDP transport");
                    }
            }
        }

        System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> IConfigurable.Configuration
        {
            set
            {
                foreach (KeyValuePair<string, string> kvp in value) ((IConfigurable)this).SetConfigurationParameter(kvp.Key, kvp.Value);
            }
        }

        #endregion
    }
}
