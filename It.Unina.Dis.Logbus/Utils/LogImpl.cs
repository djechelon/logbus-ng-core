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

using It.Unina.Dis.Logbus.Api;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
namespace It.Unina.Dis.Logbus.Utils
{
    internal class LogImpl
        : ILog
    {

        private String getMD5Hash(String text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            String RightKey = "";
            for (int i = 0; i < retVal.Length; i++)
            {
                RightKey += String.Format("{0:X2}", retVal[i]);
            }
            return RightKey.ToUpper();
        }

        public LogImpl(SyslogFacility facility, ILogCollector target)
        {
            if (target == null) throw new ArgumentNullException("target");

            Facility = facility;
            Target = target;
        }

        public LogImpl(ILogCollector target)
            : this(SyslogFacility.Local4, target) { }

        private SyslogFacility Facility { get; set; }
        private ILogCollector Target { get; set; }

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
                ApplicationName = appname
            };

            // Getting the caller information(note that index is 2 because of Log is called by another local Method... 
            StackTrace stackTrace = new StackTrace();
            StackFrame[] stackFrames = stackTrace.GetFrames();
            msg.Data = new Dictionary<String, IDictionary<String, String>>();
            msg.Data.Add("CallerData", new Dictionary<String, String>());
            msg.Data["CallerData"].Add("ClassName", stackFrames[2].GetMethod().DeclaringType.Name);
            msg.Data["CallerData"].Add("MethodName", stackFrames[2].GetMethod().Name);
            msg.MessageId = "ID" + getMD5Hash(host + "::" + appname + "::" + procid + "::" + (msg.Data["CallerData"])["ClassName"] + "::" + (msg.Data["CallerData"])["MethodName"]);

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
