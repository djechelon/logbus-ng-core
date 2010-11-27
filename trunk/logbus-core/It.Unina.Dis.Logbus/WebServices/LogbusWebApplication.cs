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
using System.Web;
using It.Unina.Dis.Logbus.Wrappers;

namespace It.Unina.Dis.Logbus.WebServices
{
    /// <summary>
    /// Web application class for Global.asax
    /// </summary>
    public class LogbusWebApplication : HttpApplication
    {
        private bool _standalone;

        /// <remarks/>
        protected void Application_Start(object sender, EventArgs e)
        {
            ILogBus logbus;
            object wrapper = AppDomain.CurrentDomain.GetData("Logbus");
            if (wrapper != null && wrapper is ILogBus)
                logbus = (ILogBus)wrapper;
            else
            {
                _standalone = true;
                logbus = LogbusSingletonHelper.Instance;
                AppDomain.CurrentDomain.SetData("Logbus",
                                                (logbus is MarshalByRefObject) ? logbus : new LogBusTie(logbus));
                logbus.Start();
            }

            try
            {
                Application.Lock();

                Application[ChannelManagementService.APPLICATION_KEY] = logbus;
                Application[ChannelSubscriptionService.APPLICATION_KEY] = logbus;
                Application["LogbusInstance"] = logbus;
            }
            finally
            {
                Application.UnLock();
            }

            //Load plugins root proxy
            if (logbus.Plugins != null)
            {
                foreach (IPlugin plugin in logbus.Plugins)
                {
                    if (plugin == null) continue; //Should never happen by design
                    MarshalByRefObject pluginRoot = plugin.GetPluginRoot();

                    if (pluginRoot != null) AppDomain.CurrentDomain.SetData(plugin.Name, pluginRoot);
                }
            }
        }

        /// <remarks/>
        protected void Session_Start(object sender, EventArgs e)
        {
        }

        /// <remarks/>
        protected void Session_End(object sender, EventArgs e)
        {
        }

        /// <remarks/>
        protected void Application_End(object sender, EventArgs e)
        {
            if (_standalone) ((IDisposable)Application["LogbusInstance"]).Dispose();
        }
    }
}