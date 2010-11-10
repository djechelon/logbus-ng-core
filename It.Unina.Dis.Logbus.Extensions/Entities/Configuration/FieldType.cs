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

namespace It.Unina.Dis.Logbus.Entities.Configuration
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/entities")]
    [System.Xml.Serialization.XmlRootAttribute("field", Namespace = "http://www.dis.unina.it/logbus-ng/entities", IsNullable = false)]
    public enum FieldType
    {

        /// <remarks/>
        host,

        /// <remarks/>
        process,

        /// <remarks/>
        logger,

        /// <remarks/>
        module,

        /// <remarks/>
        @class,

        /// <remarks/>
        method,
    }
}
