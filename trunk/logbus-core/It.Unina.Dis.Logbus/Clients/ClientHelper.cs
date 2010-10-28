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
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.RemoteLogbus;

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
            catch (LogbusConfigurationException)
            {
            }
        }


        /// <summary>
        /// Gets or sets the global client configuration
        /// </summary>
        /// <remarks>By default, it's loaded from App.config</remarks>
        public static LogbusClientConfiguration Configuration { get; set; }

        private static string UserAgent
        {
            get { return string.Format("LogbusClient/{0}", typeof (ClientHelper).Assembly.GetName().Version); }
        }

        /// <summary>
        /// Creates a default Channel Manager basing on configuration
        /// </summary>
        /// <returns></returns>
        public static IChannelManagement CreateChannelManager()
        {
            if (Configuration == null || Configuration.endpoint == null ||
                string.IsNullOrEmpty(Configuration.endpoint.managementUrl))
                throw new InvalidOperationException("Logbus is not configured for default client");
            return new ChannelManagement
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
            return new ChannelManagement
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
            if (Configuration == null || Configuration.endpoint == null ||
                string.IsNullOrEmpty(Configuration.endpoint.subscriptionUrl))
                throw new InvalidOperationException("Logbus is not configured for default client");
            return new ChannelSubscription
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
            return new ChannelSubscription
                       {
                           Url = endpointUrl,
                           UserAgent = UserAgent
                       };
        }

        #region UDP

        /// <summary>
        /// Creates a UDP log listener with the given filter
        /// </summary>
        /// <param name="manager">Management entpoint</param>
        /// <param name="subscription">Subscription endpoint</param>
        /// <param name="filter">Log filter to apply</param>
        /// <returns></returns>
        public static ILogClient CreateUnreliableClient(FilterBase filter, IChannelManagement manager,
                                                        IChannelSubscription subscription)
        {
            return new SyslogUdpClient(filter, manager, subscription);
        }

        /// <summary>
        /// Creates a UDP log listener that connects to the given channel
        /// </summary>
        /// <param name="channelId">ID of already-created channel</param>
        /// <param name="subscription">Subscription endpoint</param>
        /// <returns></returns>
        public static ILogClient CreateUnreliableClient(string channelId, IChannelSubscription subscription)
        {
            return new SyslogUdpClient(channelId, subscription);
        }


        /// <summary>
        /// Creates a client that listens to the specified channel using the default subscription endpoint
        /// </summary>
        /// <param name="channelId">ID of channel to subscribe to</param>
        /// <returns></returns>
        public static ILogClient CreateUnreliableClient(string channelId)
        {
            return new SyslogUdpClient(channelId, CreateChannelSubscriber());
        }

        /// <summary>
        /// Creates a client that listens to the newly created channel according to the specified filter
        /// </summary>
        /// <param name="filter">Filter to apply</param>
        /// <returns></returns>
        public static ILogClient CreateUnreliableClient(FilterBase filter)
        {
            if (Configuration == null || Configuration.endpoint == null ||
                string.IsNullOrEmpty(Configuration.endpoint.subscriptionUrl) ||
                string.IsNullOrEmpty(Configuration.endpoint.managementUrl))
                throw new InvalidOperationException("Logbus is not configured for default client");

            string mgmEndpoint = Configuration.endpoint.managementUrl,
                   subEndpoint = Configuration.endpoint.subscriptionUrl;

            IChannelManagement mgmObject = new ChannelManagement {Url = mgmEndpoint, UserAgent = UserAgent};
            IChannelSubscription subObject = new ChannelSubscription {Url = subEndpoint, UserAgent = UserAgent};

            return new SyslogUdpClient(filter, mgmObject, subObject);
        }

        #endregion

        #region TLS

        /// <summary>
        /// Creates a UDP log listener with the given filter
        /// </summary>
        /// <param name="manager">Management entpoint</param>
        /// <param name="subscription">Subscription endpoint</param>
        /// <param name="filter">Log filter to apply</param>
        /// <returns></returns>
        public static ILogClient CreateReliableClient(FilterBase filter, IChannelManagement manager,
                                                      IChannelSubscription subscription)
        {
            return new SyslogTlsClient(filter, manager, subscription);
        }

        /// <summary>
        /// Creates a UDP log listener that connects to the given channel
        /// </summary>
        /// <param name="channelId">ID of already-created channel</param>
        /// <param name="subscription">Subscription endpoint</param>
        /// <returns></returns>
        public static ILogClient CreateReliableClient(string channelId, IChannelSubscription subscription)
        {
            return new SyslogTlsClient(channelId, subscription);
        }


        /// <summary>
        /// Creates a client that listens to the specified channel using the default subscription endpoint
        /// </summary>
        /// <param name="channelId">ID of channel to subscribe to</param>
        /// <returns></returns>
        public static ILogClient CreateReliableClient(string channelId)
        {
            return new SyslogTlsClient(channelId, CreateChannelSubscriber());
        }

        /// <summary>
        /// Creates a client that listens to the newly created channel according to the specified filter
        /// </summary>
        /// <param name="filter">Filter to apply</param>
        /// <returns></returns>
        public static ILogClient CreateReliableClient(FilterBase filter)
        {
            if (Configuration == null || Configuration.endpoint == null ||
                string.IsNullOrEmpty(Configuration.endpoint.subscriptionUrl) ||
                string.IsNullOrEmpty(Configuration.endpoint.managementUrl))
                throw new InvalidOperationException("Logbus is not configured for default client");

            string mgmEndpoint = Configuration.endpoint.managementUrl,
                   subEndpoint = Configuration.endpoint.subscriptionUrl;

            IChannelManagement mgmObject = new ChannelManagement {Url = mgmEndpoint, UserAgent = UserAgent};
            IChannelSubscription subObject = new ChannelSubscription {Url = subEndpoint, UserAgent = UserAgent};

            return new SyslogTlsClient(filter, mgmObject, subObject);
        }

        #endregion

        #region Obsolete methods

        /// <summary>
        /// Creates a UDP log listener with the given filter
        /// </summary>
        /// <param name="manager">Management entpoint</param>
        /// <param name="subscription">Subscription endpoint</param>
        /// <param name="filter">Log filter to apply</param>
        /// <returns></returns>
        [Obsolete(
            "You should use CreateUnreliableClient(FilterBase, IChannelManagement, IChannelSubscription) or CreateReliableClient(FilterBase, IChannelManagement, IChannelSubscription) instead"
            )]
        public static ILogClient CreateDefaultClient(FilterBase filter, IChannelManagement manager,
                                                     IChannelSubscription subscription)
        {
            return CreateUnreliableClient(filter, manager, subscription);
        }

        /// <summary>
        /// Creates a UDP log listener that connects to the given channel
        /// </summary>
        /// <param name="channelId">ID of already-created channel</param>
        /// <param name="subscription">Subscription endpoint</param>
        /// <returns></returns>
        [Obsolete(
            "You should use CreateUnreliableClient(string, IChannelSubscription) or CreateReliableClient(string, IChannelSubscription) instead"
            )]
        public static ILogClient CreateDefaultClient(string channelId, IChannelSubscription subscription)
        {
            return CreateUnreliableClient(channelId, subscription);
        }


        /// <summary>
        /// Creates a client that listens to the specified channel using the default subscription endpoint
        /// </summary>
        /// <param name="channelId">ID of channel to subscribe to</param>
        /// <returns></returns>
        [Obsolete("You should use CreateUnreliableClient(string) or CreateReliableClient(string) instead")]
        public static ILogClient CreateDefaultClient(string channelId)
        {
            return CreateUnreliableClient(channelId);
        }

        /// <summary>
        /// Creates a client that listens to the newly created channel according to the specified filter
        /// </summary>
        /// <param name="filter">Filter to apply</param>
        /// <returns></returns>
        [Obsolete("You should use CreateUnreliableClient(FilterBase) or CreateReliableClient(FilterBase) instead")]
        public static ILogClient CreateDefaultClient(FilterBase filter)
        {
            return CreateUnreliableClient(filter);
        }

        #endregion
    }
}