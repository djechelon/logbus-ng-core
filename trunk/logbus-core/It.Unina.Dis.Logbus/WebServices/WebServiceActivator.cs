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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Net;
using System.Globalization;
using System.Security.Principal;
using System.Threading;
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
    internal sealed class WebServiceActivator : IAsyncRunnable, IDisposable
    {
        private const string ASMX_TEMPLATE = @"<%@ WebService Language=""C#"" Class=""{0}"" %>",
            GLOBAL_TEMPLATE = @"<%@ Application Inherits=""{0}"" Language=""C#"" %>";

        #region Constructor

        private WebServiceActivator()
        {
            _startDelegate = new ThreadStart(Start);
            _stopDelegate = new ThreadStart(Stop);
        }

        /// <remarks/>
        public WebServiceActivator(ILogBus instance, int port)
            : this()
        {
            _target = instance; HttpPort = port;
        }

        public WebServiceActivator(ILogBus instance)
            : this()
        {
            _target = instance;
        }

        /// <remarks/>
        ~WebServiceActivator()
        {
            Dispose();
        }
        #endregion

        public bool Disposed { get; set; }

        private readonly ILogBus _target;

        /// <summary>
        /// Gets or sets HTTP port the server listens on
        /// </summary>
        public int HttpPort { get; set; }


#if MONO
        private ApplicationServer _appserver;
#else
        private HttpListenerController _ctr;
#endif
        private string _physicalPath;

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

        #region IDisposable Membri di

        public void Dispose()
        {
            if (Disposed) return;
            GC.SuppressFinalize(this);

            try
            {
                Stop();
            }
            catch (InvalidOperationException) { }

            Disposed = true;
        }
        #endregion

        #region IRunnable Membri di

        public event EventHandler<CancelEventArgs> Starting;

        public event EventHandler<CancelEventArgs> Stopping;

        public event EventHandler Started;

        public event EventHandler Stopped;

        public event UnhandledExceptionEventHandler Error;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (Running) throw new NotSupportedException("Web service already running");
            if (Starting != null)
            {
                CancelEventArgs e = new CancelEventArgs(false);
                Starting(this, e);
                if (e.Cancel) return;
            }
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

                string[] prefixes = { string.Format(CultureInfo.InvariantCulture, "http://+:{0}/", HttpPort) };

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
                Running = true;
                if (Started != null) Started(this, EventArgs.Empty);
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (!Running) throw new InvalidOperationException("Web service not running");
            if (Stopping != null)
            {
                CancelEventArgs e = new CancelEventArgs(false);
                Stopping(this, e);
                if (e.Cancel) return;
            }
#if MONO
            _appserver.Stop();
#else
            _ctr.Stop();
#endif
            UninstallRuntime(_physicalPath);
            Running = false;

            if (Stopped != null) Stopped(this, EventArgs.Empty);
        }

        public bool Running
        {
            get;
            private set;
        }

        #endregion

        #region IAsyncRunnable Membri di

        public IAsyncResult BeginStart()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            return _startDelegate.BeginInvoke(null, null);
        }

        public void EndStart(IAsyncResult result)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            _startDelegate.EndInvoke(result);
        }

        public IAsyncResult BeginStop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            return _stopDelegate.BeginInvoke(null, null);
        }

        public void EndStop(IAsyncResult result)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);

            _stopDelegate.EndInvoke(result);
        }

        private readonly ThreadStart _startDelegate, _stopDelegate;

        #endregion
    }
}
