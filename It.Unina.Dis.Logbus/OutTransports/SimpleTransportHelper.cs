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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace It.Unina.Dis.Logbus.OutTransports
{
    /// <summary>
    /// Factory-of-factory for Outbound Transports
    /// </summary>
    internal class SimpleTransportHelper
        : Dictionary<string, IOutboundTransportFactory>, ITransportFactoryHelper, ILogSupport
    {

        #region ITransportFactoryHelper Membri di

        public SimpleTransportHelper()
        {
            Add("udp", new SyslogUdpTransportFactory());
            Add("tls", new SyslogTlsTransportFactory());
        }

        IOutboundTransportFactory ITransportFactoryHelper.GetFactory(string transportId)
        {
            return this[transportId];
        }

        void ITransportFactoryHelper.AddFactory(string transportId, IOutboundTransportFactory factory)
        {
            Add(transportId, factory);
        }

        void ITransportFactoryHelper.RemoveFactory(string transportId)
        {
            if (!Remove(transportId)) throw new LogbusException("Transport factory not found");
        }

        string[] ITransportFactoryHelper.AvailableTransports
        {
            get
            {
                string[] ret = new string[Keys.Count];
                Keys.CopyTo(ret, 0);
                return ret;
            }
        }

        #endregion


        #region ILogSupport Membri di

        Loggers.ILog ILogSupport.Log
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                foreach (IOutboundTransportFactory factory in Values)
                {
                    if (factory is ILogSupport) ((ILogSupport)factory).Log = value;
                }
            }
        }

        #endregion
    }
}
