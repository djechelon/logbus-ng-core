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
    /// <summary>
    /// Receives Syslog messages from a Multicast UDP socket
    /// </summary>
    internal class SyslogMulticastReceiver
        :IInboundChannel
    {

        /// <remarks/>
        public SyslogMulticastReceiver()
        {
            throw new NotImplementedException();
        }

        #region IInboundChannel Membri di

        /// <remarks/>
        public string Name
        {
            get;
            set;
        }

        /// <remarks/>
        public void Start()
        {
            throw new NotImplementedException();
        }

        /// <remarks/>
        public void Stop()
        {
            throw new NotImplementedException();
        }

        /// <remarks/>
        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        /// <remarks/>
        public event EventHandler<ParseErrorEventArgs> ParseError;

        /// <remarks/>
        public System.Collections.Generic.IDictionary<string, string> Configuration
        {
            get { throw new NotImplementedException(); }
        }

        /// <remarks/>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        /// <remarks/>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        /// <remarks/>
        public event EventHandler Started;

        /// <remarks/>
        public event EventHandler Stopped;

        /// <remarks/>
        public event UnhandledExceptionEventHandler Error;

        #endregion

        #region IDisposable Membri di

        /// <remarks/>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region IConfigurable Membri di

        /// <remarks/>
        public string GetConfigurationParameter(string key)
        {
            throw new NotImplementedException();
        }

        /// <remarks/>
        public void SetConfigurationParameter(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <remarks/>
        System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> IConfigurable.Configuration
        {
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}
