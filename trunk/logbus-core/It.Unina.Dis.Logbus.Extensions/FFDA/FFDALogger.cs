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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using It.Unina.Dis.Logbus.Loggers;

namespace It.Unina.Dis.Logbus.FFDA
{
    
    internal sealed class FFDALogger
        : SimpleLogImpl, IFFDALogger
    {
        #region Constructor
        /// <summary>
        /// Initializes the FFDA logger with given Syslog facility and concrete logger
        /// </summary>
        /// <param name="facility">Syslog facility that will be used for all the messages</param>
        /// <param name="target">Concrete logger that will collect FFDA messages</param>
        public FFDALogger(SyslogFacility facility, ILogCollector target)
            : base(facility, target)
        { }

        /// <summary>
        /// Initializes the FFDA logger with the concrete underlying logger
        /// </summary>
        /// <param name="target">Concrete logger that will collect FFDA messages</param>
        public FFDALogger(ILogCollector target)
            : base(SyslogFacility.Local0, target) { }

        #endregion

        
        public void LogSST()
        {
            Log("SST", SyslogSeverity.Info);
        }

        
        public void LogSST(string id)
        {
            if (id != null)
                Log("SST-" + id, SyslogSeverity.Info);
            else
                Log("SST", SyslogSeverity.Info);
        }

        
        public void LogSEN()
        {
            Log("SEN", SyslogSeverity.Info);
        }

        
        public void LogSEN(string id)
        {
            if (id != null)
                Log("SEN-" + id, SyslogSeverity.Info);
            else
                Log("SEN", SyslogSeverity.Info);
        }

        
        public void LogEIS()
        {
            Log("EIS", SyslogSeverity.Info);
        }

        
        public void LogEIS(string id)
        {
            if (id != null)
                Log("EIS-" + id, SyslogSeverity.Info);
            else
                Log("EIS", SyslogSeverity.Info);
        }

       
        public void LogEIE()
        {
            Log("EIE", SyslogSeverity.Info);
        }

        
        public void LogEIE(string id)
        {
            if (id != null)
                Log("EIE-" + id, SyslogSeverity.Info);
            else
                Log("EIE", SyslogSeverity.Info);
        }

        
        public void LogRIS()
        {
            Log("RIS", SyslogSeverity.Info);
        }

        
        public void LogRIS(string id)
        {
            if (id != null)
                Log("RIS-" + id, SyslogSeverity.Info);
            else
                Log("RIS", SyslogSeverity.Info);
        }

        
        public void LogRIE()
        {
            Log("RIE", SyslogSeverity.Info);
        }

        
        public void LogRIE(string id)
        {
            if (id != null)
                Log("RIE-" + id, SyslogSeverity.Info);
            else
                Log("RIE", SyslogSeverity.Info);
        }

        
        public void LogCMP()
        {
            Log("CMP", SyslogSeverity.Info);
        }

        
        public void LogCMP(string id)
        {
            if (id != null)
                Log("CMP-" + id, SyslogSeverity.Info);
            else
                Log("CMP", SyslogSeverity.Info);
        }

        
        public void LogCOA()
        {
            Log("COA", SyslogSeverity.Alert);
        }

        
        public void LogCOA(string id)
        {
            if (id != null)
                Log("COA-" + id, SyslogSeverity.Alert);
            else
                Log("COA", SyslogSeverity.Alert);
        }

    }
}