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
namespace It.Unina.Dis.Logbus.Channels
{
    class SimpleOutChannelFactory
        :IOutboundChannelFactory
    {
        #region IOutboundChannelFactory Membri di

        IOutboundChannel IOutboundChannelFactory.CreateChannel(string name, string description, It.Unina.Dis.Logbus.Filters.IFilter filter)
        {
            if (((IOutboundChannelFactory)this).TransportHelper == null) throw new NotSupportedException("Transport factory must be set before creating new channels");
            throw new System.NotImplementedException();
        }

        ITransportFactoryHelper IOutboundChannelFactory.TransportHelper
        {
            get;
            set;
        }

        #endregion
    }
}
