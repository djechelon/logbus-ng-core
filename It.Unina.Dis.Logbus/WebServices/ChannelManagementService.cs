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
using System.Web.Services.Protocols;
using System;
using It.Unina.Dis.Logbus.Filters;
using System.Globalization;
using It.Unina.Dis.Logbus.RemoteLogbus;
using System.Threading;
using It.Unina.Dis.Logbus.Wrappers;

namespace It.Unina.Dis.Logbus.WebServices
{
    [WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
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

        [WebMethod()]
        public string[] ListChannels()
        {
            return TargetChannelManager.ListChannels();
        }

        [WebMethod()]
        public void CreateChannel(ChannelCreationInformation description)
        {
            try
            {
                TargetChannelManager.CreateChannel(description);
            }
            catch { throw; } //What to do?
        }

        [WebMethod()]
        public ChannelInformation GetChannelInformation(string id)
        {
            try
            {
                return TargetChannelManager.GetChannelInformation(id);
            }
            catch { throw; }
        }

        [WebMethod()]
        public void DeleteChannel(string id)
        {
            try
            {
                TargetChannelManager.DeleteChannel(id);
            }
            catch { throw; }
        }

        #endregion
    }
}
