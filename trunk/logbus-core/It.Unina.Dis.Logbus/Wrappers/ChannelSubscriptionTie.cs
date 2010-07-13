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
    public sealed class ChannelSubscriptionTie
        : IChannelSubscription
    {

        public ChannelSubscriptionTie(IChannelSubscription targetInstance)
        {
            target = targetInstance;
        }

        private IChannelSubscription target;

        #region IChannelSubscription Membri di

        public string[] ListChannels()
        {
            return target.ListChannels();
        }

        public string[] GetAvailableTransports()
        {
            return target.GetAvailableTransports();
        }

        public It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionResponse SubscribeChannel(It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionRequest request)
        {
            return target.SubscribeChannel(request);
        }

        public void UnsubscribeChannel(string id)
        {
            target.UnsubscribeChannel(id);
        }

        public void RefreshSubscription(string id)
        {
            target.RefreshSubscription(id);
        }

        #endregion
    }
}
