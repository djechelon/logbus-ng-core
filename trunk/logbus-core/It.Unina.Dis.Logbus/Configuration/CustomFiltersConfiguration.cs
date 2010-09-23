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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0")]
    [System.Xml.Serialization.XmlRootAttribute("custom-filters", Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0", IsNullable = false)]
    public partial class CustomFiltersConfiguration
    {

        private CustomFilterDefinition[] customfilterField;

        private AssemblyToScan[] scanassemblyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("custom-filter")]
        public CustomFilterDefinition[] customfilter
        {
            get
            {
                return this.customfilterField;
            }
            set
            {
                this.customfilterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("scan-assembly")]
        public AssemblyToScan[] scanassembly
        {
            get
            {
                return this.scanassemblyField;
            }
            set
            {
                this.scanassemblyField = value;
            }
        }
    }
}
