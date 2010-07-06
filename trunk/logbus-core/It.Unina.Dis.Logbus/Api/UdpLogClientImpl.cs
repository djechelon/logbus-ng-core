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

using System.Timers;
using It.Unina.Dis.Logbus.InChannels;
namespace It.Unina.Dis.Logbus.Api
{
    internal sealed class UdpLogClientImpl
        : ILogClient
    {

        private IChannelSubscription Subcriber { get; set; }
        private Timer refresh_timer;
        private SyslogUdpReceiver Receiver { get; set; }

        #region Constructor/Destructor

        public UdpLogClientImpl(string channel_id, string LogbusEndpointUrl)
        {
        }

        ~UdpLogClientImpl()
        {
        }

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

        #endregion

        #region ILogSource Membri di

        public event SyslogMessageEventHandler MessageReceived;

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
