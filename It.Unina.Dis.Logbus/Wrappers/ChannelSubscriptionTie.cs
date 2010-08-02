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

namespace It.Unina.Dis.Logbus.Wrappers
{
    /// <summary>
    /// Proxy for IChannelSubscription
    /// </summary>
    public sealed class ChannelSubscriptionTie
        : IChannelSubscription
    {

        /// <summary>
        /// Initializes ChannelSubscriptionTie with proxied object
        /// </summary>
        /// <param name="targetInstance">IChannelSubscription to be proxied</param>
        /// <exception cref="System.ArgumentNullException">targetInstance is null</exception>
        public ChannelSubscriptionTie(IChannelSubscription targetInstance)
        {
            if (targetInstance == null) throw new System.ArgumentNullException("targetInstance");
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
        public It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionResponse SubscribeChannel(It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionRequest request)
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
        public It.Unina.Dis.Logbus.RemoteLogbus.FilterDescription DescribeFilter(string filterid)
        {
            return target.DescribeFilter(filterid);
        }

        #endregion
    }
}
