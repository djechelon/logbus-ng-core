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
    /// Interface used to manage channels on a Logbus node
    /// </summary>
#if MONO
#else
    /// <remarks/>
    [System.Web.Services.WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ChannelManagement", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
    public interface IChannelManagement
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
        /// Creates a new channel with given properties
        /// </summary>
        /// <param name="description">Describes how the channel is structured</param>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#CreateChannel", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
#endif
        void CreateChannel([System.Xml.Serialization.XmlElementAttribute("channel-creation", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] ChannelCreationInformation channelcreation);

        /// <summary>
        /// Retrieves channel information
        /// </summary>
        /// <param name="id">ID of channel</param>
        /// <returns>Information about the channel and its attributes</returns>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetChannelInformation", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("channel-info", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        ChannelInformation GetChannelInformation([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string message);

        /// <summary>
        /// Deletes a channel by ID
        /// </summary>
        /// <param name="id">Unique ID of channel</param>
#if MONO
#else
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#DeleteChannel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
#endif
        void DeleteChannel([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string message);
    }
}
