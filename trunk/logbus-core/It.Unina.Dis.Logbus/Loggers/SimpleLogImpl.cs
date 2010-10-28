﻿/*
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
using System.Runtime.CompilerServices;
using System.Threading;
using It.Unina.Dis.Logbus.Utils;

namespace It.Unina.Dis.Logbus.Loggers
{
    /// <summary>
    /// Simple ILog implementation
    /// </summary>
    public class SimpleLogImpl
        : ILogger
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

        private readonly Timer _heartbeatTimer;
        private int _hbInterval;
        private volatile Int32 _sequenceId = 1;

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SimpleLogImpl()
            : this(SyslogFacility.Local4)
        {
        }

        /// <summary>
        /// Constructor with facility
        /// </summary>
        /// <param name="facility">Syslog facility</param>
        public SimpleLogImpl(SyslogFacility facility)
        {
            Facility = facility;

            _heartbeatTimer = new Timer(HeartbeatCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Constructor with facility and collector
        /// </summary>
        /// <param name="facility">Syslog facility</param>
        /// <param name="target">Ultimate destination of messages</param>
        public SimpleLogImpl(SyslogFacility facility, ILogCollector target)
            : this(facility)
        {
            if (target == null) throw new ArgumentNullException("target");

            Collector = target;
        }

        /// <summary>
        /// Constructor with collector
        /// </summary>
        /// <param name="target">Ultimate destination of messages</param>
        public SimpleLogImpl(ILogCollector target)
            : this(SyslogFacility.Local4, target)
        {
        }

        /// <summary>
        /// Destroys SimpleLogImpl
        /// </summary>
        ~SimpleLogImpl()
        {
            if (_heartbeatTimer != null) _heartbeatTimer.Dispose();
        }

        #endregion

        /// <summary>
        /// Implements ILog.Heartbeat
        /// </summary>
        public int HeartbeatInterval
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get { return _hbInterval; }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                _hbInterval = value;
                if (value == 0)
                    _heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
                else
                    _heartbeatTimer.Change(value*1000, value*1000);
            }
        }

        /// <summary>
        /// Implements ILog.Facility
        /// </summary>
        public SyslogFacility Facility { get; set; }

        /// <summary>
        /// Implements ILog.Collector
        /// </summary>
        public ILogCollector Collector { get; set; }

        /// <summary>
        /// Heartbeat callback
        /// </summary>
        /// <param name="state"></param>
        private void HeartbeatCallback(object state)
        {
            try
            {
                if (Collector != null)
                {
                    String host = Environment.MachineName;
                    String procid = Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);
                    String appname = Process.GetCurrentProcess().ProcessName;

                    SyslogMessage msg = new SyslogMessage(host, Facility, SyslogSeverity.Debug, null)
                                            {
                                                ProcessID = procid,
                                                ApplicationName = appname,
                                                MessageId = "HEARTBEAT"
                                            };

                    PreProcessMessage(msg);

                    msg.Data.Remove("origin");
                    msg.Data.Remove("timeQuality");

                    if (msg.Data["meta"].ContainsKey("syUpTime"))
                        msg.Data["meta"].Remove("sysUpTime");

                    Collector.SubmitMessage(msg);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Processes the message before sending, adding contextual information
        /// </summary>
        /// <param name="msg">Message that is going to be sent</param>
        protected virtual void PreProcessMessage(SyslogMessage msg)
        {
            // Getting the caller information (note that index is 3 because of Log is called by another local Method... 
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            msg.Data = new Dictionary<String, IDictionary<String, String>>();

            Dictionary<string, string> callerData = new Dictionary<string, string>();
            msg.Data.Add("CallerData@" + ENTERPRISE_ID, callerData);
            if (stackFrames != null && stackFrames.Length >= 4)
            {
                callerData.Add("ClassName", stackFrames[3].GetMethod().DeclaringType.FullName);
                callerData.Add("MethodName", stackFrames[3].GetMethod().Name);
                callerData.Add("ModuleName", stackFrames[3].GetMethod().DeclaringType.Assembly.GetName().Name);
            }
            if (!string.IsNullOrEmpty(LogName)) callerData.Add("LogName", LogName);

            //Standard timeQuality
            Dictionary<string, string> timeQuality = new Dictionary<string, string>();
            msg.Data.Add("timeQuality", timeQuality);
            timeQuality.Add("tzKnown", "1");

            //Originator information
            Dictionary<string, string> origin = new Dictionary<string, string>();
            msg.Data.Add("origin", origin);
            origin.Add("ip", NetworkUtils.GetMyIPAddress().ToString());
            origin.Add("enterpriseId", ENTERPRISE_ID);
            origin.Add("software", "Logbus-ng-sharp");
            origin.Add("swVersion", typeof (SimpleLogImpl).Assembly.GetName().Version.ToString(3));

            //Meta-info
            Dictionary<string, string> meta = new Dictionary<string, string>();
            msg.Data.Add("meta", meta);

            meta.Add("sequenceId", _sequenceId.ToString(CultureInfo.InvariantCulture));
            if (_sequenceId == Int32.MaxValue) _sequenceId = 1;
            else _sequenceId++;

#if MONO
#else
            long upTime;
            using (PerformanceCounter uptime = new PerformanceCounter("System", "System Up Time"))
            {
                uptime.NextValue(); //Call this an extra time before reading its value
                upTime = (long) Math.Round(TimeSpan.FromSeconds(uptime.NextValue()).TotalMilliseconds/10);
            }
            meta.Add("sysUpTime", upTime.ToString(CultureInfo.InvariantCulture));
#endif
        }

        /// <summary>
        /// Concretely performs logging
        /// </summary>
        /// <param name="message">Text message</param>
        /// <param name="severity">Severity of message</param>
        protected virtual void Log(string message, SyslogSeverity severity)
        {
            //Reset heartbeating
            if (_hbInterval > 0)
                _heartbeatTimer.Change(HeartbeatInterval*1000, HeartbeatInterval*1000);

            String host = Environment.MachineName;
            String procid = Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);
            String appname = Process.GetCurrentProcess().ProcessName;

            SyslogMessage msg = new SyslogMessage(host, Facility, severity, message)
                                    {
                                        ProcessID = procid,
                                        ApplicationName = appname
                                    };

            PreProcessMessage(msg);

            Collector.SubmitMessage(msg);
        }

        #region ILog Membri di

        /// <remarks/>
        public string LogName { get; set; }

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

        void ILog.Debug(string format, params object[] args)
        {
            Log(string.Format(format, args), SyslogSeverity.Debug);
        }

        void ILog.Info(string format, params object[] args)
        {
            Log(string.Format(format, args), SyslogSeverity.Info);
        }

        void ILog.Notice(string format, params object[] args)
        {
            Log(string.Format(format, args), SyslogSeverity.Notice);
        }

        void ILog.Warning(string format, params object[] args)
        {
            Log(string.Format(format, args), SyslogSeverity.Warning);
        }

        void ILog.Error(string format, params object[] args)
        {
            Log(string.Format(format, args), SyslogSeverity.Error);
        }

        void ILog.Critical(string format, params object[] args)
        {
            Log(string.Format(format, args), SyslogSeverity.Critical);
        }

        void ILog.Alert(string format, params object[] args)
        {
            Log(string.Format(format, args), SyslogSeverity.Alert);
        }

        void ILog.Emergency(string format, params object[] args)
        {
            Log(string.Format(format, args), SyslogSeverity.Emergency);
        }

        #endregion
    }
}