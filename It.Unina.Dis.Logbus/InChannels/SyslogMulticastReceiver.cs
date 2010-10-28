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

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<ParseErrorEventArgs> ParseError;

        #endregion

        #region ILogSource Membri di

        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        #endregion

        #region IRunnable Membri di

        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event UnhandledExceptionEventHandler Error;

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool Running
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IConfigurable Membri di

        public string GetConfigurationParameter(string key)
        {
            throw new NotImplementedException();
        }

        public void SetConfigurationParameter(string key, string value)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> Configuration
        {
            set { throw new NotImplementedException(); }
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
