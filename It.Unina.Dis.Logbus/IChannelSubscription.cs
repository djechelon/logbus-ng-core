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
using It.Unina.Dis.Logbus.RemoteLogbus;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Manages subscription to Logbus channels
    /// </summary>
    [GeneratedCode("wsdl", "2.0.50727.3038")]
    [WebServiceBinding(Name = "ChannelSubscription", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    [WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    public interface IChannelSubscription
    {
        /// <summary>
        /// Lists the available channels by their unique IDs
        /// </summary>
        /// <returns>List of channel IDs</returns>
        [WebMethod]
        [SoapDocumentMethod("urn:#ListChannels", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlArray("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        string[] ListChannels();

        /// <summary>
        /// Lists the available outbound transports
        /// </summary>
        /// <returns>List of IDs of outbound transports</returns>
        /// <remarks>Clients must be aware of transport semantics, as defined in documentation.
        /// Clients must choose only transport they are natively compiled for and that are supported by the server</remarks>
        [WebMethod]
        [SoapDocumentMethod("urn:#GetAvailableTransports", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlArray("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        string[] GetAvailableTransports();

        /// <summary>
        /// Subscribes to a channel
        /// </summary>
        /// <param name="channelsubrequest">Subscription Information</param>
        /// <returns>Client ID (for future request) and client instructions</returns>
        [WebMethod]
        [SoapDocumentMethod("urn:#SubscribeChannel", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlElement("channel-sub-response", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        ChannelSubscriptionResponse SubscribeChannel(
            [XmlElement("channel-sub-request", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] ChannelSubscriptionRequest channelsubrequest);

        /// <summary>
        /// Unsubscribes from a channel
        /// </summary>
        /// <param name="clientid">Client ID as returned by SubscribeChannel</param>
        [WebMethod]
        [SoapDocumentMethod("urn:#UnsubscribeChannel", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Bare)]
        void UnsubscribeChannel(
            [XmlElement("client-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string clientid);

        /// <summary>
        /// Refreshes client subscription, if required by transport
        /// </summary>
        /// <param name="clientid">ID of client as returned by SubscribeChannel</param>
        [WebMethod]
        [SoapDocumentMethod("urn:#RefreshSubscription", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Bare)]
        void RefreshSubscription(
            [XmlElement("client-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string clientid);

        /// <summary>
        /// Lists the available custom filters on the server by their IDs
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        [SoapDocumentMethod("urn:#GetAvailableFilters", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlArray("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        string[] GetAvailableFilters();

        /// <summary>
        /// Describes a custom filter
        /// </summary>
        /// <param name="filterid">ID of custom filter</param>
        /// <returns></returns>
        [WebMethod]
        [SoapDocumentMethod("urn:#DescribeFilter", Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlElement("filter-description", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        FilterDescription DescribeFilter(
            [XmlElement("filter-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string filterid);
    }
}