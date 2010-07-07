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
using It.Unina.Dis.Logbus.Api;
using System.Net;
using System.Diagnostics;
using System.Globalization;

namespace It.Unina.Dis.Logbus.Utils
{
    public class FFDALogger
    {
        /// <summary>
        /// Enterprise ID for StructuredData
        /// </summary>
        private const string ENTERPRISE_ID = "8289";

        public FFDALogger(SyslogFacility facility, ILogCollector target)
        {
            if (target == null) throw new ArgumentNullException("target");

            Facility = facility;
            Target = target;
        }

        public FFDALogger(ILogCollector target)
            : this(SyslogFacility.Local0, target) { }

        private SyslogFacility Facility { get; set; }
        private ILogCollector Target { get; set; }

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

        public void LogCOA()
        {
            Log("COA", SyslogSeverity.Error);
        }

        public void LogCOA(string id)
        {
            if (id != null)
                Log("COA-" + id, SyslogSeverity.Error);
            else
                Log("COA", SyslogSeverity.Error);
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

        protected void Log(string message, SyslogSeverity severity)
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
                MessageId = "FFDA",
                ApplicationName = appname
            };

            // Getting the caller information(note that index is 2 because of Log is called by another local Method... 
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            msg.Data = new Dictionary<String, IDictionary<String, String>>();
            msg.Data.Add("CallerData@" + ENTERPRISE_ID, new Dictionary<String, String>());
            msg.Data["CallerData@" + ENTERPRISE_ID].Add("ClassName", stackFrames[2].GetMethod().DeclaringType.FullName);
            msg.Data["CallerData@" + ENTERPRISE_ID].Add("MethodName", stackFrames[2].GetMethod().Name);
            
            Target.SubmitMessage(msg);
        }
    }
}
