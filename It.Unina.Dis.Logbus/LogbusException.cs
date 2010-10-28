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
using System.Runtime.Serialization;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Root class for Logbus exceptions
    /// </summary>
    public class LogbusException
        : Exception
    {
        /// <summary>
        /// Initializes LogbusException
        /// </summary>
        public LogbusException()
        {
        }

        /// <summary>
        /// Initializes LogbusException
        /// </summary>
        /// <param name="message">Text message for exception</param>
        public LogbusException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes LogbusException
        /// </summary>
        /// <param name="info">Serialization information</param>
        /// <param name="context">Serialization streaming context</param>
        protected LogbusException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes LogbusException
        /// </summary>
        /// <param name="message">Text message</param>
        /// <param name="innerException">Exception that caused this to occur</param>
        public LogbusException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}