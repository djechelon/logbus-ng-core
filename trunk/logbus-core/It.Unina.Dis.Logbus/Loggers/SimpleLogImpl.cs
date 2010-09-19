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
namespace It.Unina.Dis.Logbus.Loggers
{
    /// <summary>
    /// Simple ILog implementation
    /// </summary>
    internal class SimpleLogImpl
        : ILog
    {
        /// <summary>
        /// Enterprise ID for StructuredData
        /// As of IANA official Enteprise ID list
        /// 
        /// 8289
        /// Universita' degli Studi di Napoli "Federico II"
        /// Francesco Palmieri
        /// fpalmieri@unina.it
        /// </summary>
        internal const string ENTERPRISE_ID = "8289";

        private volatile Int32 sequence_id = 1;

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

        protected virtual void PreProcessMessage(ref SyslogMessage msg)
        {
            // Getting the caller information(note that index is 2 because of Log is called by another local Method... 
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            msg.Data = new Dictionary<String, IDictionary<String, String>>();

            Dictionary<string, string> caller_data = new Dictionary<string, string>();
            msg.Data.Add("CallerData@" + ENTERPRISE_ID, caller_data);
            caller_data.Add("ClassName", stackFrames[3].GetMethod().DeclaringType.FullName);
            caller_data.Add("MethodName", stackFrames[3].GetMethod().Name);
            caller_data.Add("ModuleName", stackFrames[3].GetMethod().DeclaringType.Assembly.GetName().Name);
            if (!string.IsNullOrEmpty(LogName)) caller_data.Add("LogName", LogName);

            //Standard timeQuality
            Dictionary<string, string> timeQuality = new Dictionary<string, string>();
            msg.Data.Add("timeQuality", timeQuality);
            timeQuality.Add("tzKnown", "1");

            //Originator information
            Dictionary<string, string> origin = new Dictionary<string, string>();
            msg.Data.Add("origin", origin);
            origin.Add("ip", Utils.NetworkUtils.GetMyIPAddress().ToString());
            origin.Add("enterpriseId", ENTERPRISE_ID);
            origin.Add("software", "Logbus-ng-sharp");
            origin.Add("swVersion", GetType().Assembly.GetName().Version.ToString(3));

            //Meta-info
            Dictionary<string, string> meta = new Dictionary<string, string>();
            msg.Data.Add("meta", meta);

            meta.Add("sequenceId", sequence_id.ToString(CultureInfo.InvariantCulture));
            if (sequence_id == Int32.MaxValue) sequence_id = 1;
            else sequence_id++;

#if MONO
#else
            double up_time;
            using (PerformanceCounter uptime = new PerformanceCounter("System", "System Up Time"))
            {
                uptime.NextValue();       //Call this an extra time before reading its value
                up_time = TimeSpan.FromSeconds(uptime.NextValue()).TotalMilliseconds / 10;
            }
            meta.Add("sysUpTime", up_time.ToString(CultureInfo.InvariantCulture));
#endif
        }

        protected virtual void Log(string message, SyslogSeverity severity)
        {
            String host = Environment.MachineName;
            String procid = Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);
            String appname = Process.GetCurrentProcess().ProcessName;

            SyslogMessage msg = new SyslogMessage(host, Facility, severity, message)
            {
                ProcessID = procid,
                ApplicationName = appname
            };

            PreProcessMessage(ref msg);

            Target.SubmitMessage(msg);
        }

        #region ILog Membri di

        public string LogName
        {
            get;
            set;
        }

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
