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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    public enum Severity
    {

        Emergency = 0,    // Emergency: system is unusable
        Alert = 1,    // Alert: action must be taken immediately
        Critical = 2,    // Critical: critical conditions
        Error = 3,    // Error: error conditions
        Warning = 4,    // Warning: warning conditions
        Notice = 5,    // Notice: normal but significant condition
        Info = 6,    // Informational: informational messages
        Debug = 7,    // Debug: debug-level messages
    }
}
