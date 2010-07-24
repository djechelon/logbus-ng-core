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
    /// Provides FFDA-specific logging services using the underlying Logbus-ng infrastructure
    /// </summary>
    /// <remarks>
    /// FFDA messages have the following costraints:
    /// <list>
    /// <item>Facility defaults to Local0</item>
    /// <item>Severity equals to Info or Alert</item>
    /// <item>MessageID equals to <c>FFDA</c></item>
    /// <item>Text matches regular expression <c>^(SST|SEN|RIS|RIE|EIS|EIE|COA|CMP)[-]?</c></item>
    /// </list>
    /// The COA message is a special message that is triggered <b>only</b> by an external entity that detects a failure in a monitored entity
    /// Use the CMP message to report about self-detect failures
    /// </remarks>
    public sealed class FFDALogger
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

        /// <summary>
        /// Logs the event of Service Start
        /// </summary>
        public void LogSST()
        {
            SimpleLogImpl.Log("SST", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an identified Service Start
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogSST(string id)
        {
            if (id != null)
                SimpleLogImpl.Log("SST-" + id, SyslogSeverity.Info, Facility, Target);
            else
                SimpleLogImpl.Log("SST", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of a Service End
        /// </summary>
        public void LogSEN()
        {
            SimpleLogImpl.Log("SEN", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an identified Service End
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogSEN(string id)
        {
            if (id != null)
                SimpleLogImpl.Log("SEN-" + id, SyslogSeverity.Info, Facility, Target);
            else
                SimpleLogImpl.Log("SEN", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an Entity Interaction Start
        /// </summary>
        public void LogEIS()
        {
            SimpleLogImpl.Log("EIS", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an identified Entity Interaction Start
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogEIS(string id)
        {
            if (id != null)
                SimpleLogImpl.Log("EIS-" + id, SyslogSeverity.Info, Facility, Target);
            else
                SimpleLogImpl.Log("EIS", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an Entity Interaction End
        /// </summary>
        public void LogEIE()
        {
            SimpleLogImpl.Log("EIE", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an identified Entity Interaction End
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogEIE(string id)
        {
            if (id != null)
                SimpleLogImpl.Log("EIE-" + id, SyslogSeverity.Info, Facility, Target);
            else
                SimpleLogImpl.Log("EIE", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of a Resource Interaction Start
        /// </summary>
        public void LogRIS()
        {
            SimpleLogImpl.Log("RIS", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an identified Resource Interaction Start
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogRIS(string id)
        {
            if (id != null)
                SimpleLogImpl.Log("RIS-" + id, SyslogSeverity.Info, Facility, Target);
            else
                SimpleLogImpl.Log("RIS", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of a Resource Interaction End
        /// </summary>
        public void LogRIE()
        {
            SimpleLogImpl.Log("RIE", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an identified Resource Interaction End
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        public void LogRIE(string id)
        {
            if (id != null)
                SimpleLogImpl.Log("RIE-" + id, SyslogSeverity.Info, Facility, Target);
            else
                SimpleLogImpl.Log("RIE", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of a Complaint
        /// </summary>
        public void LogCMP()
        {
            SimpleLogImpl.Log("CMP", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an identified Complaint
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        /// <remarks>Never use Exception.Message as id: if you need to log such message, use another logger!</remarks>
        public void LogCMP(string id)
        {
            if (id != null)
                SimpleLogImpl.Log("CMP-" + id, SyslogSeverity.Info, Facility, Target);
            else
                SimpleLogImpl.Log("CMP", SyslogSeverity.Info, Facility, Target);
        }

        /// <summary>
        /// Logs the event of a Computational Alert
        /// </summary>
        /// <remarks>COA is triggered only by an external monitor that detects a failure in the target entity</remarks>
        public void LogCOA()
        {
            SimpleLogImpl.Log("COA", SyslogSeverity.Alert, Facility, Target);
        }

        /// <summary>
        /// Logs the event of an identified Computational Alert
        /// </summary>
        /// <param name="id">Identification for the current service instance</param>
        /// <remarks>COA is triggered only by an external monitor that detects a failure in the target entity</remarks>
        public void LogCOA(string id)
        {
            if (id != null)
                SimpleLogImpl.Log("COA-" + id, SyslogSeverity.Alert, Facility, Target);
            else
                SimpleLogImpl.Log("COA", SyslogSeverity.Alert, Facility, Target);
        }

    }
}