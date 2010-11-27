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

using System;
using It.Unina.Dis.Logbus.RemoteLogbus;

namespace It.Unina.Dis.Logbus.Wrappers
{
    /// <summary>
    /// Proxy for IChannelSubscription
    /// </summary>
    public sealed class ChannelSubscriptionTie
        : MarshalByRefObject, IChannelSubscription
    {
        /// <summary>
        /// Initializes ChannelSubscriptionTie with proxied object
        /// </summary>
        /// <param name="targetInstance">IChannelSubscription to be proxied</param>
        /// <exception cref="System.ArgumentNullException">targetInstance is null</exception>
        public ChannelSubscriptionTie(IChannelSubscription targetInstance)
        {
            if (targetInstance == null) throw new ArgumentNullException("targetInstance");
            target = targetInstance;
        }

        private IChannelSubscription target;

        #region IChannelSubscription Membri di

        /// <remarks/>
        public string[] ListChannels()
        {
            return target.ListChannels();
        }

        /// <remarks/>
        public string[] GetAvailableTransports()
        {
            return target.GetAvailableTransports();
        }

        /// <remarks/>
        public ChannelSubscriptionResponse SubscribeChannel(ChannelSubscriptionRequest request)
        {
            return target.SubscribeChannel(request);
        }

        /// <remarks/>
        public void UnsubscribeChannel(string id)
        {
            target.UnsubscribeChannel(id);
        }

        /// <remarks/>
        public void RefreshSubscription(string id)
        {
            target.RefreshSubscription(id);
        }

        /// <remarks/>
        public string[] GetAvailableFilters()
        {
            return target.GetAvailableFilters();
        }

        /// <remarks/>
        public FilterDescription DescribeFilter(string filterid)
        {
            return target.DescribeFilter(filterid);
        }

        #endregion

        /// <remarks/>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}