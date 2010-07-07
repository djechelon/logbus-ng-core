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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/configuration")]
    [System.Xml.Serialization.XmlRootAttribute("logbus-core", Namespace = "http://www.dis.unina.it/logbus-ng/configuration", IsNullable = false)]
    public partial class LogbusCoreConfiguration
    {

        [System.Xml.Serialization.XmlNamespaceDeclarations()]
        public System.Xml.Serialization.XmlSerializerNamespaces xmlns
        {
            get
            {
                System.Xml.Serialization.XmlSerializerNamespaces ret = new System.Xml.Serialization.XmlSerializerNamespaces();
                ret.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                ret.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                ret.Add("filter", "http://www.dis.unina.it/logbus-ng/filters");
                ret.Add("config", "http://www.dis.unina.it/logbus-ng/configuration");
                return ret;
            }
        }

        private InboundChannelDefinition[] inchannelsField;

        private CustomFiltersConfiguration customfiltersField;

        private OutputTransportsConfiguration outtransportsField;

        private It.Unina.Dis.Logbus.Filters.FilterBase corefilterField;

        private string outChannelFactoryTypeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("in-channels", IsNullable = true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("in-channel", IsNullable = false)]
        public InboundChannelDefinition[] inchannels
        {
            get
            {
                return this.inchannelsField;
            }
            set
            {
                this.inchannelsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("custom-filters")]
        public CustomFiltersConfiguration customfilters
        {
            get
            {
                return this.customfiltersField;
            }
            set
            {
                this.customfiltersField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("out-transports")]
        public OutputTransportsConfiguration outtransports
        {
            get
            {
                return this.outtransportsField;
            }
            set
            {
                this.outtransportsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("core-filter", IsNullable = true)]
        public It.Unina.Dis.Logbus.Filters.FilterBase corefilter
        {
            get
            {
                return this.corefilterField;
            }
            set
            {
                this.corefilterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string outChannelFactoryType
        {
            get
            {
                return this.outChannelFactoryTypeField;
            }
            set
            {
                this.outChannelFactoryTypeField = value;
            }
        }
    }
}
