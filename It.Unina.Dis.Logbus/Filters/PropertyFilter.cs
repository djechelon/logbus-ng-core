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
using System.Collections;
using System.Globalization;
namespace It.Unina.Dis.Logbus.Filters
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    [System.Xml.Serialization.XmlRootAttribute("Property", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public partial class PropertyFilter : FilterBase
    {

        /// <remarks/>
        public PropertyFilter()
        {
            this.PropertyChanged += PropertyFilter_PropertyChanged;
        }

        void PropertyFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                                targetvalue = Enum.Parse(typeof(Severity), value);
                            }
                            catch { throw new ArgumentException("Value is incompatible with property"); }
                            break;
                        }
                    case Property.Facility:
                        {
                            try
                            {
                                targetvalue = Enum.Parse(typeof(Facility), value);
                            }
                            catch { throw new ArgumentException("Value is incompatible with property"); }
                            break;
                        }
                    case Property.Timestamp:
                        {
                            DateTime val;
                            if (!DateTime.TryParse(value, out val)) throw new ArgumentException("Value is incompatible with property");
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
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
                this.RaisePropertyChanged("value");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified)]
        public Property propertyName
        {
            get
            {
                return this.propertyNameField;
            }
            set
            {
                this.propertyNameField = value;
                this.RaisePropertyChanged("propertyName");
            }
        }

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

        private object targetvalue;

        /// <remarks/>
        public override bool IsMatch(SyslogMessage message)
        {
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
                            return (int)property == (int)targetvalue;
                        }
                    case ComparisonOperator.geq:
                        {
                            return (int)property >= (int)targetvalue;
                        }
                    case ComparisonOperator.gt:
                        {
                            return (int)property > (int)targetvalue;
                        }
                    case ComparisonOperator.leq:
                        {
                            return (int)property <= (int)targetvalue;
                        }
                    case ComparisonOperator.lt:
                        {
                            return (int)property < (int)targetvalue;
                        }
                    case ComparisonOperator.neq:
                        {
                            return (int)property != (int)targetvalue;
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
                            return (DateTime)property == (DateTime)targetvalue;
                        }
                    case ComparisonOperator.geq:
                        {
                            return (DateTime)property >= (DateTime)targetvalue;
                        }
                    case ComparisonOperator.gt:
                        {
                            return (DateTime)property > (DateTime)targetvalue;
                        }
                    case ComparisonOperator.leq:
                        {
                            return (DateTime)property <= (DateTime)targetvalue;
                        }
                    case ComparisonOperator.lt:
                        {
                            return (DateTime)property < (DateTime)targetvalue;
                        }
                    case ComparisonOperator.neq:
                        {
                            return (DateTime)property != (DateTime)targetvalue;
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
            throw new InvalidProgramException("Software bug!");
        }

        private bool prop_set = false, cmp_set = false, val_set = false;
    }
}
