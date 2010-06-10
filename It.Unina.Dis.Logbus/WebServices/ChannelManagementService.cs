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
    public class ChannelManagementService
        :WebService, IChannelManagement
    {

        protected ILogbusController TargetLogbus
        {
            get;
            set;
        }

        #region IChannelManagement Membri di

        public string[] ListChannels()
        {
            throw new System.NotImplementedException();
        }

        public void CreateChannel(ChannelCreationInformation description)
        {
            throw new System.NotImplementedException();
        }

        public ChannelInformation GetChannelInformation(string id)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteChannel(string id)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
