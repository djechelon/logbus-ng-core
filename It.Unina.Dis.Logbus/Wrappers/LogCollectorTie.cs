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

namespace It.Unina.Dis.Logbus.Wrappers
{
    /// <summary>
    /// Apparently useless, this class implements delegation over a Log collector
    /// </summary>
    public sealed class LogCollectorTie
        : ILogCollector
    {
        /// <summary>
        /// Initializes LogCollectorTie with a target object
        /// </summary>
        /// <param name="target">Object to proxy</param>
        /// <exception cref="System.ArgumentNullException">Argument is null</exception>
        public LogCollectorTie(ILogCollector target)
        {
            if (target == null) throw new ArgumentNullException("target");
            Target = target;
        }

        /// <summary>
        /// Object to tie
        /// </summary>
        public ILogCollector Target { get; private set; }

        #region ILogCollector Membri di

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            Target.SubmitMessage(message);
        }

        #endregion
    }
}