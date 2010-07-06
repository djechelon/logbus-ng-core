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
using It.Unina.Dis.Logbus.RemoteLogbus;
namespace It.Unina.Dis.Logbus.WebServices
{
    [WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    public class ChannelSubscriptionService
        : WebService, IChannelSubscription
    {

        #region Constructor

        public ChannelSubscriptionService()
            : base()
        { }

        public ChannelSubscriptionService(IChannelSubscription target)
            : base()
        {
            TargetChannelSubscription = target;
        }

        #endregion

        public virtual IChannelSubscription TargetChannelSubscription
        {
            get;
            set;
        }

        #region IChannelSubscription Membri di

        [WebMethod()]
        public string[] ListChannels()
        {
            return TargetChannelSubscription.ListChannels();
        }

        [WebMethod()]
        public string[] GetAvailableTransports()
        {
            return TargetChannelSubscription.GetAvailableTransports();
        }

        [WebMethod()]
        public ChannelSubscriptionResponse SubscribeChannel(ChannelSubscriptionRequest request)
        {
            try
            {
                return TargetChannelSubscription.SubscribeChannel(request);
            }
            catch { throw; }
        }

        [WebMethod()]
        public void UnsubscribeChannel(string id)
        {
            try
            {
                TargetChannelSubscription.UnsubscribeChannel(id);
            }
            catch { throw; }
        }

        [WebMethod()]
        public void RefreshSubscription(string id)
        {
            try
            {
                TargetChannelSubscription.RefreshSubscription(id);
            }
            catch { throw; }
        }

        #endregion
    }
}
