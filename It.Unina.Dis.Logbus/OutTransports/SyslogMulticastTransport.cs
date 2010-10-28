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

namespace It.Unina.Dis.Logbus.OutTransports
{
    internal class SyslogMulticastTransport
        : IOutboundTransport
    {
        #region IOutboundTransport Membri di

        public void SubmitMessage(SyslogMessage message)
        {
            throw new NotImplementedException();
        }

        public int SubscribedClients
        {
            get { throw new NotImplementedException(); }
        }

        public string SubscribeClient(IEnumerable<KeyValuePair<string, string>> inputInstructions,
                                      out IEnumerable<KeyValuePair<string, string>> outputInstructions)
        {
            throw new NotImplementedException();
        }

        public bool RequiresRefresh
        {
            get { throw new NotImplementedException(); }
        }

        public long SubscriptionTtl
        {
            get { throw new NotImplementedException(); }
        }

        public void RefreshClient(string clientId)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeClient(string clientId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}