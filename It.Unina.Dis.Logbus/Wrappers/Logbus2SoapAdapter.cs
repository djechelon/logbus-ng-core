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
using System.Collections.Generic;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.RemoteLogbus;

namespace It.Unina.Dis.Logbus.Wrappers
{
    /// <summary>
    /// Adapts the interface of ILogBus to SOAP callers
    /// </summary>
    public sealed class Logbus2SoapAdapter
        : MarshalByRefObject, IChannelManagement, IChannelSubscription
    {
        private readonly ILogBus _target;

        /// <summary>
        /// Initializes the wrapper with a properly initialized target
        /// </summary>
        /// <param name="targetInstance">Logbus service</param>
        public Logbus2SoapAdapter(ILogBus targetInstance)
        {
            if (targetInstance == null) throw new ArgumentNullException("targetInstance");
            _target = targetInstance;
        }

        #region IChannelManagement Membri di

        string[] IChannelManagement.ListChannels()
        {
            string[] ret = new string[_target.OutboundChannels.Count];
            int i = 0;
            foreach (IOutboundChannel chan in _target.OutboundChannels)
            {
                ret[i] = chan.ID;
                i++;
            }
            return ret;
        }

        void IChannelManagement.CreateChannel(ChannelCreationInformation description)
        {
            _target.CreateChannel(description.id, description.title, description.filter, description.description,
                                  description.coalescenceWindow);
        }

        ChannelInformation IChannelManagement.GetChannelInformation(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            IOutboundChannel chan = null;

            foreach (IOutboundChannel ch in _target.OutboundChannels)
                if (ch.ID == id)
                {
                    chan = ch;
                    break;
                }

            if (chan == null) return null; //Really?

            return new ChannelInformation
                       {
                           clients = chan.SubscribedClients.ToString(),
                           coalescenceWindow = (long) chan.CoalescenceWindowMillis,
                           description = chan.Description,
                           filter = chan.Filter as FilterBase,
                           id = chan.ID,
                           title = chan.Name
                       };
        }

        void IChannelManagement.DeleteChannel(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");
            IOutboundChannel chan = null;

            foreach (IOutboundChannel ch in _target.OutboundChannels)
                if (ch.ID == id)
                {
                    chan = ch;
                    break;
                }

            if (chan.SubscribedClients > 0)
                throw new InvalidOperationException(
                    "Unable to delete channels to which there are still subscribed clients");
            chan.Stop();
            _target.OutboundChannels.Remove(chan);
        }

        #endregion

        #region IChannelSubscription Membri di

        string[] IChannelSubscription.ListChannels()
        {
            return (this as IChannelManagement).ListChannels();
        }

        string[] IChannelSubscription.GetAvailableTransports()
        {
            return _target.AvailableTransports;
        }

        ChannelSubscriptionResponse IChannelSubscription.SubscribeChannel(ChannelSubscriptionRequest request)
        {
            IEnumerable<KeyValuePair<string, string>> out_params;
            Dictionary<string, string> in_params = new Dictionary<string, string>();
            foreach (KeyValuePair kvp in request.param)
                in_params.Add(kvp.name, kvp.value);
            string clientid;
            try
            {
                clientid = _target.SubscribeClient(request.channelid, request.transport, in_params, out out_params);
            }
            catch
            {
                throw;
            }

            ChannelSubscriptionResponse ret = new ChannelSubscriptionResponse();
            ret.clientid = clientid;

            List<KeyValuePair> lst = new List<KeyValuePair>();
            foreach (KeyValuePair<string, string> kvp in out_params)
                lst.Add(new KeyValuePair {name = kvp.Key, value = kvp.Value});
            ret.param = lst.ToArray();

            return ret;
        }

        void IChannelSubscription.UnsubscribeChannel(string id)
        {
            try
            {
                _target.UnsubscribeClient(id);
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
                _target.RefreshClient(id);
            }
            catch
            {
                throw;
            }
        }

        string[] IChannelSubscription.GetAvailableFilters()
        {
            throw new NotImplementedException();
        }

        FilterDescription IChannelSubscription.DescribeFilter(string filterid)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}