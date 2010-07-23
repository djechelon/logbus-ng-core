﻿/*
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
using System.Web.Services.Protocols;
using System;
using It.Unina.Dis.Logbus.Filters;
using System.Globalization;
using It.Unina.Dis.Logbus.RemoteLogbus;
using System.Threading;
using It.Unina.Dis.Logbus.Wrappers;

namespace It.Unina.Dis.Logbus.WebServices
{
    [System.Web.Services.WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ChannelManagement", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    public class ChannelManagementService
        : WebService, IChannelManagement
    {

        public const string APPLICATION_KEY = "Logbus.ChannelManagement";

        #region Constructor

        public ChannelManagementService()
            : base()
        {
            if (Application[APPLICATION_KEY] != null)
                TargetChannelManager = Application[APPLICATION_KEY] as IChannelManagement;
            else
            {
                object AppDomainData = Thread.GetDomain().GetData("Logbus");
                if (AppDomainData != null)
                    try
                    {
                        TargetChannelManager = (AppDomainData is IChannelManagement) ?
                            AppDomainData as IChannelManagement : new Logbus2SoapAdapter((ILogBus)AppDomainData);
                    }
                    catch { }
            }
        }

        public ChannelManagementService(IChannelManagement target)
            : base()
        {
            TargetChannelManager = target;
        }

        #endregion

        public virtual IChannelManagement TargetChannelManager
        {
            get;
            set;
        }

        #region IChannelManagement Membri di

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#ListChannels", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("string-array", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        public string[] ListChannels()
        {
            return TargetChannelManager.ListChannels();
        }

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#CreateChannel", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
#endif
        public void CreateChannel([System.Xml.Serialization.XmlElementAttribute("channel-creation", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] ChannelCreationInformation channelcreation)
        {
            try
            {
                TargetChannelManager.CreateChannel(channelcreation);
            }
            catch { throw; } //What to do?
        }

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetChannelInformation", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("channel-info", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
#endif
        public ChannelInformation GetChannelInformation([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string message)
        {
            try
            {
                return TargetChannelManager.GetChannelInformation(message);
            }
            catch { throw; }
        }

#if MONO
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#DeleteChannel", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
#endif
        public void DeleteChannel([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")] string message)
        {
            try
            {
                TargetChannelManager.DeleteChannel(message);
            }
            catch { throw; }
        }

        #endregion
    }
}
