﻿/*
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
using It.Unina.Dis.Logbus.Filters;
namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// This interface represents the Logbus core service
    /// </summary>
    public interface ILogBus
        : IDisposable
    {
        string[] GetAvailableTransports();

        IList<IOutboundChannel> OutboundChannels { get; }

        IList<IInboundChannel> InboundChannels { get; }

        ITransportFactoryHelper TransportFactoryHelper { get; set; }

        void Start();

        void Stop();

        IFilter MainFilter { get; set; }
    }
}
