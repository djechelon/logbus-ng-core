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

namespace It.Unina.Dis.Logbus
{
    public class SyslogMessageEventArgs : EventArgs
    {
        public SyslogMessage Message
        {
            get;
            set;
        }


        public SyslogMessageEventArgs() : base() { }

        public SyslogMessageEventArgs(SyslogMessage msg)
            : base()
        {
            Message = msg;
        }
    }
}
