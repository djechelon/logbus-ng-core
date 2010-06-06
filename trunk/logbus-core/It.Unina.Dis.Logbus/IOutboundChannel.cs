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

using It.Unina.Dis.Logbus.Filters;
using System;
namespace It.Unina.Dis.Logbus
{
    public interface IOutboundChannel
        : IDisposable
    {
        string ID { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        void SubmitMessage(SyslogMessage message);

        int SubscribedClients { get; }

        void Start();

        void Stop();

        IFilter Filter { get; set; }

        ulong CoalescenceWindowMillis { get; set; }

        IOutboundTransportFactory TransportFactory { get; set; }
    }
}
