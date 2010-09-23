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
    [System.Xml.Serialization.XmlRootAttribute("logger", Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0", IsNullable = false)]
    public partial class LoggerDefinition : TypeAndParamBase
    {

        private string nameField;

        private string collectoridField;

        private int heartbeatintervalField;

        private bool permanentField;

        public LoggerDefinition()
        {
            this.heartbeatintervalField = 0;
            this.permanentField = false;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("collector-id")]
        public string collectorid
        {
            get
            {
                return this.collectoridField;
            }
            set
            {
                this.collectoridField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("heartbeat-interval")]
        [System.ComponentModel.DefaultValueAttribute(0)]
        public int heartbeatinterval
        {
            get
            {
                return this.heartbeatintervalField;
            }
            set
            {
                this.heartbeatintervalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool permanent
        {
            get
            {
                return this.permanentField;
            }
            set
            {
                this.permanentField = value;
            }
        }
    }
}
