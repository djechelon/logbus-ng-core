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

using System.Runtime.Serialization;
using System;
namespace It.Unina.Dis.Logbus
{
    public class TransportException
        : LogbusException
    {
        public TransportException()
            : base() { }

        public TransportException(string message)
            : base(message) { }

        protected TransportException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public TransportException(string message, Exception innerException)
            : base(message, innerException) { }

    }
}
