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
using System.Threading;
using It.Unina.Dis.Logbus.Wrappers;
namespace It.Unina.Dis.Logbus.WebServices
{
    public class ChannelSubscriptionService
        : WebService, IChannelSubscription
    {

        public const string APPLICATION_KEY = "Logbus.ChannelSubscription";

        #region Constructor

        public ChannelSubscriptionService()
            : base()
        {
            if (Application[APPLICATION_KEY] != null)
                TargetChannelSubscription = Application[APPLICATION_KEY] as IChannelSubscription;
            else
            {
                object AppDomainData = Thread.GetDomain().GetData("Logbus");
                if (AppDomainData !=null)
                    try
                    {
                        TargetChannelSubscription = (AppDomainData is IChannelSubscription) ?
                            AppDomainData as IChannelSubscription : new Logbus2SoapAdapter((ILogBus)AppDomainData);
                    }
                    catch { }
            }
        }

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

        public string[] ListChannels()
        {
            return TargetChannelSubscription.ListChannels();
        }

        public string[] GetAvailableTransports()
        {
            return TargetChannelSubscription.GetAvailableTransports();
        }

        public ChannelSubscriptionResponse SubscribeChannel(ChannelSubscriptionRequest request)
        {
            try
            {
                return TargetChannelSubscription.SubscribeChannel(request);
            }
            catch { throw; }
        }

        public void UnsubscribeChannel(string id)
        {
            try
            {
                TargetChannelSubscription.UnsubscribeChannel(id);
            }
            catch { throw; }
        }

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
