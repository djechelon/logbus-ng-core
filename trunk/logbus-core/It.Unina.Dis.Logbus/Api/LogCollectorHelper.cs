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
using It.Unina.Dis.Logbus.Utils;
using System.Collections;
using It.Unina.Dis.Logbus.Filters;
namespace It.Unina.Dis.Logbus.Api
{
    /// <summary>
    /// This class provides services for Log collectors that want to subscribe to Logbus channels
    /// </summary>
    public sealed class LogCollectorHelper
    {
        private LogCollectorHelper() { }

        public static string LogbusEndpointUrl
        {
            get;
            set;
        }

        public static ILogClient GetUdpClient(string logbusEndpointUrl, FilterBase filter)
        {
            ChannelManagement mgmt_object = new ChannelManagement()
            {
                Url = logbusEndpointUrl,
                UserAgent = string.Format("LogbusClient/{0}", typeof(LogCollectorHelper).Assembly.GetName().Version)
            };

            ArrayList channel_ids = new ArrayList(mgmt_object.ListChannels());
            string random_id;
            do
            {
                random_id = Randomizer.RandomAlphanumericString(5);
            } while (channel_ids.Contains(random_id));

            ChannelCreationInformation info = new ChannelCreationInformation();
            info.coalescenceWindow = 0;
            info.description = "Channel created by LogCollector";
            info.filter = filter;
            info.title = "AutoChannel";
            info.id = random_id;

            mgmt_object.CreateChannel(info);

            return new UdpLogClientImpl(random_id, LogbusEndpointUrl, true);

        }
    }
}
