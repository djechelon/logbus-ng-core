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

using System.Web.Hosting;
namespace It.Unina.Dis.Logbus.Wrappers
{
    /// <summary>
    /// Adapts the interface of ILogBus to SOAP callers
    /// </summary>
    public sealed class Logbus2SoapAdapter
        : System.MarshalByRefObject, IChannelManagement, IChannelSubscription
    {

        private ILogBus target;

        /// <summary>
        /// Initializes the wrapper with a properly initialized target
        /// </summary>
        /// <param name="targetInstance">Logbus service</param>
        public Logbus2SoapAdapter(ILogBus targetInstance)
        {
            if (targetInstance == null) throw new System.ArgumentNullException("targetInstance");
            target = targetInstance;
        }

        #region IChannelManagement Membri di

        string[] IChannelManagement.ListChannels()
        {
            string[] ret = new string[target.OutboundChannels.Count];
            int i = 0;
            foreach (IOutboundChannel chan in target.OutboundChannels)
            {
                ret[i] = chan.ID;
                i++;
            }
            return ret;
        }

        void IChannelManagement.CreateChannel(It.Unina.Dis.Logbus.RemoteLogbus.ChannelCreationInformation description)
        {
            target.CreateChannel(description.id, description.title, description.filter, description.description, description.coalescenceWindow);
        }

        It.Unina.Dis.Logbus.RemoteLogbus.ChannelInformation IChannelManagement.GetChannelInformation(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new System.ArgumentNullException("id");
            IOutboundChannel chan = null;

            foreach (IOutboundChannel ch in target.OutboundChannels)
                if (ch.ID == id)
                {
                    chan = ch;
                    break;
                }

            if (chan == null) return null; //Really?

            return new It.Unina.Dis.Logbus.RemoteLogbus.ChannelInformation()
            {
                clients = chan.SubscribedClients.ToString(),
                coalescenceWindow = (long)chan.CoalescenceWindowMillis,
                description = chan.Description,
                filter = chan.Filter as It.Unina.Dis.Logbus.Filters.FilterBase,
                id = chan.ID,
                title = chan.Name
            };
        }

        void IChannelManagement.DeleteChannel(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new System.ArgumentNullException("id");
            IOutboundChannel chan = null;

            foreach (IOutboundChannel ch in target.OutboundChannels)
                if (ch.ID == id)
                {
                    chan = ch;
                    break;
                }

            if (chan.SubscribedClients > 0) throw new System.InvalidOperationException("Unable to delete channels to which there are still subscribed clients");
            chan.Stop();
            target.OutboundChannels.Remove(chan);
        }

        #endregion

        #region IChannelSubscription Membri di

        string[] IChannelSubscription.ListChannels()
        {
            return (this as IChannelManagement).ListChannels();
        }

        string[] IChannelSubscription.GetAvailableTransports()
        {
            return target.AvailableTransports;
        }

        It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionResponse IChannelSubscription.SubscribeChannel(It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionRequest request)
        {
            System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> out_params;
            System.Collections.Generic.Dictionary<string, string> in_params = new System.Collections.Generic.Dictionary<string, string>();
            foreach (It.Unina.Dis.Logbus.RemoteLogbus.KeyValuePair kvp in request.param)
                in_params.Add(kvp.name, kvp.value);
            string clientid;
            try
            {
                clientid = target.SubscribeClient(request.channelid, request.transport, in_params, out out_params);
            }
            catch
            {
                throw;
            }

            It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionResponse ret = new It.Unina.Dis.Logbus.RemoteLogbus.ChannelSubscriptionResponse();
            ret.clientid = clientid;

            System.Collections.Generic.List<It.Unina.Dis.Logbus.RemoteLogbus.KeyValuePair> lst = new System.Collections.Generic.List<It.Unina.Dis.Logbus.RemoteLogbus.KeyValuePair>();
            foreach (System.Collections.Generic.KeyValuePair<string, string> kvp in out_params)
                lst.Add(new It.Unina.Dis.Logbus.RemoteLogbus.KeyValuePair() { name = kvp.Key, value = kvp.Value });
            ret.param = lst.ToArray();

            return ret;
        }

        void IChannelSubscription.UnsubscribeChannel(string id)
        {
            try
            {
                target.UnsubscribeClient(id);
            }
            catch
            {
                throw;
            }
        }

        void IChannelSubscription.RefreshSubscription(string id)
        {
            try
            {
                target.RefreshClient(id);
            }
            catch
            {
                throw;
            }
        }

        string[] IChannelSubscription.GetAvailableFilters()
        {
            throw new System.NotImplementedException();
        }

        It.Unina.Dis.Logbus.RemoteLogbus.FilterDescription IChannelSubscription.DescribeFilter(string filterid)
        {
            throw new System.NotImplementedException();
        }
        #endregion

    }
}
