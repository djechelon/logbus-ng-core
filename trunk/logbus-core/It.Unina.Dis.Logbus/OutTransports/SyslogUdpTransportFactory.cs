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

namespace It.Unina.Dis.Logbus.OutTransports
{
    [Design.TransportFactory("udp", Name = "RFC5426 transport", Description = "Syslog over UDP transport according to RFC5426")]
    class SyslogUdpTransportFactory
        : IOutboundTransportFactory
    {

        #region IOutboundTransportFactory Membri di

        IOutboundTransport IOutboundTransportFactory.CreateTransport()
        {
            return new SyslogUdpTransport()
            {
                SubscriptionTtl = 10000
            };
        }

        string IConfigurable.GetConfigurationParameter(string key)
        {
            throw new System.NotSupportedException("Configuration not supported by UDP transport");
        }

        void IConfigurable.SetConfigurationParameter(string key, string value)
        {
            throw new System.NotSupportedException("Configuration not supported by UDP transport");
        }

        System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> IConfigurable.Configuration
        {
            set { throw new System.NotSupportedException("Configuration not supported by UDP transport"); }
        }

        #endregion
    }
}
