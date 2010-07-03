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
namespace It.Unina.Dis.Logbus.OutTransports
{
    class SimpleTransportHelper
        : Dictionary<string, IOutboundTransportFactory>, ITransportFactoryHelper
    {

        #region ITransportFactoryHelper Membri di

        public SimpleTransportHelper()
            : base()
        {
            base.Add("udp", new OutTransports.SyslogUdpTransportFactory());
        }

        IOutboundTransportFactory ITransportFactoryHelper.this[string transportId]
        {
            get
            {
                return this[transportId];
            }
            set
            {
                this[transportId] = value;
            }
        }

        IOutboundTransportFactory ITransportFactoryHelper.GetFactory(string transportId)
        {
            return this[transportId];
        }

        void ITransportFactoryHelper.AddFactory(string transportId, IOutboundTransportFactory factory)
        {
            this.Add(transportId, factory);
        }

        void ITransportFactoryHelper.RemoveFactory(string transportId)
        {
            if (!this.Remove(transportId)) throw new LogbusException("Object not found");
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
    }
}
