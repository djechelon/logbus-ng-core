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
using System.Xml.Serialization;

namespace It.Unina.Dis.Logbus.Filters
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    [XmlRoot("Not", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public class NotFilter : FilterBase
    {
        private FilterBase filterField;

        /// <remarks/>
        public FilterBase filter
        {
            get { return filterField; }
            set
            {
                filterField = value;
                RaisePropertyChanged("filter");
            }
        }

        /// <remarks>Required by FilterBase</remarks>
        public override bool IsMatch(SyslogMessage message)
        {
            return !filter.IsMatch(message);
        }
    }
}