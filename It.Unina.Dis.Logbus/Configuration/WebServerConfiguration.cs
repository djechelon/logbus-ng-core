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
    [GeneratedCode("xsd", "4.0.30319.1")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.dis.unina.it/logbus-ng/configuration/3.0")]
    [XmlRoot("webserver", Namespace = "http://www.dis.unina.it/logbus-ng/configuration/3.0", IsNullable = false)]
    public class WebServerConfiguration
    {
        private bool activeField;

        private short portField;

        public WebServerConfiguration()
        {
            activeField = false;
            portField = ((8065));
        }

        /// <remarks/>
        [XmlAttribute]
        [DefaultValue(false)]
        public bool active
        {
            get { return activeField; }
            set { activeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        [DefaultValue(typeof (short), "8065")]
        public short port
        {
            get { return portField; }
            set { portField = value; }
        }
    }
}