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
using System.Threading;
using It.Unina.Dis.Logbus.Wrappers;
namespace It.Unina.Dis.Logbus.WebServices
{
#if MONO
    [System.Web.Services.WebServiceBindingAttribute(Name = "ChannelSubscription", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    [System.Web.Services.WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
#endif
    public class ChannelSubscriptionService
        : WebService, IChannelSubscription
    {

        public const string APPLICATION_KEY = "Logbus.ChannelSubscription";

        #region Constructor

        public ChannelSubscriptionService()
            : base()
        {
            if (Application[APPLICATION_KEY] != null)
                TargetChannelSubscription = Application[APPLICATION_KEY] as IChannelSubscription;
            else
            {
                object AppDomainData = Thread.GetDomain().GetData("Logbus");
                if (AppDomainData != null)
                    try
                    {
                        TargetChannelSubscription = (AppDomainData is IChannelSubscription) ?
                            AppDomainData as IChannelSubscription : new Logbus2SoapAdapter((ILogBus)AppDomainData);
                    }
                    catch { }
            }
        }

        public ChannelSubscriptionService(IChannelSubscription target)
            : base()
        {
            TargetChannelSubscription = target;
        }

        #endregion

        public virtual IChannelSubscription TargetChannelSubscription
        {
            get;
            set;
        }

        #region IChannelSubscription Membri di

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#ListChannels", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        public string[] ListChannels()
        {
            return TargetChannelSubscription.ListChannels();
        }

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetAvailableTransports", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        public string[] GetAvailableTransports()
        {
            return TargetChannelSubscription.GetAvailableTransports();
        }

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#SubscribeChannel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("channel-sub-response", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        public ChannelSubscriptionResponse SubscribeChannel([System.Xml.Serialization.XmlElementAttribute("channel-sub-request", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] ChannelSubscriptionRequest channelsubrequest)
#else
        public ChannelSubscriptionResponse SubscribeChannel(ChannelSubscriptionRequest channelsubrequest)
#endif
        {
            try
            {
                return TargetChannelSubscription.SubscribeChannel(channelsubrequest);
            }
            catch { throw; }
        }

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#UnsubscribeChannel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        public void UnsubscribeChannel([System.Xml.Serialization.XmlElementAttribute("client-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string clientid)
#else
        public void UnsubscribeChannel( string clientid)
#endif  
        {
            try
            {
                TargetChannelSubscription.UnsubscribeChannel(clientid);
            }
            catch { throw; }
        }

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#RefreshSubscription", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        public void RefreshSubscription([System.Xml.Serialization.XmlElementAttribute("client-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string clientid)
#else
        public void RefreshSubscription(string clientid)
#endif
        {
            try
            {
                TargetChannelSubscription.RefreshSubscription(clientid);
            }
            catch { throw; }
        }

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetAvailableFilters", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        public string[] GetAvailableFilters()
        {
            return TargetChannelSubscription.GetAvailableFilters();
        }

        /// <summary>
        /// Describes a custom filter
        /// </summary>
        /// <param name="filterid">ID of custom filter</param>
        /// <returns></returns>
#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#DescribeFilter", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("filter-description", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        public FilterDescription DescribeFilter([System.Xml.Serialization.XmlElementAttribute("filter-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string filterid)
#else
        public FilterDescription DescribeFilter(string filterid)
#endif
        {
            try
            {
                return TargetChannelSubscription.DescribeFilter(filterid);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
