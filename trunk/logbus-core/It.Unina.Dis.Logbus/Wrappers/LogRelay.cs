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
    /// Utility class that emulates a relay.
    /// From one side, it collects messages as <see cref="It.Unina.Dis.Logbus.ILogCollector"/>, and from
    /// the other side it forwards them as <see cref="It.Unina.Dis.Logbus.ILogSource"/>
    /// </summary>
    public class LogRelay
        : MarshalByRefObject, ILogCollector, ILogSource
    {
        #region ILogCollector Membri di

        /// <summary>
        /// Implements ILogCollector.SubmitMessage
        /// </summary>
        public void SubmitMessage(SyslogMessage message)
        {
            if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs(message));
        }

        #endregion

        #region ILogSource Membri di

        /// <summary>
        /// Implements ILogSource.MessageReceived
        /// </summary>
        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        #endregion

        /// <remarks/>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}