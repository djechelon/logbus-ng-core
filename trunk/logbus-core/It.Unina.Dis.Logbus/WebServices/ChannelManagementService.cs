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

using System.Web.Services;
using It.Unina.Dis.Logbus.RemoteLogbus;

namespace It.Unina.Dis.Logbus.WebServices
{
    /// <summary>
    /// Default implementation of IChannelManagement skeleton
    /// </summary>
#if MONO
    [System.Web.Services.WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ChannelManagement", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
    public class ChannelManagementService
        : WebService, IChannelManagement
    {

        /// <summary>
        /// Key with which store the IChannelManagement proxied object into ASP.NET Application object
        /// </summary>
        public const string APPLICATION_KEY = "Logbus.ChannelManagement";

        #region Constructor

        /// <remarks/>
        public ChannelManagementService()
        {

            if (Application[APPLICATION_KEY] != null && Application[APPLICATION_KEY] is IChannelManagement)
                TargetChannelManager = (IChannelManagement)Application[APPLICATION_KEY];
            else
            {
                throw new LogbusException("Logbus instance not set by Global.asax");
            }
        }

        /// <remarks/>
        public ChannelManagementService(IChannelManagement target)
        {
            TargetChannelManager = target;
        }

        #endregion

        /// <summary>
        /// Target to be proxies
        /// </summary>
        public IChannelManagement TargetChannelManager
        {
            get;
            set;
        }

        #region IChannelManagement Membri di

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#ListChannels", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        public virtual string[] ListChannels()
        {
            return TargetChannelManager.ListChannels();
        }

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#CreateChannel", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        public void CreateChannel([System.Xml.Serialization.XmlElementAttribute("channel-creation", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] ChannelCreationInformation channelcreation)
#else
        public virtual void CreateChannel(ChannelCreationInformation channelcreation)
#endif
        {
            TargetChannelManager.CreateChannel(channelcreation);
        }

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetChannelInformation", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("channel-info", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        public ChannelInformation GetChannelInformation([System.Xml.Serialization.XmlElementAttribute("channel-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string channelid)
#else
        public virtual ChannelInformation GetChannelInformation(string channelid)
#endif
        {
            return TargetChannelManager.GetChannelInformation(channelid);
        }

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#DeleteChannel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        public void DeleteChannel([System.Xml.Serialization.XmlElementAttribute("channel-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string channelid)
#else
        public virtual void DeleteChannel(string channelid)
#endif
        {
            TargetChannelManager.DeleteChannel(channelid);
        }

        #endregion
    }
}
