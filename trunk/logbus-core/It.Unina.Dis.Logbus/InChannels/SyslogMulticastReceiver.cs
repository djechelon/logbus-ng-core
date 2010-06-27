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
namespace It.Unina.Dis.Logbus.InChannels
{
    public class SyslogMulticastReceiver
        :IInboundChannel
    {

        #region IInboundChannel Membri di

        public string Name
        {
            get;
            set;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        public System.Collections.Generic.IDictionary<string, string> Configuration
        {
            get { throw new NotImplementedException(); }
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
