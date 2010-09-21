using System;
using It.Unina.Dis.Logbus;
using It.Unina.Dis.Logbus.WebServices;

namespace LogbusWebapp
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            ILogBus logbus = LogbusSingletonHelper.Instance;
            logbus.Start();
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