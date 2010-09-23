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
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(LoggerDefinition))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(InboundChannelDefinition))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(LogCollectorDefinitionBase))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(LogbusCollectorDefinition))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ForwarderDefinition))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0")]
    public abstract partial class TypeAndParamBase
    {

        private KeyValuePair[] paramField;

        private string typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("param")]
        public KeyValuePair[] param
        {
            get
            {
                return this.paramField;
            }
            set
            {
                this.paramField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }
}
