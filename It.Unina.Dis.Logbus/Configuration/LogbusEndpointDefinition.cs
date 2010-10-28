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

namespace It.Unina.Dis.Logbus.Configuration
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.1432")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0")]
    [XmlRoot("endpoint", Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0", IsNullable = false)]
    public class LogbusEndpointDefinition
    {
        private string subscriptionUrlField;

        private string managementUrlField;

        /// <remarks/>
        [XmlAttribute(DataType = "anyURI")]
        public string subscriptionUrl
        {
            get { return subscriptionUrlField; }
            set { subscriptionUrlField = value; }
        }

        /// <remarks/>
        [XmlAttribute(DataType = "anyURI")]
        public string managementUrl
        {
            get { return managementUrlField; }
            set { managementUrlField = value; }
        }
    }
}