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

using System.Text.RegularExpressions;
using System;
namespace It.Unina.Dis.Logbus.Filters
{
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(MessageRegexNotMatchFilter))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    [System.Xml.Serialization.XmlRootAttribute("MessageRegexMatch", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public partial class MessageRegexMatchFilter : FilterBase
    {

        /// <remarks/>
        public MessageRegexMatchFilter()
        {
            this.PropertyChanged += MessageRegexMatchFilter_PropertyChanged;
        }

        
        private void MessageRegexMatchFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            pattern_regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
        }

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

        private Regex pattern_regex;

        /// <remarks/>
        public override bool IsMatch(SyslogMessage message)
        {
            //Review implementation and compile regex for best performance
            return pattern_regex.IsMatch(message.Text);
        }
    }
}
