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
#if !MONO
    /// <remarks/>
    [GeneratedCode("wsdl", "2.0.50727.3038")]
    [WebServiceBinding(Name = "EntityManagement", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
    [XmlInclude(typeof (LoggingEntityIdentifier))]
#endif
    public interface IEntityManagement
    {
#if !MONO
        /// <remarks/>
        [WebMethod]
        [SoapDocumentMethod("urn:#GetLoggingEntities", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlArray("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: XmlArrayItem("entity", IsNullable = false)]
#endif
        LoggingEntity[] GetLoggingEntities();

#if !MONO
        /// <remarks/>
        [WebMethod]
        [SoapDocumentMethod("urn:#FindLoggingEntities", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlArray("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: XmlArrayItem("entity", IsNullable = false)]
        LoggingEntity[] FindLoggingEntities(
            [XmlElement(Namespace = "http://www.dis.unina.it/logbus-ng/em")] TemplateQuery query);
#else
    /// <remarks/>
        LoggingEntity[] FindLoggingEntities(TemplateQuery query);
#endif
    }
}