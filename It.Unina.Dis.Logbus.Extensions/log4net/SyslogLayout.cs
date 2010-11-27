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

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using log4net.Core;
using log4net.Layout;

namespace It.Unina.Dis.Logbus.log4net
{
    /// <summary>
    /// Log4net layout class for Syslog 2009 format
    /// </summary>
    public sealed class SyslogLayout
        : ILayout
    {
        /// <summary>
        /// Initializes a new instance of SyslogLayout
        /// </summary>
        public SyslogLayout()
        {
            Facility = SyslogFacility.User;
        }

        /// <summary>
        /// Facility for SyslogLayout messages
        /// </summary>
        public SyslogFacility Facility { get; set; }

        #region ILayout Membri di

        string ILayout.ContentType
        {
            get { return "text/plain"; }
        }

        string ILayout.Footer
        {
            get { return string.Empty; }
        }

        void ILayout.Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            SyslogSeverity severity;
            int level = loggingEvent.Level.Value;

            if (level <= Level.Debug.Value)
                severity = SyslogSeverity.Debug;
            else if (level <= Level.Info.Value)
                severity = SyslogSeverity.Info;
            else if (level <= Level.Notice.Value)
                severity = SyslogSeverity.Notice;
            else if (level <= Level.Warn.Value)
                severity = SyslogSeverity.Warning;
            else if (level <= Level.Error.Value)
                severity = SyslogSeverity.Error;
            else if (level <= Level.Critical.Value)
                severity = SyslogSeverity.Critical;
            else if (level <= Level.Alert.Value)
                severity = SyslogSeverity.Alert;
            else
                severity = SyslogSeverity.Emergency;

            SyslogMessage message = new SyslogMessage(Dns.GetHostName(), SyslogFacility.User, severity,
                                                      loggingEvent.MessageObject.ToString())
                                        {
                                            MessageId = "log4net",
                                            ProcessID =
                                                Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture),
                                            ApplicationName = Process.GetCurrentProcess().ProcessName
                                        };

            writer.WriteLine(message.ToRfc5424String());
        }

        string ILayout.Header
        {
            get { return string.Empty; }
        }

        bool ILayout.IgnoresException
        {
            get { return true; }
        }

        #endregion
    }
}