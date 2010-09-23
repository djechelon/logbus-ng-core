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

namespace It.Unina.Dis.Logbus.Collectors
{
    /// <summary>
    /// Forwards a message to multiple loggers
    /// </summary>
    internal class MultiCollector
        : ILogCollector
    {

        public ILogCollector[] Collectors
        {
            get;
            set;
        }

        #region ILogCollector Membri di

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            if (Collectors == null) return;
            foreach (ILogCollector collector in Collectors)
                collector.SubmitMessage(message);
        }

        #endregion
    }
}
