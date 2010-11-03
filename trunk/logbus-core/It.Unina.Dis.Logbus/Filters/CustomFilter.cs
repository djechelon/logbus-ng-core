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
            _configured = false;

            if (e.PropertyName == "name")
            {
                _filterImpl = null;

                if (!string.IsNullOrEmpty(name))
                {
                    if (!CustomFilterHelper.Instance.AvailableFilters.ContainsKey(name))
                        throw new LogbusException(string.Format("Filter {0} not registered", name));
                }
            }

            if (e.PropertyName == "parameter" && _filterImpl != null)
            {
                if (_filterImpl is ICustomFilter)
                    ((ICustomFilter)_filterImpl).Configuration = parameter;
                _configured = true;
            }
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

        private IFilter _filterImpl;
        private bool _configured;

        /// <remarks/>
        public override bool IsMatch(SyslogMessage message)
        {
            if (_filterImpl == null)
                lock (this)
                    if (_filterImpl == null)
                        _filterImpl = CustomFilterHelper.Instance.BuildFilter(name, parameter);

            if (!_configured && _filterImpl is ICustomFilter)
            {
                lock (this)
                {
                    ((ICustomFilter)_filterImpl).Configuration = parameter;
                    _configured = true;
                }
            }

            return _filterImpl.IsMatch(message);
        }
    }
}