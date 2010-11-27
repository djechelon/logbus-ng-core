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
    /// Interface used to manage channels on a Logbus node
    /// </summary>
    [WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    [GeneratedCode("wsdl", "2.0.50727.3038")]
    [WebServiceBinding(Name = "ChannelManagement", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    public interface IChannelManagement
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
        /// Creates a new channel with given properties
        /// </summary>
        /// <param name="channelcreation">Describes how the channel is structured</param>
        [WebMethod]
        [SoapDocumentMethod("urn:#CreateChannel", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        void CreateChannel([XmlElement("channel-creation", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] ChannelCreationInformation channelcreation);

        /// <summary>
        /// Retrieves channel information
        /// </summary>
        /// <param name="channelid">ID of channel</param>
        /// <returns>Information about the channel and its attributes</returns>
        [WebMethod]
        [SoapDocumentMethod("urn:#GetChannelInformation", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        [return: XmlElement("channel-info", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
        ChannelInformation GetChannelInformation([XmlElement("channel-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string channelid);

        /// <summary>
        /// Deletes a channel by ID
        /// </summary>
        /// <param name="channelid">Unique ID of channel</param>
        [WebMethod]
        [SoapDocumentMethod("urn:#DeleteChannel", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
        void DeleteChannel([XmlElement("channel-id", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string channelid);
    }
}