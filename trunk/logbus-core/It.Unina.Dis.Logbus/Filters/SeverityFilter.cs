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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    [System.Xml.Serialization.XmlRootAttribute("Severity", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public partial class SeverityFilter : FilterBase
    {

        private ComparisonOperator comparisonField;

        private Severity severityField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public ComparisonOperator comparison
        {
            get
            {
                return this.comparisonField;
            }
            set
            {
                this.comparisonField = value;
                this.RaisePropertyChanged("comparison");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public Severity severity
        {
            get
            {
                return this.severityField;
            }
            set
            {
                this.severityField = value;
                this.RaisePropertyChanged("severity");
            }
        }

        public override bool IsMatch(SyslogMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
