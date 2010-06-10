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
namespace It.Unina.Dis.Logbus.WebServices
{
    [WebService(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl/")]
    public class ChannelSubscriptionService
        : WebService, IChannelSubscription
    {
        #region IChannelSubscription Membri di

        public string[] ListChannels()
        {
            throw new System.NotImplementedException();
        }

        public string[] GetAvailableTransports()
        {
            throw new System.NotImplementedException();
        }

        public ChannelSubscriptionResponse SubscribeChannel(ChannelSubscriptionRequest request)
        {
            throw new System.NotImplementedException();
        }

        public void UnsubscribeChannel(string id)
        {
            throw new System.NotImplementedException();
        }

        public void RefreshSubscription(string id)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
