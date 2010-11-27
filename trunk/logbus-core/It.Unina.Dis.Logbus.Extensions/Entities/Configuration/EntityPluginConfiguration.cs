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

namespace It.Unina.Dis.Logbus.Entities.Configuration
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/entities")]
    [System.Xml.Serialization.XmlRootAttribute("logbus-entityplugin", Namespace = "http://www.dis.unina.it/logbus-ng/entities", IsNullable = false)]
    public partial class EntityPluginConfiguration
    {

        private FieldType[] entitykeyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("entity-key")]
        [System.Xml.Serialization.XmlArrayItemAttribute("field", IsNullable = false)]
        public FieldType[] entitykey
        {
            get
            {
                return this.entitykeyField;
            }
            set
            {
                this.entitykeyField = value;
            }
        }
    }
}
