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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace It.Unina.Dis.Logbus.Channels
{
    class SimpleOutChannel
        : IOutboundChannel
    {

        #region Private fields

        private Dictionary<string, IOutboundTransport> transports;

        /// <summary>
        /// Returns if channel is running or not
        /// </summary>
        public bool Running
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set;
        }
        #endregion


        #region Constructor/Destructor

        public SimpleOutChannel(string id, string name, Filters.IFilter filter)
        {
            this.ID = id;
            this.Name = name;
            this.Filter = filter;
        }

        #endregion

        #region IOutboundChannel Membri di

        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public void SubmitMessage(SyslogMessage message)
        {
            throw new System.NotImplementedException();
        }

        public int SubscribedClients
        {
            get {
                int ret = 0;
                foreach (KeyValuePair<string,IOutboundTransport> trans_kvp in transports) ret += trans_kvp.Value.SubscribedClients;
                return ret;
            }
        }

        public void Start()
        {
            transports = new Dictionary<string, IOutboundTransport>();

            throw new System.NotImplementedException();
        }

        public void Stop()
        {
            throw new System.NotImplementedException();

            transports = null;
        }

        public It.Unina.Dis.Logbus.Filters.IFilter Filter
        {
            get;
            set;
        }

        public ulong CoalescenceWindowMillis
        {
            get;
            set;
        }


        public ITransportFactoryHelper TransportFactoryHelper
        {
            set { throw new System.NotImplementedException(); }
        }

        public string SubscribeClient(string transportId, IEnumerable<KeyValuePair<string, string>> inputInstructions, out IEnumerable<KeyValuePair<string, string>> outputInstructions)
        {
            throw new System.NotImplementedException();
        }

        public void RefreshClient(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public void UnsubscribeClient(string clientId)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
