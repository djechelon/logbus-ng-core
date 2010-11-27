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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System;
using System.Web.Services.Protocols;
using System.Reflection;
namespace It.Unina.Dis.Logbus.WebServices
{
    /// <summary>
    /// Handles Web Service protocol mappings
    /// </summary>
    internal sealed class LogbusHttpModule
        : IHttpModule
    {
        private const string HTTP_APP_KEY = "LogbusHttpModule.Collection";

        private Dictionary<string, Type> _mappings;

        #region IHttpModule Membri di

        /// <remarks/>
        public void Dispose()
        { }

        /// <remarks/>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequest;

            _mappings = context.Application[HTTP_APP_KEY] as Dictionary<string, Type>;

            if (_mappings == null) throw new InvalidOperationException("Could not find web service mappings");
        }

        #endregion

        void BeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;

            string fpath = app.Context.Request.FilePath;

            if (string.IsNullOrEmpty(fpath))
            {
                app.Context.RemapHandler(
                    (IHttpHandler)Activator.CreateInstance(Type.GetType("System.Web.HttpNotFoundHandler, System.Web")));
            }
            else
            {
                //Remove .asmx

                if (!_mappings.ContainsKey(fpath))
                {
                    //File not found
                    app.Context.RemapHandler(
                    (IHttpHandler)Activator.CreateInstance(Type.GetType("System.Web.HttpNotFoundHandler, System.Web")));
                }
                else
                {
                    //Web service handler
                    WebServiceHandlerFactory factory = new WebServiceHandlerFactory();
                    MethodInfo mInfo = factory.GetType().GetMethod("CoreGetHandler");
                    IHttpHandler handlerToUse =
                        (IHttpHandler)
                        mInfo.Invoke(factory, new object[] { _mappings[fpath], app.Context, app.Request, app.Response });
                    app.Context.RemapHandler(handlerToUse);
                }
            }
        }

        /// <summary>
        /// Configures web service mappings
        /// </summary>
        /// <param name="plugins"></param>
        /// <param name="application"></param>
        public static void ConfigureMappings(IEnumerable<IPlugin> plugins, HttpApplication application)
        {
            Dictionary<string, Type> webservicesType = new Dictionary<string, Type>
                                                         {
                                                             {"LogbusManagement", typeof (ChannelManagementService)},
                                                             {"LogbusSubscription", typeof (ChannelSubscriptionService)}
                                                         };

            if (plugins != null)
                foreach (IPlugin plugin in plugins)
                {
                    WsdlSkeletonDefinition[] skeletons = plugin.GetWsdlSkeletons();
                    if (skeletons != null)
                        foreach (WsdlSkeletonDefinition def in skeletons)
                        {
                            if (def.SkeletonType == null)
                                throw new LogbusException(string.Format("Plugin {0} declares empty skeleton type",
                                                                        plugin.Name));
                            if (def.SkeletonType.IsAssignableFrom(typeof(System.Web.Services.WebService)))
                                throw new LogbusException(
                                    string.Format("Plugin {0} does not declare a valid WSDL skeleton type", plugin.Name));

                            string fname = def.UrlFileName;
                            if (fname.EndsWith(".asmx", false, CultureInfo.InvariantCulture))
                                fname = fname.Substring(0, fname.Length - 5);

                            if (!Regex.IsMatch(fname, @"^[a-zA-Z0-9_\.\-%]+$", RegexOptions.CultureInvariant))
                                throw new LogbusException(string.Format(
                                    "Plugin {0} declares invalid WSDL endpoint: {1}",
                                    plugin.Name, def.UrlFileName));

                            if (webservicesType.ContainsKey(fname))
                                throw new LogbusException(string.Format("Endpoint {0} already assigned to another plugin", fname));

                            webservicesType.Add(fname, def.SkeletonType);
                        }
                }

            application.Application.Lock();
            try
            {
                application.Application.Add(HTTP_APP_KEY, webservicesType);
            }
            finally
            {
                application.Application.UnLock();
            }
        }
    }
}
