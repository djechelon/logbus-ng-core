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
    /// Wraps log source functions.
    /// This interface shows Logbus's log broadcasting functions to other members
    /// </summary>
    public interface ILogSource
    {

        /// <summary>
        /// A new message is available on the bus for retrieval and processing
        /// </summary>
        event SyslogMessageEventHandler MessageReceived;

    }
}
