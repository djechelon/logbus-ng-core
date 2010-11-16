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

using System.Runtime.CompilerServices;
using System;
using System.Net;
using System.Globalization;
using System.Security.Principal;
using It.Unina.Dis.Logbus.Wrappers;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using It.Unina.Dis.Logbus.Filters;
using System.Diagnostics;
#if MONO
using Mono.WebServer;
#endif
namespace It.Unina.Dis.Logbus.WebServices
{
    /// <summary>
    /// Activates and deactivates the Web Service listener for Logbus-ng
    /// </summary>
    /// <remarks>Credits to http://msdn.microsoft.com/en-us/magazine/cc163879.aspx</remarks>
    public sealed class WebServiceActivator
    {
        private const string ASMX_TEMPLATE = @"<%@ WebService Language=""C#"" Class=""{0}"" %>",
            GLOBAL_TEMPLATE = @"<%@ Application Inherits=""{0}"" Language=""C#"" %>";

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

        private static WebServiceActivator _instance;

        private readonly ILogBus _target;
        private readonly int _httpPort;
#if MONO
        private ApplicationServer _appserver;
#else
        private HttpListenerController _ctr;
#endif
        private string _physicalPath;

        private void StartService()
        {
            try
            {
                if (!AmIRoot())
                {
                    throw new LogbusException("In order to start Web Service the process must be run as super user");
                }

                string appPath = InstallRuntime();
#if MONO
                WebSource ws = new XSPWebSource(IPAddress.Any, _httpPort, true);

                _appserver = new ApplicationServer(ws, appPath);
                _appserver.AddApplication(null, _httpPort, "/", appPath);

                _appserver.GetSingleApp().AppHost = new XSPApplicationHost();
                _appserver.GetSingleApp().RequestBroker = new XSPRequestBroker();
                ((VPathToHost)_appserver.GetSingleApp()).CreateHost(_appserver, ws);

                AppDomain targetDomain = _appserver.AppHost.Domain;

                targetDomain.SetData("Logbus", (_target is MarshalByRefObject) ? (MarshalByRefObject)_target : new LogBusTie(_target));
                targetDomain.SetData("CustomFilterHelper", CustomFilterHelper.Instance);

                foreach (IPlugin plugin in _target.Plugins)
                {
                    MarshalByRefObject pluginRoot = plugin.GetPluginRoot();
                    if (pluginRoot != null) targetDomain.SetData(plugin.Name, pluginRoot);
                }

                _appserver.Start(true);

#else

                string[] prefixes = { string.Format(CultureInfo.InvariantCulture, "http://+:{0}/", _httpPort) };

                _ctr = new HttpListenerController(prefixes, "/", appPath);

                _ctr.Start();

                //If object is not marshalled by reference, use a wrapper, otherwise don't complicate object graph
                _ctr.Domain.SetData("Logbus", (_target is MarshalByRefObject) ? (MarshalByRefObject)_target : new LogBusTie(_target));
                _ctr.Domain.SetData("CustomFilterHelper", CustomFilterHelper.Instance);

                foreach (IPlugin plugin in _target.Plugins)
                {
                    MarshalByRefObject pluginRoot = plugin.GetPluginRoot();
                    if (pluginRoot != null) _ctr.Domain.SetData(plugin.Name, pluginRoot);
                }
#endif
            }
            catch (LogbusException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LogbusException("Unable to start web server", ex);
            }
        }

        private void StopService()
        {
#if MONO
            _appserver.Stop();
#else
            _ctr.Stop();
#endif
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
            if (_instance != null) throw new NotSupportedException("Currently, only one instance is supported");

            if (!HttpListener.IsSupported) throw new PlatformNotSupportedException("This action is not supported on this platform");
            try
            {
                _instance = new WebServiceActivator(service, httpPort);
                _instance.StartService();
            }
            catch (Exception) { _instance = null; throw; }
        }

        /// <summary>
        /// Stops the HTTP/WebService listener that is currently enabled for controlling Logbus
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Stop()
        {
            if (_instance == null) throw new InvalidOperationException("Web service listener is not started");
            try
            {
                _instance.StopService();
            }
            finally { _instance = null; }
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
                    string wsDeclaration = string.Format(ASMX_TEMPLATE,
                                                         typeof(ChannelManagementService).AssemblyQualifiedName);

                    using (
                        StreamWriter sw = new StreamWriter(File.Create(Path.Combine(_physicalPath, mgmFname)),
                                                           Encoding.Default))
                        sw.Write(wsDeclaration);
                }

                {
                    string wsDeclaration = string.Format(ASMX_TEMPLATE,
                                                         typeof(ChannelSubscriptionService).AssemblyQualifiedName);

                    using (
                        StreamWriter sw = new StreamWriter(File.Create(Path.Combine(_physicalPath, subFname)),
                                                           Encoding.Default))
                        sw.Write(wsDeclaration);
                }

                {
                    string globalDeclaration = string.Format(GLOBAL_TEMPLATE,
                                                             typeof(LogbusWebApplication).AssemblyQualifiedName);
                    using (
                        StreamWriter sw = new StreamWriter(File.Create(Path.Combine(_physicalPath, "Global.asax")),
                                                           Encoding.Default))
                        sw.Write(globalDeclaration);
                }

                string bindir = Path.Combine(fullpath, "bin");
                if (!Directory.Exists(bindir)) Directory.CreateDirectory(bindir);
                CopyAssemblyTo(GetType().Assembly, bindir);


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

                            string wsDeclaration = string.Format(ASMX_TEMPLATE, def.SkeletonType.AssemblyQualifiedName);

                            using (
                                StreamWriter sw = new StreamWriter(File.Create(Path.Combine(_physicalPath, fname + ".asmx")),
                                                                   Encoding.Default))
                                sw.Write(wsDeclaration);

                            //Copy skeleton asembly if needed
                            CopyAssemblyTo(def.SkeletonType.Assembly, bindir);
                            foreach (AssemblyName dependency in def.SkeletonType.Assembly.GetReferencedAssemblies())
                            {
                                try
                                {
                                    CopyAssemblyTo(Assembly.Load(dependency), bindir);
                                }
                                //Possible broken dependency
                                catch { }
                            }
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
        /// Copies given assembly to bin directory, if not in GAC
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="binDirectory"></param>
        private void CopyAssemblyTo(Assembly assembly, string binDirectory)
        {
            if (assembly.GlobalAssemblyCache) return;

            string codebase = assembly.Location;
            string finalName = Path.Combine(binDirectory, Path.GetFileName(codebase));

            if (!File.Exists(finalName)) File.Copy(codebase, finalName);
        }

        /// <summary>
        /// Uninstalls the ASP.NET files needed for WS responder
        /// </summary>
        /// <param name="path"></param>
        private void UninstallRuntime(string path)
        {
            Directory.Delete(path, true);
        }

        /// <summary>
        /// Determine if running as super user or not
        /// </summary>
        /// <returns></returns>
        private bool AmIRoot()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.WinCE:
                    return true; //Running Logbus-ng on a cell phone??
                case PlatformID.Xbox:
                    return true; //Would you ever run Logbus-ng as a game? :-S
                case PlatformID.Win32S:
                    throw new PlatformNotSupportedException();
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                    {
                        // ReSharper disable AssignNullToNotNullAttribute
                        WindowsPrincipal princ = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                        // ReSharper restore AssignNullToNotNullAttribute
                        return princ.IsInRole(WindowsBuiltInRole.Administrator) ||
                               princ.Identity.Name.EndsWith("Administrator");
                    }
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    {
                        /*
                        return Environment.UserName == "root";
                         * */
                        //Workaround to Mono bug 653564
                        using (Process whoami = new Process
                                                    {
                                                        StartInfo = new ProcessStartInfo("whoami")
                                                                        {
                                                                            UseShellExecute = false,
                                                                            RedirectStandardOutput = true
                                                                        }
                                                    })
                        {
                            whoami.Start();
                            whoami.WaitForExit();
                            return whoami.StandardOutput.ReadToEnd().Trim() == "root";
                        }
                    }
                default:
                    throw new PlatformNotSupportedException();
            }
        }
    }
}
