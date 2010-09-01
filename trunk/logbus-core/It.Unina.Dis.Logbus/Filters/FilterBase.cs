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
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(CustomFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(PropertyFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(FacilityEqualsFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SeverityFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(MessageRegexMatchFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(MessageRegexNotMatchFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(FalseFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TrueFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(NotFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(OrFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(AndFilter))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    [System.Xml.Serialization.XmlRootAttribute("filter", Namespace = "http://www.dis.unina.it/logbus-ng/filters", IsNullable = false)]
    public abstract partial class FilterBase : It.Unina.Dis.Logbus.Configuration.XmlnsSupport, System.ComponentModel.INotifyPropertyChanged, IFilter
    {

        /// <remarks/>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <remarks/>
        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
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
            if (filter == null) throw new System.ArgumentNullException("filter");
            return new NotFilter() { filter = filter };
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
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");

            FilterBase[] a_array = null, b_array = null;

            a_array = (a is OrFilter) ? ((OrFilter)a).filter : new FilterBase[] { a };
            b_array = (b is OrFilter) ? ((OrFilter)b).filter : new FilterBase[] { b };

            FilterBase[] final_array = new FilterBase[a_array.Length + b_array.Length];
            System.Array.Copy(a_array, 0, final_array, 0, a_array.Length);
            System.Array.Copy(b_array, 0, final_array, a_array.Length, b_array.Length);

            return new OrFilter() { filter = final_array };
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
            if (a == null) throw new System.ArgumentNullException("a");
            if (b == null) throw new System.ArgumentNullException("b");

            FilterBase[] a_array = null, b_array = null;

            a_array = (a is AndFilter) ? ((AndFilter)a).filter : new FilterBase[] { a };
            b_array = (b is AndFilter) ? ((AndFilter)b).filter : new FilterBase[] { b };

            FilterBase[] final_array = new FilterBase[a_array.Length + b_array.Length];
            System.Array.Copy(a_array, 0, final_array, 0, a_array.Length);
            System.Array.Copy(b_array, 0, final_array, a_array.Length, b_array.Length);

            return new AndFilter() { filter = final_array };
        }
    }
}
