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

namespace It.Unina.Dis.Logbus.Filters
{
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(MessageRegexNotMatchFilter))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    public partial class MessageRegexMatchFilter : FilterBase
    {

        private string patternField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public string pattern
        {
            get
            {
                return this.patternField;
            }
            set
            {
                this.patternField = value;
                this.RaisePropertyChanged("pattern");
            }
        }

        public override bool IsMatch(SyslogMessage message)
        {
            //Review implementation and compile regex for best performance
            return System.Text.RegularExpressions.Regex.IsMatch(message.Text, pattern);
        }
    }
}
