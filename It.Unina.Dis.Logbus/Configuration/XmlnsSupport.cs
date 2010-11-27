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
using System.Xml.Serialization;

namespace It.Unina.Dis.Logbus.Configuration
{
    /// <remarks />
    [Serializable]
    public abstract class XmlnsSupport
    {
        /// <remarks />
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces xmlns
        {
            get
            {
                XmlSerializerNamespaces ret = new XmlSerializerNamespaces();
                ret.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                ret.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                ret.Add("filter", "http://www.dis.unina.it/logbus-ng/filters");
                ret.Add("config", "http://www.dis.unina.it/logbus-ng/configuration/2.0");
                return ret;
            }
        }
    }
}