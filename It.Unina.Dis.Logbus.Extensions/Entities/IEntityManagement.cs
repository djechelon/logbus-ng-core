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

namespace It.Unina.Dis.Logbus.Entities
{
    /// <remarks/>
    #if MONO
#else
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "EntityManagement", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(LoggingEntityIdentifier))]
#endif
    public interface IEntityManagement
    {

        /// <remarks/>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetLoggingEntities", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("entity", IsNullable = false)]
#endif
        LoggingEntity[] GetLoggingEntities();

        /// <remarks/>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#FindLoggingEntities", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("entity", IsNullable = false)]
#endif
        LoggingEntity[] FindLoggingEntities([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/em")] TemplateQuery query);
    }
}
