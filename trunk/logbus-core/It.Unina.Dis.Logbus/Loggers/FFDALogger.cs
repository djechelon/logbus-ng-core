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
using System.Threading;

namespace It.Unina.Dis.Logbus.Utils
{
    /// <summary>
    /// Provides FFDA-specific logging services using the underlying Logbus-ng infrastructure
    /// </summary>
    public class FFDALogger
    {
        #region Constructor
        /// <summary>
        /// Initializes the FFDA logger with given Syslog facility and concrete logger
        /// </summary>
        /// <param name="facility">Syslog facility that will be used for all the messages</param>
        /// <param name="target">Concrete logger that will collect FFDA messages</param>
        public FFDALogger(SyslogFacility facility, ILogCollector target)
        {
            if (target == null) throw new ArgumentNullException("target");

            Facility = facility;
            Target = target;
        }

        /// <summary>
        /// Initializes the FFDA logger with the concrete underlying logger
        /// </summary>
        /// <param name="target">Concrete logger that will collect FFDA messages</param>
        public FFDALogger(ILogCollector target)
            : this(SyslogFacility.Local0, target) { }

        #endregion

        private SyslogFacility Facility { get; set; }
        private ILogCollector Target { get; set; }
        private string GetFlow()
        {
            if (FlowId == null)
                return Thread.CurrentThread.GetHashCode().ToString(CultureInfo.InvariantCulture);
            else
                return FlowId.GetHashCode().ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Uniquely identifies the current control flow.
        /// </summary>
        /// <remarks>
        /// Used to trace messages from different entities/nodes that are related by the execution of a single distributed operation, such as a transaction
        /// </remarks>
        public object FlowId
        {
            get;
            set;
        }

        /// <summary>
        /// Logs the event of Service Start
        /// </summary>
        public void LogSST()
        {
            Log("SST", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of an identified Service Start
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogSST(string id)
        {
            if (id != null)
                Log("SST-" + id, SyslogSeverity.Info);
            else
                Log("SST", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of a Service End
        /// </summary>
        public void LogSEN()
        {
            Log("SEN", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of an identified Service End
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogSEN(string id)
        {
            if (id != null)
                Log("SEN-" + id, SyslogSeverity.Info);
            else
                Log("SEN", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of an Entity Interaction Start
        /// </summary>
        public void LogEIS()
        {
            Log("EIS", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of an identified Entity Interaction Start
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogEIS(string id)
        {
            if (id != null)
                Log("EIS-" + id, SyslogSeverity.Info);
            else
                Log("EIS", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of an Entity Interaction End
        /// </summary>
        public void LogEIE()
        {
            Log("EIE", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of an identified Entity Interaction End
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogEIE(string id)
        {
            if (id != null)
                Log("EIE-" + id, SyslogSeverity.Info);
            else
                Log("EIE", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of a Computational Alert
        /// </summary>
        public void LogCOA()
        {
            Log("COA", SyslogSeverity.Error);
        }

        /// <summary>
        /// Logs the event of an identified Computational Alert
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogCOA(string id)
        {
            if (id != null)
                Log("COA-" + id, SyslogSeverity.Error);
            else
                Log("COA", SyslogSeverity.Error);
        }

        /// <summary>
        /// Logs the event of a Resource Interaction Start
        /// </summary>
        public void LogRIS()
        {
            Log("RIS", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of an identified Resource Interaction Start
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogRIS(string id)
        {
            if (id != null)
                Log("RIS-" + id, SyslogSeverity.Info); 
            else
                Log("RIS", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of a Resource Interaction End
        /// </summary>
        public void LogRIE()
        {
            Log("RIE", SyslogSeverity.Info);
        }

        /// <summary>
        /// Logs the event of an identified Resource Interaction End
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogRIE(string id)
        {
            if (id != null)
                Log("RIE-" + id, SyslogSeverity.Info);
            else
                Log("RIE", SyslogSeverity.Info);
        }

        /// <summary>
        /// Construct and delivers a Syslog message to the underlying logger
        /// </summary>
        /// <param name="message"></param>
        /// <param name="severity"></param>
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
                MessageId = "FFDA",
                ApplicationName = appname
            };

            // Getting the caller information(note that index is 2 because of Log is called by another local Method... 
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            msg.Data = new Dictionary<String, IDictionary<String, String>>();
            msg.Data.Add("CallerData@" + SimpleLogImpl.ENTERPRISE_ID, new Dictionary<String, String>());
            msg.Data["CallerData@" + SimpleLogImpl.ENTERPRISE_ID].Add("ClassName", stackFrames[3].GetMethod().DeclaringType.FullName);
            msg.Data["CallerData@" + SimpleLogImpl.ENTERPRISE_ID].Add("MethodName", stackFrames[3].GetMethod().Name);

            Target.SubmitMessage(msg);
        }
    }
}