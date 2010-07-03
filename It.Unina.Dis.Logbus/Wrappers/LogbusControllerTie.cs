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
    /// Apparently useless, this class implements delegation over a Logbus controller
    /// </summary>
    public class LogbusControllerTie
        : ILogbusController
    {

        private ILogbusController delegate_object;

        public LogbusControllerTie(ILogbusController target)
        {
            delegate_object = target;
        }

        #region ILogbusController Membri di

        public IOutboundChannel[] AvailableChannels
        {
            get { return delegate_object.AvailableChannels; }
        }

        public void CreateChannel(string id, string name, It.Unina.Dis.Logbus.Filters.IFilter filter, string description, long coalescenceWindow)
        {
            delegate_object.CreateChannel(id, name, filter, description, coalescenceWindow);
        }

        public void RemoveChannel(string id)
        {
            delegate_object.RemoveChannel(id);
        }

        public string[] AvailableTransports
        {
            get { return delegate_object.AvailableTransports; }
        }

        public string SubscribeClient(string channelId, string transportId, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> transportInstructions, out System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> clientInstructions)
        {
            return delegate_object.SubscribeClient(channelId, transportId, transportInstructions, out clientInstructions);
        }

        public void RefreshClient(string clientId)
        {
            delegate_object.RefreshClient(clientId);
        }

        public void UnsubscribeClient(string clientId)
        {
            delegate_object.UnsubscribeClient(clientId);
        }

        #endregion
    }
}