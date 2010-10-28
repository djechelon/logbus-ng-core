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
    [XmlRoot("Custom", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public class CustomFilter : FilterBase
    {
        private FilterParameter[] parameterField;

        private string nameField;

        /// <remarks/>
        public CustomFilter()
        {
            PropertyChanged += CustomFilter_PropertyChanged;
        }

        private void CustomFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //Reset filter implementation, so next time the IsMatch is invoked it must be rebuilt
            filter_impl = null;
        }

        /// <remarks/>
        [XmlElement("parameter", IsNullable = true)]
        public FilterParameter[] parameter
        {
            get { return parameterField; }
            set
            {
                parameterField = value;
                RaisePropertyChanged("parameter");
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string name
        {
            get { return nameField; }
            set
            {
                nameField = value;
                RaisePropertyChanged("name");
            }
        }

        private IFilter filter_impl;

        /// <remarks/>
        public override bool IsMatch(SyslogMessage message)
        {
            if (filter_impl == null)
            {
                filter_impl = CustomFilterHelper.Instance.BuildFilter(name, parameter);
            }
            return filter_impl.IsMatch(message);
        }
    }
}