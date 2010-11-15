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
    /// <summary>
    /// Default implementation of IChannelSubscription skeleton
    /// </summary>
    [WebServiceBinding(Name = "ChannelSubscription", Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    [WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    public class ChannelSubscriptionService
        : WebService, IChannelSubscription
    {
        /// <summary>
        /// Key with which store IChannelSubscription into ASP.NET Application object
        /// </summary>
        public const string APPLICATION_KEY = "Logbus.ChannelSubscription";

        #region Constructor

        /// <remarks/>
        public ChannelSubscriptionService()
        {
            if (Application[APPLICATION_KEY] != null && Application[APPLICATION_KEY] is IChannelSubscription)
                TargetChannelSubscription = (IChannelSubscription) Application[APPLICATION_KEY];
            else
            {
                throw new LogbusException("Logbus instance not set by Global.asax");
            }
        }

        /// <summary>
        /// Initialize ChannelSubscriptionService with a target
        /// </summary>
        /// <param name="target">Target to proxy</param>
        public ChannelSubscriptionService(IChannelSubscription target)
        {
            TargetChannelSubscription = target;
        }

        #endregion

        /// <summary>
        /// Target IChannelSubscription to proxy
        /// </summary>
        public IChannelSubscription TargetChannelSubscription { get; set; }

        #region IChannelSubscription Membri di

        /// <remarks/>
        public virtual string[] ListChannels()
        {
            return TargetChannelSubscription.ListChannels();
        }

        /// <remarks/>
        public virtual string[] GetAvailableTransports()
        {
            return TargetChannelSubscription.GetAvailableTransports();
        }

        /// <remarks/>
        public virtual ChannelSubscriptionResponse SubscribeChannel(ChannelSubscriptionRequest channelsubrequest)
        {
            return TargetChannelSubscription.SubscribeChannel(channelsubrequest);
        }

        /// <remarks/>
        public virtual void UnsubscribeChannel( string clientid)
        {
            TargetChannelSubscription.UnsubscribeChannel(clientid);
        }

        /// <remarks/>
        public virtual void RefreshSubscription(string clientid)
        {
            TargetChannelSubscription.RefreshSubscription(clientid);
        }

        /// <remarks/>
        public virtual string[] GetAvailableFilters()
        {
            return TargetChannelSubscription.GetAvailableFilters();
        }

        /// <remarks/>
        public virtual FilterDescription DescribeFilter(string filterid)
        {
            return TargetChannelSubscription.DescribeFilter(filterid);
        }

        #endregion
    }
}