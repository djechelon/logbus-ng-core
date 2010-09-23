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

namespace It.Unina.Dis.Logbus.Configuration
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0")]
    [System.Xml.Serialization.XmlRootAttribute("logbus-logger", Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0", IsNullable = false)]
    public partial class LogbusLoggerConfiguration
    {

        private LogbusCollectorDefinition[] collectorField;

        private LoggerDefinition[] loggerField;

        private string defaultloggertypeField;

        private string defaultcollectorField;

        private int defaultheartbeatintervalField;

        public LogbusLoggerConfiguration()
        {
            this.defaultheartbeatintervalField = 0;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("collector")]
        public LogbusCollectorDefinition[] collector
        {
            get
            {
                return this.collectorField;
            }
            set
            {
                this.collectorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("logger")]
        public LoggerDefinition[] logger
        {
            get
            {
                return this.loggerField;
            }
            set
            {
                this.loggerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-logger-type")]
        public string defaultloggertype
        {
            get
            {
                return this.defaultloggertypeField;
            }
            set
            {
                this.defaultloggertypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-collector")]
        public string defaultcollector
        {
            get
            {
                return this.defaultcollectorField;
            }
            set
            {
                this.defaultcollectorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("default-heartbeat-interval")]
        [System.ComponentModel.DefaultValueAttribute(0)]
        public int defaultheartbeatinterval
        {
            get
            {
                return this.defaultheartbeatintervalField;
            }
            set
            {
                this.defaultheartbeatintervalField = value;
            }
        }
    }
}
