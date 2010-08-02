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
        /// <summary>
        /// Emergency: system is unusable
        /// </summary>
        Emergency = 0,

        /// <summary>
        /// Alert: action must be taken immediately
        /// </summary>
        Alert = 1,

        /// <summary>
        /// Critical: critical conditions
        /// </summary>
        Critical = 2,

        /// <summary>
        /// Error: error conditions
        /// </summary>
        Error = 3,

        /// <summary>
        /// Warning: noticeable events
        /// </summary>
        Warning = 4,

        /// <summary>
        /// Notice: normal but significant condition
        /// </summary>
        Notice = 5,

        /// <summary>
        /// Informational: low-priority informational messages
        /// </summary>
        Info = 6,

        /// <summary>
        /// Debug: debug-level messages
        /// </summary>
        Debug = 7

    }
}
