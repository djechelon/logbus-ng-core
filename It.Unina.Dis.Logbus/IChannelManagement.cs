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
    [System.Web.Services.WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ChannelManagement", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    public interface IChannelManagement
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
        /// Creates a new channel with given properties
        /// </summary>
        /// <param name="description">Describes how the channel is structured</param>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:#CreateChannel", RequestNamespace = "", ResponseNamespace = "", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
        void CreateChannel(ChannelCreationInformation description);

        /// <summary>
        /// Retrieves channel information
        /// </summary>
        /// <param name="id">ID of channel</param>
        /// <returns>Information about the channel and its attributes</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:#GetChannelInformation", RequestNamespace = "", ResponseNamespace = "", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
        [return: System.Xml.Serialization.XmlElementAttribute("info")]
        ChannelInformation GetChannelInformation(string id);

        /// <summary>
        /// Deletes a channel by ID
        /// </summary>
        /// <param name="id">Unique ID of channel</param>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("urn:#DeleteChannel", RequestNamespace = "", ResponseNamespace = "", Use = System.Web.Services.Description.SoapBindingUse.Literal)]
        void DeleteChannel(string id);
    }
}
