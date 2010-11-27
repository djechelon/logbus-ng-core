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
using It.Unina.Dis.Logbus.Loggers;

namespace It.Unina.Dis.Logbus.FieldFailureData
{
    internal sealed class FieldFailureAlerter
        : SimpleLogImpl, IFieldFailureAlerter
    {
        #region Constructor

        public FieldFailureAlerter(ILogCollector target)
            : base(target)
        {
        }

        #endregion

        #region IFFDAMonitor Membri di

        public void LogCOA()
        {
            Log("COA", SyslogSeverity.Alert);
        }

        public void LogCOA(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("COA-" + id, SyslogSeverity.Alert);
        }

        public void LogEIA()
        {
            Log("EIA", SyslogSeverity.Alert);
        }

        public void LogEIA(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("EIA-" + id, SyslogSeverity.Alert);
        }

        public void LogRIA()
        {
            Log("RIA", SyslogSeverity.Alert);
        }

        public void LogRIA(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("RIA-" + id, SyslogSeverity.Alert);
        }

        #endregion
    }
}