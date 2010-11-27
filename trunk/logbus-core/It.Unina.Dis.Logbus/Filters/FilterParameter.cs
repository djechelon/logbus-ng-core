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
    [XmlRoot("parameter", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = true)]
    public class FilterParameter : object, INotifyPropertyChanged
    {
        private object valueField;

        private string nameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public object value
        {
            get { return valueField; }
            set
            {
                valueField = value;
                RaisePropertyChanged("value");
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

        /// <remarks/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <remarks/>
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}