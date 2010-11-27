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
    [GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [WebServiceBindingAttribute(Name = "EntityManagement", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
    [XmlIncludeAttribute(typeof(LoggingEntityIdentifier))]
    public interface IEntityManagement
    {

        /// <summary>
        /// Queries the EntityPlugin for all the logging entities currently known
        /// </summary>
        /// <returns>List of known active entities</returns>
        [WebMethodAttribute]
        [SoapDocumentMethodAttribute("urn:#GetLoggingEntities", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlArrayAttribute("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: XmlArrayItemAttribute("entity", IsNullable = false)]
        LoggingEntity[] GetLoggingEntities();

        /// <summary>
        /// Queries the EntityPlugin for all the logging entities currently known that match the given filter query
        /// </summary>
        /// <param name="query">Select criterion</param>
        /// <returns>List of known active entities that match the query</returns>
        [WebMethodAttribute]
        [SoapDocumentMethodAttribute("urn:#FindLoggingEntities", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlArrayAttribute("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: XmlArrayItemAttribute("entity", IsNullable = false)]
        LoggingEntity[] FindLoggingEntities([XmlElementAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/em")] TemplateQuery query);
    }
}
