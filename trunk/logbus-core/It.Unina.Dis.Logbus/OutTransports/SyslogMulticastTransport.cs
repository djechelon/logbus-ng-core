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
    class SyslogMulticastTransport
        : IOutboundTransport
    {
        #region IOutboundTransport Membri di

        public void SubmitMessage(SyslogMessage message)
        {
            throw new System.NotImplementedException();
        }

        public int SubscribedClients
        {
            get { throw new System.NotImplementedException(); }
        }

        public string SubscribeClient(System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> inputInstructions, out System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> outputInstructions)
        {
            throw new System.NotImplementedException();
        }

        public bool RequiresRefresh
        {
            get { throw new System.NotImplementedException(); }
        }

        public int SubscriptionTtl
        {
            get { throw new System.NotImplementedException(); }
        }

        public void RefreshClient(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public void UnsubscribeClient(string clientId)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
