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
    internal sealed class SimpleOutChannelFactory
        :IOutboundChannelFactory, ILogSupport
    {

        #region IOutboundChannelFactory Membri di

        IOutboundChannel IOutboundChannelFactory.CreateChannel(string name, string description, Filters.IFilter filter)
        {
            IOutboundChannel ret = new SimpleOutChannel
                                       {
                                           Name = name,
                                           Description = description,
                                           Filter = filter,
                                           TransportFactoryHelper = TransportFactoryHelper,
                                           Log = Log
                                       };
            return ret;
        }

        public ITransportFactoryHelper TransportFactoryHelper
        {
            get;
            set;
        }

        #endregion

        #region ILogSupport Membri di

        public Loggers.ILog Log { private get; set; }

        #endregion
    }
}
