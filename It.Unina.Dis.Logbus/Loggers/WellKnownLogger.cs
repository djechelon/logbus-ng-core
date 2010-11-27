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

namespace It.Unina.Dis.Logbus.Loggers
{
    /// <summary>
    /// Lists the well-known Logbus-ng loggers, that can be mapped by default rules
    /// </summary>
    public enum WellKnownLogger
    {
        /// <summary>
        /// Logger for internal Logbus-ng entities. Logs as Syslog-Internal right to current Logbus instance
        /// </summary>
        Logbus,

        /// <summary>
        /// Logger for clients. Defaults logs onto console
        /// </summary>
        Client,

        /// <summary>
        /// Logger for collectors. Default logs onto console as Syslog-Internal
        /// </summary>
        CollectorInternal
    }
}