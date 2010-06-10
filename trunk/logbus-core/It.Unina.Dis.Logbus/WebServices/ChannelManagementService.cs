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
namespace It.Unina.Dis.Logbus.WebServices
{
    [WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    public class ChannelManagementService
        : WebService, IChannelManagement
    {

        protected ILogbusController TargetLogbus
        {
            get;
            set;
        }

        #region IChannelManagement Membri di

        public string[] ListChannels()
        {
            int tot = TargetLogbus.AvailableChannels.Length;
            string[] ret = new string[tot];
            for (int i = 0; i < tot; i++)
            {
                ret[i] = TargetLogbus.AvailableChannels[i].ID;
            }
            return ret;
        }

        public void CreateChannel(ChannelCreationInformation description)
        {
            try
            {
                TargetLogbus.CreateChannel(description.id, description.title, description.filter, description.description, description.coalescenceWindow);
            }
            catch { throw; } //What to do?
        }

        public ChannelInformation GetChannelInformation(string id)
        {
            IOutboundChannel chan = null;
            foreach (IOutboundChannel ch in TargetLogbus.AvailableChannels)
                if (ch.ID.Equals(id))
                {
                    chan = ch;
                    break;
                }

            if (chan == null) return null;

            ChannelInformation ret = new ChannelInformation()
            {
                id = chan.ID,
                title = chan.Name,
                description = chan.Description,
                coalescenceWindow = (long)chan.CoalescenceWindowMillis,
                filter = (FilterBase)chan.Filter,
                clients = chan.SubscribedClients.ToString(CultureInfo.InvariantCulture)
            };
            return ret;
        }

        public void DeleteChannel(string id)
        {
            try
            {
                TargetLogbus.RemoveChannel(id);
            }
            catch { throw; }
        }

        #endregion
    }
}
