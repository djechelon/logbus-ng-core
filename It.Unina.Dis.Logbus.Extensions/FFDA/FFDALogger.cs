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
        : SimpleLogImpl, IInstrumentedLogger
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

        private volatile bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);

            Log("SDW", SyslogSeverity.Info);

            _disposed = true;
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

            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            IDictionary<string, string> callerData = msg.Data["CallerData@" + ENTERPRISE_ID];

            if (stackFrames != null && stackFrames.Length >= 5)
            {
                callerData["ClassName"] = stackFrames[4].GetMethod().DeclaringType.FullName;
                callerData["MethodName"] = stackFrames[4].GetMethod().Name;
                callerData["ModuleName"] = stackFrames[4].GetMethod().DeclaringType.Assembly.GetName().Name;
            }

            msg.MessageId = "FFDA";
        }

        #region IFFDALogger Membri di

        public object Flow { get; set; }

        public void LogSST()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Log("SST-" + GetFlowId(), SyslogSeverity.Info);
        }

        public void LogSST(string id)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (id == null)
                throw new ArgumentNullException("id");

            Log("SST-" + id, SyslogSeverity.Info);
        }

        public void LogSEN()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Log("SEN-" + GetFlowId(), SyslogSeverity.Info);
        }

        public void LogSEN(string id)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (id == null)
                throw new ArgumentNullException("id");

            Log("SEN-" + id, SyslogSeverity.Info);
        }

        public void LogEIS()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Log("EIS-" + GetFlowId(), SyslogSeverity.Info);
        }

        public void LogEIS(string id)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (id == null)
                throw new ArgumentNullException("id");

            Log("EIS-" + id, SyslogSeverity.Info);
        }

        public void LogEIE()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Log("EIE-" + GetFlowId(), SyslogSeverity.Info);
        }

        public void LogEIE(string id)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (id == null)
                throw new ArgumentNullException("id");

            Log("EIE-" + id, SyslogSeverity.Info);
        }

        public void LogRIS()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Log("RIS-" + GetFlowId(), SyslogSeverity.Info);
        }

        public void LogRIS(string id)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (id == null)
                throw new ArgumentNullException("id");

            Log("RIS-" + id, SyslogSeverity.Info);
        }

        public void LogRIE()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Log("RIE-" + GetFlowId(), SyslogSeverity.Info);
        }

        public void LogRIE(string id)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (id == null)
                throw new ArgumentNullException("id");

            Log("RIE-" + id, SyslogSeverity.Info);
        }

        public void LogCMP()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            Log("CMP-" + GetFlowId(), SyslogSeverity.Info);
        }

        public void LogCMP(string id)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (id == null)
                throw new ArgumentNullException("id");

            Log("CMP-" + id, SyslogSeverity.Info);
        }

        #endregion
    }
}