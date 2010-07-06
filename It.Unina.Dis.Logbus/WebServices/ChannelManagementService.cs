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
namespace It.Unina.Dis.Logbus.WebServices
{
    [WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    public class ChannelManagementService
        : WebService, IChannelManagement
    {

        #region Constructor

        public ChannelManagementService()
            : base()
        { }

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