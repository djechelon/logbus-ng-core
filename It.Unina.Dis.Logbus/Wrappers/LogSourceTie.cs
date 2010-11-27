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
    /// Apparently useless, this class implements delegation over a Log source
    /// </summary>
    public sealed class LogSourceTie
        : MarshalByRefObject, ILogSource
    {
        /// <summary>
        /// Initializes LogSourceTie with the log source
        /// </summary>
        /// <param name="target">Log source to proxy</param>
        public LogSourceTie(ILogSource target)
        {
            if (target == null) throw new ArgumentNullException("target");
            Target = target;
            Target.MessageReceived += Target_MessageReceived;
        }

        private void Target_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            if (MessageReceived != null) MessageReceived(sender, e);
        }

        /// <summary>
        /// Delegated log source
        /// </summary>
        public ILogSource Target { get; private set; }

        #region ILogSource Membri di

        /// <remarks/>
        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        #endregion

        /// <remarks/>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}