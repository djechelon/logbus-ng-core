﻿/*
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
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(MessageRegexNotMatchFilter))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(MessageRegexMatchFilter))]
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
    [System.Xml.Serialization.XmlRootAttribute("core-filter", Namespace = "http://www.dis.unina.it/logbus/configuration", IsNullable = true)]
    public abstract partial class Filter: IFilter
    {
        #region IFilter Membri di

        public abstract bool IsMatch(SyslogMessage message);

        #endregion
    }
}
