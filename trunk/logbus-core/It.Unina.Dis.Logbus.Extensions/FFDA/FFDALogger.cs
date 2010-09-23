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
    /// <summary>
    /// Default implementation of IFFDALogger and IFFDAMonitor
    /// </summary>
    internal sealed class FFDALogger
        : SimpleLogImpl, IFFDALogger, IFFDAMonitorLogger
    {
        #region Constructor/Destructor
        /// <summary>
        /// Initializes the FFDA logger with given Syslog facility and concrete logger
        /// </summary>
        /// <param name="facility">Syslog facility that will be used for all the messages</param>
        /// <param name="target">Concrete logger that will collect FFDA messages</param>
        public FFDALogger(SyslogFacility facility, ILogCollector target)
            : base(facility, target)
        {
            GC.SuppressFinalize(base.Target);
            Log("SUP", SyslogSeverity.Info);
        }

        /// <summary>
        /// Initializes the FFDA logger with the concrete underlying logger
        /// </summary>
        /// <param name="target">Concrete logger that will collect FFDA messages</param>
        public FFDALogger(ILogCollector target)
            : this(SyslogFacility.Local0, target) { }

        ~FFDALogger()
        {
            Dispose(false);
        }

        private volatile bool _disposed;

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            Log("SDW", SyslogSeverity.Info);

            if (disposing)
            {
                if (base.Target is IDisposable) ((IDisposable)base.Target).Dispose();
            }

            GC.ReRegisterForFinalize(base.Target);
        }
        #endregion

        private string GetFlowId()
        {
            if (Flow != null)
            {
                if (Flow is string) return (string)Flow;
                if (Flow is decimal || Flow is int || Flow is float || Flow is double || Flow is long) return Flow.ToString();
                return Flow.GetHashCode().ToString(CultureInfo.InvariantCulture);
            }
            return Thread.CurrentThread.GetHashCode().ToString(CultureInfo.InvariantCulture);
        }

        protected override void PreProcessMessage(ref SyslogMessage msg)
        {
            base.PreProcessMessage(ref msg);

            msg.MessageId = "FFDA";
        }

        #region IFFDALogger Membri di

        public object Flow { get; set; }

        public void LogSST()
        {
            Log("SST-" + GetFlowId(), SyslogSeverity.Info);
        }


        public void LogSST(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("SST-" + id, SyslogSeverity.Info);
        }


        public void LogSEN()
        {
            Log("SEN-" + GetFlowId(), SyslogSeverity.Info);
        }


        public void LogSEN(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("SEN-" + id, SyslogSeverity.Info);
        }


        public void LogEIS()
        {
            Log("EIS-" + GetFlowId(), SyslogSeverity.Info);
        }


        public void LogEIS(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("EIS-" + id, SyslogSeverity.Info);
        }


        public void LogEIE()
        {
            Log("EIE-" + GetFlowId(), SyslogSeverity.Info);
        }


        public void LogEIE(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("EIE-" + id, SyslogSeverity.Info);
        }


        public void LogRIS()
        {
            Log("RIS-" + GetFlowId(), SyslogSeverity.Info);
        }


        public void LogRIS(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("RIS-" + id, SyslogSeverity.Info);
        }


        public void LogRIE()
        {
            Log("RIE-" + GetFlowId(), SyslogSeverity.Info);
        }


        public void LogRIE(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("RIE-" + id, SyslogSeverity.Info);
        }


        public void LogCMP()
        {
            Log("CMP-" + GetFlowId(), SyslogSeverity.Info);
        }


        public void LogCMP(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            Log("CMP-" + id, SyslogSeverity.Info);
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