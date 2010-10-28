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

namespace It.Unina.Dis.Logbus.OutTransports
{
    /// <summary>
    /// Transport exception specific to the event that a client is not subscribed to the transport
    /// </summary>
    [Serializable]
    public class ClientNotSubscribedException
        : TransportException
    {
        /// <summary>
        /// Initializes a new instance of ClientNotSubscribedException
        /// </summary>
        public ClientNotSubscribedException()
            : base("Client not subscribed")
        {
        }

        /// <summary>
        /// Initializes a new instance of ClientNotSubscribedException
        /// </summary>
        /// <param name="innerException">Exception that caused ClientNotSubscribedException</param>
        public ClientNotSubscribedException(Exception innerException)
            : base("Client not subscribed", innerException)
        {
        }
    }
}