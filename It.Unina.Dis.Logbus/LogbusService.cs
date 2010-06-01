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
using System.Threading;
using It.Unina.Dis.Logbus.Filters;
using System.Collections;

namespace It.Unina.Dis.Logbus
{
    public class LogbusService: ILogBus//, ILogbusController
    {
        private Thread hubThread;
        private Queue<SyslogMessage> fifo_queue;

        //http://stackoverflow.com/questions/668440/handling-objectdisposedexception-correctly-in-an-idisposable-class-hierarchy
        protected bool Disposed
        {
            get;
            private set;
        }

        #region Constructor/destructor

        public LogbusService()
        { 
        }

        ~LogbusService()
        {
            Dispose(false);
        }
        #endregion

        #region ILogBus Membri di

        public virtual string[] GetAvailableTransports()
        {
            //return (TransportFactory != null) ? TransportFactory.GetAvailableTransports() : new string[0];
            throw new NotImplementedException();
        }

        public virtual IList<IOutboundChannel> OutboundChannels
        {
            get;
            protected set;
        }

        public virtual IList<IInboundChannel> InboundChannels
        {
            get;
            protected set;
        }

        public virtual IFilter MainFilter
        {
            get;
            set;
        }

        public virtual IOutboundTransportFactory TransportFactory
        {
            get;
            set;
        }

        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
        }

        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            Disposed = true;
        }
        #endregion
    }
}
