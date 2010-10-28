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
using System.Web.Services;

namespace It.Unina.Dis.Logbus.Entities
{
#if MONO
    /// <summary>
    /// WSDL skeleton class for EntityPlugin
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "EntityManagement", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(LoggingEntityIdentifier))]
#endif

    /// <summary>
    /// WSDL skeleton class for EntityPlugin
    /// </summary>
    public sealed class EntityManagementSkeleton
        : WebService, IEntityManagement
    {
        private readonly IEntityManagement _proxy;

        /// <summary>
        /// Initializes a new instance of EntityManagerSkeleton
        /// </summary>
        public EntityManagementSkeleton()
        {
            _proxy =
                (Application[EntityPlugin.PLUGIN_ID] as IEntityManagement ??
                 AppDomain.CurrentDomain.GetData(EntityPlugin.PLUGIN_ID)) as IEntityManagement;

            if (_proxy == null)
                throw new InvalidOperationException(
                    "No Entity Manager proxy found. Perhaps it has not been activated in App.config/Web.config.");
        }

        #region IEntityManagement Membri di

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetLoggingEntities", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("entity", IsNullable = false)]
#endif

        LoggingEntity[] IEntityManagement.GetLoggingEntities()
        {
            return _proxy.GetLoggingEntities();
        }

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#FindLoggingEntities", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("entities", Namespace = "http://www.dis.unina.it/logbus-ng/em")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("entity", IsNullable = false)]
        LoggingEntity[] IEntityManagement.FindLoggingEntities([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/em")] TemplateQuery query)
        {
#else
        LoggingEntity[] IEntityManagement.FindLoggingEntities(TemplateQuery query)
        {
#endif
            return _proxy.FindLoggingEntities(query);
        }

        #endregion
    }
}