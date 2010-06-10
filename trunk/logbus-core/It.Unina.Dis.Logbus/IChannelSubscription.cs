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

using It.Unina.Dis.Logbus.WebServices;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Manages subscription to Logbus channels
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ChannelSubscription", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    [System.Web.Services.WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    public interface IChannelSubscription
    {

        /// <summary>
        /// Lists the available channels by their unique IDs
        /// </summary>
        /// <returns>List of channel IDs</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:#ListChannels", RequestNamespace = "", ResponseNamespace = "", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
        [return: System.Xml.Serialization.XmlArrayAttribute("list")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        string[] ListChannels();

        /// <summary>
        /// Lists the available outbound transports
        /// </summary>
        /// <returns>List of IDs of outbound transports</returns>
        /// <remarks>Clients must be aware of transport semantics, as defined in documentation.
        /// Clients must choose only transport they are natively compiled for and that are supported by the server</remarks>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:#GetAvailableTransports", RequestNamespace = "", ResponseNamespace = "", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
        [return: System.Xml.Serialization.XmlArrayAttribute("list")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        string[] GetAvailableTransports();

        /// <summary>
        /// Subscribes to a channel
        /// </summary>
        /// <param name="request">Subscription Information</param>
        /// <returns>Client ID (for future request) and client instructions</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:#SubscribeChannel", RequestNamespace = "", ResponseNamespace = "", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
        [return: System.Xml.Serialization.XmlElementAttribute("client-config")]
        ChannelSubscriptionResponse SubscribeChannel(ChannelSubscriptionRequest request);

        /// <summary>
        /// Unsubscribes from a channel
        /// </summary>
        /// <param name="id">Client ID as returned by SubscribeChannel</param>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:#UnsubscribeChannel", RequestNamespace = "", ResponseNamespace = "", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
        void UnsubscribeChannel(string id);

        /// <summary>
        /// Refreshes client subscription, if required by transport
        /// </summary>
        /// <param name="id">ID of client as returned by SubscribeChannel</param>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:#RefreshSubscription", RequestNamespace = "", ResponseNamespace = "", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
        void RefreshSubscription(string id);
    }
}
