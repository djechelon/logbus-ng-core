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
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
namespace It.Unina.Dis.Logbus.Loggers
{
    public class SimpleLogImpl
        : ILog
    {
        /// <summary>
        /// Enterprise ID for StructuredData
        /// As of IANA official Enteprise ID list
        /// 
        /// 8289
        ///     Universita' degli Studi di Napoli "Federico II"
        ///         Francesco Palmieri
        ///             fpalmieri&unina.it
        /// </summary>
        public const string ENTERPRISE_ID = "8289";

        public SimpleLogImpl(SyslogFacility facility, ILogCollector target)
        {
            if (target == null) throw new ArgumentNullException("target");

            Facility = facility;
            Target = target;
        }

        public SimpleLogImpl(ILogCollector target)
            : this(SyslogFacility.Local4, target) { }

        protected SyslogFacility Facility { get; set; }
        protected ILogCollector Target { get; set; }

        protected virtual void Log(string message, SyslogSeverity severity)
        {
            String host = Environment.MachineName;
            String procid = Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);
            String appname = Process.GetCurrentProcess().ProcessName;

            SyslogMessage msg = new SyslogMessage()
            {
                Severity = severity,
                Facility = Facility,
                Text = message,
                Timestamp = DateTime.Now,
                Host = host,
                ProcessID = procid,
                ApplicationName = appname
            };

            // Getting the caller information(note that index is 2 because of Log is called by another local Method... 
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            msg.Data = new Dictionary<String, IDictionary<String, String>>();
            msg.Data.Add("CallerData@"+ENTERPRISE_ID, new Dictionary<String, String>());
            msg.Data["CallerData@"+ENTERPRISE_ID].Add("ClassName", stackFrames[2].GetMethod().DeclaringType.FullName);
            msg.Data["CallerData@"+ENTERPRISE_ID].Add("MethodName", stackFrames[2].GetMethod().Name);
            
            Target.SubmitMessage(msg);
        }

        #region ILog Membri di

        void ILog.Debug(string message)
        {
            Log(message, SyslogSeverity.Debug);
        }

        void ILog.Info(string message)
        {
            Log(message, SyslogSeverity.Info);
        }

        void ILog.Notice(string message)
        {
            Log(message, SyslogSeverity.Notice);
        }

        void ILog.Warning(string message)
        {
            Log(message, SyslogSeverity.Warning);
        }

        void ILog.Error(string message)
        {
            Log(message, SyslogSeverity.Error);
        }

        void ILog.Critical(string message)
        {
            Log(message, SyslogSeverity.Critical);
        }

        void ILog.Alert(string message)
        {
            Log(message, SyslogSeverity.Alert);
        }

        void ILog.Emergency(string message)
        {
            Log(message, SyslogSeverity.Emergency);
        }

        #endregion
    }
}
