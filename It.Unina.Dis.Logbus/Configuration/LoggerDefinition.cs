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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace It.Unina.Dis.Logbus.Configuration
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0")]
    [XmlRoot("logger", Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0", IsNullable = false)]
    public class LoggerDefinition : TypeAndParamBase
    {
        private string nameField;

        private string collectoridField;

        private int heartbeatintervalField;

        private bool permanentField;

        /// <remarks/>
        public LoggerDefinition()
        {
            heartbeatintervalField = 0;
            permanentField = false;
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlAttribute("collector-id")]
        public string collectorid
        {
            get { return collectoridField; }
            set { collectoridField = value; }
        }

        /// <remarks/>
        [XmlAttribute("heartbeat-interval")]
        [DefaultValue(0)]
        public int heartbeatinterval
        {
            get { return heartbeatintervalField; }
            set { heartbeatintervalField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        [DefaultValue(false)]
        public bool permanent
        {
            get { return permanentField; }
            set { permanentField = value; }
        }
    }
}