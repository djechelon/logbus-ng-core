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
using It.Unina.Dis.Logbus.Configuration;

namespace It.Unina.Dis.Logbus.Filters
{
    /// <remarks/>
    [XmlInclude(typeof (CustomFilter))]
    [XmlInclude(typeof (PropertyFilter))]
    [XmlInclude(typeof (FacilityEqualsFilter))]
    [XmlInclude(typeof (SeverityFilter))]
    [XmlInclude(typeof (MessageRegexMatchFilter))]
    [XmlInclude(typeof (MessageRegexNotMatchFilter))]
    [XmlInclude(typeof (FalseFilter))]
    [XmlInclude(typeof (TrueFilter))]
    [XmlInclude(typeof (NotFilter))]
    [XmlInclude(typeof (OrFilter))]
    [XmlInclude(typeof (AndFilter))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    [XmlRoot("filter", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public abstract class FilterBase : XmlnsSupport, INotifyPropertyChanged, IFilter
    {
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

        #region IFilter Membri di

        /// <remarks>
        /// Required by IFilter
        /// </remarks>
        public abstract bool IsMatch(SyslogMessage message);

        #endregion

        /// <summary>
        /// Operator implementing the logical <b>not</b> of a filter
        /// </summary>
        /// <param name="filter">Filter to negate</param>
        /// <returns>A new instance of a <see cref="It.Unina.Dis.Logbus.Filters.NotFilter"/> that negates output of previous filter</returns>
        public static FilterBase operator !(FilterBase filter)
        {
            if (filter == null) throw new ArgumentNullException("filter");
            return new NotFilter {filter = filter};
        }

        /// <summary>
        /// Operator implementing the logical <b>or</b> of a filter
        /// </summary>
        /// <param name="a">Left-side filter</param>
        /// <param name="b">Right-side filter</param>
        /// <returns>A new instance of a <see cref="It.Unina.Dis.Logbus.Filters.OrFilter"/> with the two filters as arguments</returns>
        /// <remarks>If one of the original filters is an <b>or</b> of other filters, it gets merged with the other one. It means that two
        /// or filters won't result in an "outer" or containing both, but into a single or filter. This might affect advanced debugging but
        /// does never affect the final result of message processing</remarks>
        public static FilterBase operator |(FilterBase a, FilterBase b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            FilterBase[] a_array = null, b_array = null;

            a_array = (a is OrFilter) ? ((OrFilter) a).filter : new[] {a};
            b_array = (b is OrFilter) ? ((OrFilter) b).filter : new[] {b};

            FilterBase[] final_array = new FilterBase[a_array.Length + b_array.Length];
            Array.Copy(a_array, 0, final_array, 0, a_array.Length);
            Array.Copy(b_array, 0, final_array, a_array.Length, b_array.Length);

            return new OrFilter {filter = final_array};
        }

        /// <summary>
        /// Operatand implementing the logical <b>and</b> of a filter
        /// </summary>
        /// <param name="a">Left-side filter</param>
        /// <param name="b">Right-side filter</param>
        /// <returns>A new instance of a <see cref="It.Unina.Dis.Logbus.Filters.AndFilter"/> with the two filters as arguments</returns>
        /// <remarks>If one of the andiginal filters is an <b>and</b> of other filters, it gets merged with the other one. It means that two
        /// and filters won't result in an "outer" and containing both, but into a single and filter. This might affect advanced debugging but
        /// does never affect the final result of message processing</remarks>
        public static FilterBase operator &(FilterBase a, FilterBase b)
        {
            if (a == null) throw new ArgumentNullException("a");
            if (b == null) throw new ArgumentNullException("b");

            FilterBase[] a_array = null, b_array = null;

            a_array = (a is AndFilter) ? ((AndFilter) a).filter : new[] {a};
            b_array = (b is AndFilter) ? ((AndFilter) b).filter : new[] {b};

            FilterBase[] final_array = new FilterBase[a_array.Length + b_array.Length];
            Array.Copy(a_array, 0, final_array, 0, a_array.Length);
            Array.Copy(b_array, 0, final_array, a_array.Length, b_array.Length);

            return new AndFilter {filter = final_array};
        }
    }
}