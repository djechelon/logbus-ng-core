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
using System.Collections;
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
    [XmlRoot("Property", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public class PropertyFilter : FilterBase
    {
        /// <remarks/>
        public PropertyFilter()
        {
            PropertyChanged += PropertyFilter_PropertyChanged;
        }

        private void PropertyFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "propertyName") prop_set = true;
            else if (e.PropertyName == "comparison") cmp_set = true;
            else if (e.PropertyName == "value") val_set = true;

            if (val_set && prop_set && cmp_set)
                switch (propertyName)
                {
                    case Property.Severity:
                        {
                            try
                            {
                                targetvalue = Enum.Parse(typeof (Severity), value);
                            }
                            catch
                            {
                                throw new ArgumentException("Value is incompatible with property");
                            }
                            break;
                        }
                    case Property.Facility:
                        {
                            try
                            {
                                targetvalue = Enum.Parse(typeof (Facility), value);
                            }
                            catch
                            {
                                throw new ArgumentException("Value is incompatible with property");
                            }
                            break;
                        }
                    case Property.Timestamp:
                        {
                            DateTime val;
                            if (!DateTime.TryParse(value, out val))
                                throw new ArgumentException("Value is incompatible with property");
                            targetvalue = val;
                            break;
                        }
                    case Property.Host:
                    case Property.ApplicationName:
                    case Property.MessageID:
                    case Property.ProcessID:
                    case Property.Text:
                        {
                            targetvalue = value;
                            break;
                        }
                    case Property.Data:
                        {
                            throw new NotSupportedException();
                        }
                }
        }

        private string valueField;

        private Property propertyNameField;

        private ComparisonOperator comparisonField;

        /// <remarks/>
        public string value
        {
            get { return valueField; }
            set
            {
                valueField = value;
                RaisePropertyChanged("value");
            }
        }

        /// <remarks/>
        [XmlAttribute(Form = XmlSchemaForm.Qualified)]
        public Property propertyName
        {
            get { return propertyNameField; }
            set
            {
                propertyNameField = value;
                RaisePropertyChanged("propertyName");
            }
        }

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

        private object targetvalue;

        /// <remarks/>
        public override bool IsMatch(SyslogMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            object property;
            switch (propertyName)
            {
                case Property.Severity:
                    {
                        property = message.Severity;
                        break;
                    }
                case Property.Facility:
                    {
                        property = message.Facility;
                        break;
                    }
                case Property.Timestamp:
                    {
                        property = message.Timestamp;
                        break;
                    }
                case Property.Host:
                    {
                        property = message.Host;
                        break;
                    }
                case Property.ApplicationName:
                    {
                        property = message.ApplicationName;
                        break;
                    }
                case Property.ProcessID:
                    {
                        property = message.ProcessID;
                        break;
                    }
                case Property.MessageID:
                    {
                        property = message.MessageId;
                        break;
                    }
                case Property.Data:
                    {
                        throw new NotSupportedException("Data is structured. Cannot be compared");
                    }
                case Property.Text:
                    {
                        property = message.Text;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid property");
                    }
            }

            if (property == null)
            {
                return false;
            }
            else if (property is IComparable)
            {
                Comparer cmp = Comparer.DefaultInvariant;
                int result = cmp.Compare(property, targetvalue);
                switch (comparison)
                {
                    case ComparisonOperator.eq:
                        {
                            return result == 0;
                        }
                    case ComparisonOperator.geq:
                        {
                            return result >= 0;
                        }
                    case ComparisonOperator.gt:
                        {
                            return result > 0;
                        }
                    case ComparisonOperator.leq:
                        {
                            return result <= 0;
                        }
                    case ComparisonOperator.lt:
                        {
                            return result < 0;
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

            else if (property is int)
            {
                switch (comparison)
                {
                    case ComparisonOperator.eq:
                        {
                            return (int) property == (int) targetvalue;
                        }
                    case ComparisonOperator.geq:
                        {
                            return (int) property >= (int) targetvalue;
                        }
                    case ComparisonOperator.gt:
                        {
                            return (int) property > (int) targetvalue;
                        }
                    case ComparisonOperator.leq:
                        {
                            return (int) property <= (int) targetvalue;
                        }
                    case ComparisonOperator.lt:
                        {
                            return (int) property < (int) targetvalue;
                        }
                    case ComparisonOperator.neq:
                        {
                            return (int) property != (int) targetvalue;
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
            else if (property is DateTime)
            {
                switch (comparison)
                {
                    case ComparisonOperator.eq:
                        {
                            return (DateTime) property == (DateTime) targetvalue;
                        }
                    case ComparisonOperator.geq:
                        {
                            return (DateTime) property >= (DateTime) targetvalue;
                        }
                    case ComparisonOperator.gt:
                        {
                            return (DateTime) property > (DateTime) targetvalue;
                        }
                    case ComparisonOperator.leq:
                        {
                            return (DateTime) property <= (DateTime) targetvalue;
                        }
                    case ComparisonOperator.lt:
                        {
                            return (DateTime) property < (DateTime) targetvalue;
                        }
                    case ComparisonOperator.neq:
                        {
                            return (DateTime) property != (DateTime) targetvalue;
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
            throw new InvalidProgramException("Software bug!");
        }

        private bool prop_set, cmp_set, val_set;
    }
}