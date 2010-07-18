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
using It.Unina.Dis.Logbus.Wrappers;
using System.Web.Hosting;
using System.Web;
using System.IO;
using System.Reflection;
using System.Text;
namespace It.Unina.Dis.Logbus.WebServices
{
    /// <summary>
    /// Activates and deactivates the Web Service listener for Logbus-ng
    /// </summary>
    /// <remarks>Credits to http://msdn.microsoft.com/en-us/magazine/cc163879.aspx</remarks>
    public sealed class WebServiceActivator
    {
        #region Constructor
        static WebServiceActivator()
        {
        }

        private WebServiceActivator(ILogBus instance, int port)
        {
            target = instance; httpPort = port;
        }

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

        private ILogBus target;
        private int httpPort;
        private HttpListenerController ctr;
        private string physical_path;
        private Logbus2SoapAdapter adapter;

        private void StartService()
        {
            string app_path = InstallRuntime();
            string[] prefixes = new string[] { string.Format(CultureInfo.InvariantCulture, "http://+:{0}/", httpPort) };

            ctr = new HttpListenerController(prefixes, "/", app_path);
            adapter = new Logbus2SoapAdapter(target);

            ctr.Start();
            //If object is not marshalled by reference, use a wrapper, otherwise don't complicate object graph
            object wrapper = (target is MarshalByRefObject) ? target : new LogBusTie(target);
            ctr.Domain.SetData("Logbus", wrapper);
        }

        private void StopService()
        {
            ctr.Stop();
            adapter.Stop(true);
            UninstallRuntime(physical_path);
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
            string dirname = Path.GetRandomFileName();
            string fullpath = Path.Combine(Path.GetTempPath(), dirname);
            if (File.Exists(fullpath)) File.Delete(fullpath);

            //Create temporary directory for ASP.NET
            Directory.CreateDirectory(fullpath);
            physical_path = fullpath;

            //Copy .asmx files from assembly to directory
            string resname_mgm = "It.Unina.Dis.Logbus.WebServices.LogbusManagement.asmx",
                resname_sub = "It.Unina.Dis.Logbus.WebServices.LogbusSubscription.asmx";
            //resname_asax = "It.Unina.Dis.Logbus.WebServices.Global.asax";
            string mgm_fname = "LogbusManagement.asmx", sub_fname = "LogbusSubscription.asmx";
            {
                string ws_declaration;
                using (StreamReader sr = new StreamReader(GetType().Assembly.GetManifestResourceStream(resname_mgm), Encoding.Default))
                    ws_declaration = string.Format(sr.ReadToEnd(), typeof(ChannelManagementService).AssemblyQualifiedName);

                using (StreamWriter bw = new StreamWriter(File.Create(Path.Combine(physical_path, mgm_fname)), Encoding.Default))
                    bw.Write(ws_declaration);
            }

            {
                string ws_declaration;
                using (StreamReader sr = new StreamReader(GetType().Assembly.GetManifestResourceStream(resname_sub), Encoding.Default))
                    ws_declaration = string.Format(sr.ReadToEnd(), typeof(ChannelSubscriptionService).AssemblyQualifiedName);

                using (StreamWriter bw = new StreamWriter(File.Create(Path.Combine(physical_path, sub_fname)), Encoding.Default))
                    bw.Write(ws_declaration);
            }


            if (!Assembly.GetExecutingAssembly().GlobalAssemblyCache)
            {
                //Deploy assembly too
                string codebase = Assembly.GetExecutingAssembly().Location;
                string bindir = Path.Combine(fullpath, "bin");
                Directory.CreateDirectory(bindir);
                File.Copy(codebase, Path.Combine(bindir, Path.GetFileName(codebase)));
            }

            //Return the path
            return fullpath;
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
