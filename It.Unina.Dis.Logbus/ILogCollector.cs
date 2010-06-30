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

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Wraps message-collection functions of Logbus.
    /// This interface abstracts the message submission functions of Logbus or its APIs
    /// </summary>
    public interface ILogCollector
    {

        /// <summary>
        /// Submits a Syslog message to the Log bus for possible forwarding
        /// </summary>
        /// <param name="message">Syslog message to forward to clients</param>
        /// <remarks>Message will be actually forwarded only if it matches the filter</remarks>
        /// <exception cref="LogbusException"></exception>
        void SubmitMessage(SyslogMessage message);
    }
}
