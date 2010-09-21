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

using It.Unina.Dis.Logbus.RemoteLogbus;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Configuration;
using System;

namespace It.Unina.Dis.Logbus.Clients
{
    /// <summary>
    /// This class provides services for Log collectors that want to subscribe to Logbus channels
    /// </summary>
    public static class ClientHelper
    {
        static ClientHelper()
        {
            try
            {
                Configuration = ConfigurationHelper.ClientConfiguration;
            }
            catch (LogbusConfigurationException) { }
        }


        /// <summary>
        /// Gets or sets the global client configuration
        /// </summary>
        /// <remarks>By default, it's loaded from App.config</remarks>
        public static LogbusClientConfiguration Configuration
        {
            get;
            set;
        }

        private static string UserAgent
        {
            get
            {
                return string.Format("LogbusClient/{0}", typeof(ClientHelper).Assembly.GetName().Version);
            }
        }

        /// <summary>
        /// Creates a default Channel Manager basing on configuration
        /// </summary>
        /// <returns></returns>
        public static IChannelManagement CreateChannelManager()
        {
            if (Configuration == null || Configuration.endpoint == null || string.IsNullOrEmpty(Configuration.endpoint.managementUrl)) throw new InvalidOperationException("Logbus is not configured for default client");
            return new ChannelManagement()
            {
                Url = Configuration.endpoint.managementUrl,
                UserAgent = UserAgent
            };
        }

        /// <summary>
        /// Creates a Channel Manager bound to the given SOAP endpoint
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <returns></returns>
        public static IChannelManagement CreateChannelManager(string endpointUrl)
        {
            return new ChannelManagement()
            {
                Url = endpointUrl,
                UserAgent = UserAgent
            };
        }

        /// <summary>
        /// Creates a default Channel Subscriber basing on configuration
        /// </summary>
        /// <returns></returns>
        public static IChannelSubscription CreateChannelSubscriber()
        {
            if (Configuration == null || Configuration.endpoint == null || string.IsNullOrEmpty(Configuration.endpoint.subscriptionUrl)) throw new InvalidOperationException("Logbus is not configured for default client");
            return new ChannelSubscription()
            {
                Url = Configuration.endpoint.subscriptionUrl,
                UserAgent = UserAgent
            };
        }

        /// <summary>
        /// Creates a Channel Subscriber bound to the given SOAP endpoint
        /// </summary>
        /// <param name="endpointUrl"></param>
        /// <returns></returns>
        public static IChannelSubscription CreateChannelSubscriber(string endpointUrl)
        {
            return new ChannelSubscription()
            {
                Url = endpointUrl,
                UserAgent = UserAgent
            };
        }

        /// <summary>
        /// Creates a UDP log listener with the given filter
        /// </summary>
        /// <param name="manager">Management entpoint</param>
        /// <param name="subscription">Subscription endpoint</param>
        /// <param name="filter">Log filter to apply</param>
        /// <returns></returns>
        public static ILogClient CreateDefaultClient(FilterBase filter, IChannelManagement manager, IChannelSubscription subscription)
        {
            return new UdpLogClientImpl(filter, manager, subscription);
        }

        /// <summary>
        /// Creates a UDP log listener that connects to the given channel
        /// </summary>
        /// <param name="channelId">ID of already-created channel</param>
        /// <param name="subscription">Subscription endpoint</param>
        /// <returns></returns>
        public static ILogClient CreateDefaultClient(string channelId, IChannelSubscription subscription)
        {
            return new UdpLogClientImpl(channelId, subscription);
        }


        /// <summary>
        /// Creates a client that listens to the specified channel using the default subscription endpoint
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public static ILogClient CreateDefaultClient(string channelId)
        {
            return new UdpLogClientImpl(channelId, CreateChannelSubscriber());
        }

        /// <summary>
        /// Creates a client that listens to the newly created channel according to the specified filter
        /// </summary>
        /// <param name="filter">Filter for </param>
        /// <returns></returns>
        public static ILogClient CreateDefaultClient(FilterBase filter)
        {
            if (Configuration == null || Configuration.endpoint == null || string.IsNullOrEmpty(Configuration.endpoint.subscriptionUrl) || string.IsNullOrEmpty(Configuration.endpoint.managementUrl)) throw new InvalidOperationException("Logbus is not configured for default client");

            string mgm_endpoint = Configuration.endpoint.managementUrl, sub_endpoint = Configuration.endpoint.subscriptionUrl;

            IChannelManagement mgm_object = new ChannelManagement() { Url = mgm_endpoint, UserAgent = UserAgent };
            IChannelSubscription sub_object = new ChannelSubscription() { Url = sub_endpoint, UserAgent = UserAgent };

            return new UdpLogClientImpl(filter, mgm_object, sub_object);
        }
    }
}
