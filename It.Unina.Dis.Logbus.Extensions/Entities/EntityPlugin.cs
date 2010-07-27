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

namespace It.Unina.Dis.Logbus.Entities
{
    public sealed class EntityPlugin
        :IPlugin
    {
        #region IPlugin Membri di

        public void Register(ILogBus logbus)
        {
            logbus.MessageReceived += new SyslogMessageEventHandler(logbus_MessageReceived);
        }

        void logbus_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public void Unregister()
        {
            throw new System.NotImplementedException();
        }

        public It.Unina.Dis.Logbus.Loggers.ILog Log
        {
            private get;
            set;
        }

        public string Name
        {
            get { return "Logbus.EntityManager"; }
        }

        public WsdlSkeletonDefinition[] GetWsdlSkeletons()
        {
            throw new System.NotImplementedException();
        }

        public System.MarshalByRefObject GetPluginRoot()
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
