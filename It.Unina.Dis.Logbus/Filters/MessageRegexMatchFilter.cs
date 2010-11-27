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
using System.Text.RegularExpressions;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace It.Unina.Dis.Logbus.Filters
{
    /// <remarks/>
    [XmlInclude(typeof (MessageRegexNotMatchFilter))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    [XmlRoot("MessageRegexMatch", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public class MessageRegexMatchFilter : FilterBase
    {
        /// <remarks/>
        public MessageRegexMatchFilter()
        {
            PropertyChanged += MessageRegexMatchFilter_PropertyChanged;
        }


        private void MessageRegexMatchFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            pattern_regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);
        }

        private string patternField;

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public string pattern
        {
            get { return patternField; }
            set
            {
                patternField = value;
                RaisePropertyChanged("pattern");
            }
        }

        private Regex pattern_regex;

        /// <remarks/>
        public override bool IsMatch(SyslogMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            //Review implementation and compile regex for best performance
            return pattern_regex.IsMatch(message.Text);
        }
    }
}