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

namespace It.Unina.Dis.Logbus.FFDA
{
    public enum FFDAEvent : byte
    {
        /// <summary>
        /// Service Up
        /// </summary>
        SUP,

        /// <summary>
        /// Service Shutdown
        /// </summary>
        SDW,

        /// <summary>
        /// Service Start
        /// </summary>
        SST,

        /// <summary>
        /// Service End
        /// </summary>
        SEN,

        /// <summary>
        /// Entity Interaction Start
        /// </summary>
        EIS,

        /// <summary>
        /// Entity Interaction End
        /// </summary>
        EIE,

        /// <summary>
        /// Resource Interaction Start
        /// </summary>
        RIS,

        /// <summary>
        /// Resource Interaction End
        /// </summary>
        RIE,

        /// <summary>
        /// Complaint
        /// </summary>
        CMP
    }
}
