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
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0")]
    [XmlRoot("custom-filters", Namespace = "http://www.dis.unina.it/logbus-ng/configuration/2.0", IsNullable = false)]
    public class CustomFiltersConfiguration
    {
        private CustomFilterDefinition[] customfilterField;

        private AssemblyToScan[] scanassemblyField;

        /// <remarks/>
        [XmlElement("custom-filter")]
        public CustomFilterDefinition[] customfilter
        {
            get { return customfilterField; }
            set { customfilterField = value; }
        }

        /// <remarks/>
        [XmlElement("scan-assembly")]
        public AssemblyToScan[] scanassembly
        {
            get { return scanassemblyField; }
            set { scanassemblyField = value; }
        }
    }
}