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

using log4net;
using log4net.Layout;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using log4net.Core;
namespace It.Unina.Dis.Logbus.log4net
{
    public sealed class OldSyslogLayout
        : ILayout
    {

        #region ILayout Membri di

        string ILayout.ContentType
        {
            get { return "text/plain"; }
        }

        string ILayout.Footer
        {
            get { return string.Empty; }
        }

        void ILayout.Format(System.IO.TextWriter writer, global::log4net.Core.LoggingEvent loggingEvent)
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

            SyslogMessage message = new SyslogMessage()
            {
                Timestamp = loggingEvent.TimeStamp,
                Text = loggingEvent.MessageObject.ToString(),
                MessageId = "log4net",
                ProcessID = Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture),
                ApplicationName = Process.GetCurrentProcess().ProcessName,
                //Log4net default uses User as facility
                Facility = SyslogFacility.User,
                Host = Dns.GetHostName(),
                Severity = severity
            };

            writer.WriteLine(message.ToRfc3164String());
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
