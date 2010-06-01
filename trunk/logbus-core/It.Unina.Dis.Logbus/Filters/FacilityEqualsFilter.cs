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
    public partial class FacilityEqualsFilter : FilterBase
    {

        private Facility facilityField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public Facility facility
        {
            get
            {
                return this.facilityField;
            }
            set
            {
                this.facilityField = value;
                this.RaisePropertyChanged("facility");
            }
        }

        public override bool IsMatch(SyslogMessage message)
        {
            //Comparison by name. We make it work!
            string msgname, filtername;
            msgname = System.Enum.GetName(typeof(SyslogFacility), message.Facility);
            filtername = System.Enum.GetName(typeof(Facility), facility);

            return msgname.Equals(filtername);
        }
    }
}
