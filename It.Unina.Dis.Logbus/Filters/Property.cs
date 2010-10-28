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
using System.Xml.Serialization;

namespace It.Unina.Dis.Logbus.Filters
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    public enum Property
    {
        /// <remarks/>
        Timestamp,

        /// <remarks/>
        Severity,

        /// <remarks/>
        Facility,

        /// <remarks/>
        Host,

        /// <remarks/>
        ApplicationName,

        /// <remarks/>
        ProcessID,

        /// <remarks/>
        MessageID,

        /// <remarks/>
        Data,

        /// <remarks/>
        Text,
    }
}