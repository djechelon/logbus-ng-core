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

using It.Unina.Dis.Logbus.FFDA;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Extensions for <see cref="It.Unina.Dis.Logbus.SyslogMessage"/>
    /// </summary>
    public static class SyslogMessageExtensions
    {
        /// <summary>
        /// Gets FFDA information, if message is FFD
        /// </summary>
        /// <param name="msg">Syslog message to parse</param>
        /// <returns>FFDA information about the message</returns>
        /// <exception cref="System.InvalidOperationException">Message is not FFD</exception>
        public static FFDAInformation GetFfdaInformation(this SyslogMessage msg)
        {
            return new FFDAInformation(msg);
        }
    }
}
