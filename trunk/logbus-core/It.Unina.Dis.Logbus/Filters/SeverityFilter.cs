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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace It.Unina.Dis.Logbus.Filters
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    [XmlRoot("Severity", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public class SeverityFilter : FilterBase
    {
        private ComparisonOperator comparisonField;

        private Severity severityField;

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public ComparisonOperator comparison
        {
            get { return comparisonField; }
            set
            {
                comparisonField = value;
                RaisePropertyChanged("comparison");
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public Severity severity
        {
            get { return severityField; }
            set
            {
                severityField = value;
                RaisePropertyChanged("severity");
            }
        }


        /// <remarks>"Higher" severity has lower code</remarks>
        public override bool IsMatch(SyslogMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            int result = ((int) message.Severity).CompareTo((int) severity);
            switch (comparison)
            {
                case ComparisonOperator.eq:
                    {
                        return result == 0;
                    }
                case ComparisonOperator.geq:
                    {
                        return result <= 0;
                    }
                case ComparisonOperator.gt:
                    {
                        return result < 0;
                    }
                case ComparisonOperator.leq:
                    {
                        return result >= 0;
                    }
                case ComparisonOperator.lt:
                    {
                        return result > 0;
                    }
                case ComparisonOperator.neq:
                    {
                        return result != 0;
                    }
                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }
    }
}