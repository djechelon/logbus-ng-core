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

using System.CodeDom.Compiler;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace It.Unina.Dis.Logbus.Entities
{
    /// <summary>
    /// Interface for remote queries on EntityPlugin
    /// </summary>
    [XmlIncludeAttribute(typeof(LoggingEntityIdentifier))]
    public interface IEntityManagement
    {

        /// <remarks/>
        [WebMethodAttribute]
        [SoapDocumentMethodAttribute("urn:#GetLoggingEntities", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlArrayAttribute("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: XmlArrayItemAttribute("entity", IsNullable = false)]
        LoggingEntity[] GetLoggingEntities();

        /// <remarks/>
        [WebMethodAttribute()]
        [SoapDocumentMethodAttribute("urn:#FindLoggingEntities", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlArrayAttribute("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: XmlArrayItemAttribute("entity", IsNullable = false)]
        LoggingEntity[] FindLoggingEntities([XmlElementAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/em")] TemplateQuery query);

        /// <remarks/>
        [WebMethodAttribute]
        [SoapDocumentMethodAttribute("urn:#NewOperation", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlElementAttribute("entity-definition", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        EntityDefinition GetEntityDefinition();
    }
}
