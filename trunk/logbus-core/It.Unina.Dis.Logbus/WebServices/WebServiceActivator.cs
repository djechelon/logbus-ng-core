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

using System.Runtime.CompilerServices;
using System;
using System.Net;
using System.Globalization;
using It.Unina.Dis.Logbus.Wrappers;
using System.Web.Hosting;
using System.Web;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
namespace It.Unina.Dis.Logbus.WebServices
{
    /// <summary>
    /// Activates and deactivates the Web Service listener for Logbus-ng
    /// </summary>
    /// <remarks>Credits to http://msdn.microsoft.com/en-us/magazine/cc163879.aspx</remarks>
    public sealed class WebServiceActivator
    {
        private const string AsmxTemplate = @"<%@ WebService Language=""C#"" Class=""{0}"" %>";

        #region Constructor

        /// <remarks/>
        private WebServiceActivator(ILogBus instance, int port)
        {
            _target = instance; _httpPort = port;
        }

        /// <remarks/>
        ~WebServiceActivator()
        {
            try
            {
                StopService();
            }
            catch { }
        }
        #endregion

        private static WebServiceActivator instance
        {
            get;
            set;
        }

        private readonly ILogBus _target;
        private readonly int _httpPort;
        private HttpListenerController _ctr;
        private string _physicalPath;

        private void StartService()
        {
            string appPath = InstallRuntime();
            string[] prefixes = new string[] { string.Format(CultureInfo.InvariantCulture, "http://+:{0}/", _httpPort) };

            _ctr = new HttpListenerController(prefixes, "/", appPath);

            _ctr.Start();
            //If object is not marshalled by reference, use a wrapper, otherwise don't complicate object graph
            MarshalByRefObject wrapper = (_target is MarshalByRefObject) ? (MarshalByRefObject)_target : new LogBusTie(_target);
            _ctr.Domain.SetData("Logbus", wrapper);

            foreach (IPlugin plugin in _target.Plugins)
            {
                MarshalByRefObject pluginRoot = plugin.GetPluginRoot();
                if (pluginRoot != null) _ctr.Domain.SetData(plugin.Name, pluginRoot);
            }
        }

        private void StopService()
        {
            _ctr.Stop();
            UninstallRuntime(_physicalPath);
        }

        /// <summary>
        /// Starts an HTTP/WebService listener on the given port, controlling the default Logbus service instance
        /// </summary>
        /// <param name="httpPort">port to listen on</param>
        public static void Start(int httpPort)
        {
            Start(LogbusSingletonHelper.Instance, httpPort);
        }

        /// <summary>
        /// Starts an HTTP/WebService listener on the given port that controls the given Logbus service
        /// </summary>
        /// <param name="service">Logbus service to control</param>
        /// <param name="httpPort">HTTP port to listen on</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Start(ILogBus service, int httpPort)
        {
            if (instance != null) throw new NotSupportedException("Currently, only one instance is supported");
            if (!HttpListener.IsSupported) throw new PlatformNotSupportedException("This action is not supported on this platform");
            try
            {
                instance = new WebServiceActivator(service, httpPort);
                instance.StartService();
            }
            catch (Exception) { instance = null; throw; }
        }

        /// <summary>
        /// Stops the HTTP/WebService listener that is currently enabled for controlling Logbus
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Stop()
        {
            if (instance == null) throw new InvalidOperationException("Web service listener is not started");
            try
            {
                instance.StopService();
            }
            finally { instance = null; }
        }

        /// <summary>
        /// Installs the ASP.NET runtime needed for WS responder
        /// </summary>
        /// <returns></returns>
        /// <remarks>Uninstall method must be called upon dispose, otherwise junk files would be left in the file system</remarks>
        private string InstallRuntime()
        {
            try
            {
                string dirname = Path.GetRandomFileName();
                string fullpath = Path.Combine(Path.GetTempPath(), dirname);
                if (File.Exists(fullpath)) File.Delete(fullpath);

                //Create temporary directory for ASP.NET
                Directory.CreateDirectory(fullpath);
                _physicalPath = fullpath;

                //Copy .asmx files from assembly to directory
                const string mgmFname = "LogbusManagement.asmx";
                const string subFname = "LogbusSubscription.asmx";
                {
                    string wsDeclaration = string.Format(AsmxTemplate,
                                                         typeof(ChannelManagementService).AssemblyQualifiedName);

                    using (
                        StreamWriter sw = new StreamWriter(File.Create(Path.Combine(_physicalPath, mgmFname)),
                                                           Encoding.Default))
                        sw.Write(wsDeclaration);
                }

                {
                    string wsDeclaration = string.Format(AsmxTemplate,
                                                         typeof(ChannelSubscriptionService).AssemblyQualifiedName);

                    using (
                        StreamWriter sw = new StreamWriter(File.Create(Path.Combine(_physicalPath, subFname)),
                                                           Encoding.Default))
                        sw.Write(wsDeclaration);
                }

                string bindir = Path.Combine(fullpath, "bin");
                if (!GetType().Assembly.GlobalAssemblyCache)
                {
                    //Deploy assembly too
                    string codebase = Assembly.GetExecutingAssembly().Location;
                    if (!Directory.Exists(bindir)) Directory.CreateDirectory(bindir);
                    File.Copy(codebase, Path.Combine(bindir, Path.GetFileName(codebase)));
                }

                //Install plugins, if any
                foreach (IPlugin plugin in _target.Plugins)
                {
                    WsdlSkeletonDefinition[] defs = plugin.GetWsdlSkeletons();
                    if (defs != null)
                        foreach (WsdlSkeletonDefinition def in defs)
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

                            string wsDeclaration = string.Format(AsmxTemplate, def.SkeletonType.AssemblyQualifiedName);

                            using (
                                StreamWriter sw = new StreamWriter(File.Create(Path.Combine(_physicalPath, fname + ".asmx")),
                                                                   Encoding.Default))
                                sw.Write(wsDeclaration);

                            //Copy skeleton asembly if needed
                            Assembly skeletonAssembly = def.SkeletonType.Assembly;
                            if (skeletonAssembly.Equals(GetType().Assembly) || skeletonAssembly.GlobalAssemblyCache)
                                continue;
                            string codebase = def.SkeletonType.Assembly.Location;
                            string binpath = Path.Combine(bindir, Path.GetFileName(codebase));

                            if (!File.Exists(binpath)) File.Copy(codebase, binpath);
                        }
                }

                //Return the path
                return fullpath;
            }
            catch (LogbusException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LogbusException("Unable to install ASP.NET runtime", ex);
            }
        }

        /// <summary>
        /// Uninstalls the ASP.NET files needed for WS responder
        /// </summary>
        /// <param name="path"></param>
        private void UninstallRuntime(string path)
        {
            Directory.Delete(path, true);
        }
    }
}