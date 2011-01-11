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
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace It.Unina.Dis.Logbus.Configuration
{
    /// <remarks/>
    [GeneratedCode("xsd", "4.0.30319.1")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/configuration/3.0")]
    [XmlRoot("logbus-logger", Namespace = "http://www.dis.unina.it/logbus-ng/configuration/3.0", IsNullable = false)]
    public class LogbusLoggerConfiguration : XmlnsSupport
    {
        private LogbusCollectorDefinition[] collectorField;

        private string defaultcollectorField;

        private int defaultheartbeatintervalField;
        private string defaultloggertypeField;
        private LoggerDefinition[] loggerField;

        public LogbusLoggerConfiguration()
        {
            defaultheartbeatintervalField = 0;
        }

        /// <remarks/>
        [XmlElement("collector")]
        public LogbusCollectorDefinition[] collector
        {
            get { return collectorField; }
            set { collectorField = value; }
        }

        /// <remarks/>
        [XmlElement("logger")]
        public LoggerDefinition[] logger
        {
            get { return loggerField; }
            set { loggerField = value; }
        }

        /// <remarks/>
        [XmlAttribute("default-logger-type")]
        public string defaultloggertype
        {
            get { return defaultloggertypeField; }
            set { defaultloggertypeField = value; }
        }

        /// <remarks/>
        [XmlAttribute("default-collector")]
        public string defaultcollector
        {
            get { return defaultcollectorField; }
            set { defaultcollectorField = value; }
        }

        /// <remarks/>
        [XmlAttribute("default-heartbeat-interval")]
        [DefaultValue(0)]
        public int defaultheartbeatinterval
        {
            get { return defaultheartbeatintervalField; }
            set { defaultheartbeatintervalField = value; }
        }
    }
}