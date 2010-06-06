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

namespace It.Unina.Dis.Logbus.OutChannels
{
    public class SimpleOutChannelFactory
        :IOutboundChannelFactory
    {

        #region IOutboundChannelFactory Membri di

        IOutboundChannel IOutboundChannelFactory.CreateChannel(string name, string description, It.Unina.Dis.Logbus.Filters.IFilter filter)
        {
            IOutboundChannel ret = new SimpleOutChannel();
            ret.Name = name;
            ret.Description = description;
            ret.Filter = filter;
            return ret;
        }

        IOutboundTransportFactory IOutboundChannelFactory.TransportFactory
        {
            get;
            set;
        }

        #endregion
    }
}
