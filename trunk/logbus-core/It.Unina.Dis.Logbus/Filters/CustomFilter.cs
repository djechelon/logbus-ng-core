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
    [System.Xml.Serialization.XmlRootAttribute("Custom", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public partial class CustomFilter : FilterBase
    {

        private FilterParameter[] parameterField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("parameter", IsNullable = true)]
        public FilterParameter[] parameter
        {
            get
            {
                return this.parameterField;
            }
            set
            {
                this.parameterField = value;
                this.RaisePropertyChanged("parameter");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
                this.RaisePropertyChanged("name");
            }
        }

        public override bool IsMatch(SyslogMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
