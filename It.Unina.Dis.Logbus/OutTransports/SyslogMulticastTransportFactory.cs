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
    [Design.TransportFactory("multicast", Name = "Multicast RFC5426 transport", Description = "Syslog over Multicast/UDP")]
    class SyslogMulticastTransportFactory
        : IOutboundTransportFactory
    {

        #region IOutboundTransportFactory Membri di

        public IOutboundTransport CreateTransport()
        {
            throw new System.NotImplementedException();
        }

        public string GetConfigurationParameter(string key)
        {
            throw new System.NotImplementedException();
        }

        public void SetConfigurationParameter(string key, string value)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Configuration
        {
            set { throw new System.NotImplementedException(); }
        }

        #endregion
    }
}
