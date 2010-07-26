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

using It.Unina.Dis.Logbus.RemoteLogbus;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Manages subscription to Logbus channels
    /// </summary>
#if MONO
#else
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ChannelSubscription", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    [System.Web.Services.WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
#endif
    public interface IChannelSubscription
    {

        /// <summary>
        /// Lists the available channels by their unique IDs
        /// </summary>
        /// <returns>List of channel IDs</returns>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#ListChannels", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        string[] ListChannels();

        /// <summary>
        /// Lists the available outbound transports
        /// </summary>
        /// <returns>List of IDs of outbound transports</returns>
        /// <remarks>Clients must be aware of transport semantics, as defined in documentation.
        /// Clients must choose only transport they are natively compiled for and that are supported by the server</remarks>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetAvailableTransports", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        string[] GetAvailableTransports();

        /// <summary>
        /// Subscribes to a channel
        /// </summary>
        /// <param name="request">Subscription Information</param>
        /// <returns>Client ID (for future request) and client instructions</returns>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#SubscribeChannel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("channel-sub-response", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        ChannelSubscriptionResponse SubscribeChannel([System.Xml.Serialization.XmlElementAttribute("channel-sub-request", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] ChannelSubscriptionRequest channelsubrequest);

        /// <summary>
        /// Unsubscribes from a channel
        /// </summary>
        /// <param name="id">Client ID as returned by SubscribeChannel</param>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#UnsubscribeChannel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
#endif
        void UnsubscribeChannel([System.Xml.Serialization.XmlElementAttribute("client-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string clientid);

        /// <summary>
        /// Refreshes client subscription, if required by transport
        /// </summary>
        /// <param name="id">ID of client as returned by SubscribeChannel</param>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#RefreshSubscription", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
#endif
        void RefreshSubscription([System.Xml.Serialization.XmlElementAttribute("client-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string clientid);

        /// <summary>
        /// Lists the available custom filters on the server by their IDs
        /// </summary>
        /// <returns></returns>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetAvailableFilters", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        string[] GetAvailableFilters();

        /// <summary>
        /// Describes a custom filter
        /// </summary>
        /// <param name="filterid">ID of custom filter</param>
        /// <returns></returns>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#DescribeFilter", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("filter-description", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        FilterDescription DescribeFilter([System.Xml.Serialization.XmlElementAttribute("filter-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string filterid);
    }
}
