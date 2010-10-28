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
    /// Default implementation of IChannelSubscription skeleton
    /// </summary>
#if MONO
    [System.Web.Services.WebServiceBindingAttribute(Name = "ChannelSubscription", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    [System.Web.Services.WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
#endif

    public class ChannelSubscriptionService
        : WebService, IChannelSubscription
    {
        /// <summary>
        /// Key with which store IChannelSubscription into ASP.NET Application object
        /// </summary>
        public const string APPLICATION_KEY = "Logbus.ChannelSubscription";

        #region Constructor

        /// <remarks/>
        public ChannelSubscriptionService()
        {
            if (Application[APPLICATION_KEY] != null && Application[APPLICATION_KEY] is IChannelSubscription)
                TargetChannelSubscription = (IChannelSubscription) Application[APPLICATION_KEY];
            else
            {
                throw new LogbusException("Logbus instance not set by Global.asax");
            }
        }

        /// <summary>
        /// Initialize ChannelSubscriptionService with a target
        /// </summary>
        /// <param name="target">Target to proxy</param>
        public ChannelSubscriptionService(IChannelSubscription target)
        {
            TargetChannelSubscription = target;
        }

        #endregion

        /// <summary>
        /// Target IChannelSubscription to proxy
        /// </summary>
        public IChannelSubscription TargetChannelSubscription { get; set; }

        #region IChannelSubscription Membri di

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#ListChannels", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif

        public virtual string[] ListChannels()
        {
            return TargetChannelSubscription.ListChannels();
        }

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetAvailableTransports", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif

        public virtual string[] GetAvailableTransports()
        {
            return TargetChannelSubscription.GetAvailableTransports();
        }

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#SubscribeChannel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("channel-sub-response", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        public ChannelSubscriptionResponse SubscribeChannel([System.Xml.Serialization.XmlElementAttribute("channel-sub-request", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] ChannelSubscriptionRequest channelsubrequest)
#else
        public virtual ChannelSubscriptionResponse SubscribeChannel(ChannelSubscriptionRequest channelsubrequest)
#endif
        {
            return TargetChannelSubscription.SubscribeChannel(channelsubrequest);
        }

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#UnsubscribeChannel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        public void UnsubscribeChannel([System.Xml.Serialization.XmlElementAttribute("client-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string clientid)
#else
        public virtual void UnsubscribeChannel(string clientid)
#endif
        {
            TargetChannelSubscription.UnsubscribeChannel(clientid);
        }

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#RefreshSubscription", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        public void RefreshSubscription([System.Xml.Serialization.XmlElementAttribute("client-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string clientid)
#else
        public virtual void RefreshSubscription(string clientid)
#endif
        {
            TargetChannelSubscription.RefreshSubscription(clientid);
        }

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetAvailableFilters", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif

        public virtual string[] GetAvailableFilters()
        {
            return TargetChannelSubscription.GetAvailableFilters();
        }

        /// <remarks/>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#DescribeFilter", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("filter-description", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        public FilterDescription DescribeFilter([System.Xml.Serialization.XmlElementAttribute("filter-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string filterid)
#else
        public virtual FilterDescription DescribeFilter(string filterid)
#endif
        {
            return TargetChannelSubscription.DescribeFilter(filterid);
        }

        #endregion
    }
}