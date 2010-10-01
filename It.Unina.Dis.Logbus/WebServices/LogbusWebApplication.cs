﻿/*
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
namespace It.Unina.Dis.Logbus.WebServices
{
    public class LogbusWebApplication : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ILogBus logbus;
            object wrapper = AppDomain.CurrentDomain.GetData("Logbus");
            if (wrapper != null && wrapper is ILogBus)
                logbus = (ILogBus)wrapper;
            else
            {
                logbus = LogbusSingletonHelper.Instance;
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

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            (Application["LogbusInstance"] as IDisposable).Dispose();
        }
    }
}
