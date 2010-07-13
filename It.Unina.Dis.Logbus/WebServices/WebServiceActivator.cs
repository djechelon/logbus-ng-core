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
            HostingEnvironment.RegisterObject(adapter);
            ctr.Start();

            /*
            mgm_service = new ChannelManagementService(new Logbus2SoapAdapter(target));
            sub_service = new ChannelSubscriptionService(new Logbus2SoapAdapter(target));
            */

        }

        private void StopService()
        {
            ctr.Stop();
            adapter.Stop(true);
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
        /// 
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

        private string InstallRuntime()
        {
            string dirname = Path.GetRandomFileName();
            string fullpath = Path.Combine(Path.GetTempPath(), dirname);
            if (File.Exists(fullpath)) File.Delete(fullpath);

            //Create temporary directory for ASP.NET
            Directory.CreateDirectory(fullpath);

            //Copy .asmx files from assembly to directory
            string resname_mgm = "It.Unina.Dis.Logbus.WebServices.LogbusManagement.asmx", resname_sub = "It.Unina.Dis.Logbus.WebServices.LogbusSubscription.asmx";
            string mgm_fname = "LogbusManagement.asmx", sub_fname = "LogbusSubscription.asmx";
            using (BinaryReader br = new BinaryReader(GetType().Assembly.GetManifestResourceStream(resname_mgm)))
            {
                using (BinaryWriter bw = new BinaryWriter(File.Create(Path.Combine(fullpath, mgm_fname))))
                {
                    int readbytes, position = 0;
                    do
                    {
                        byte[] buffer = new byte[1024];
                        readbytes = br.Read(buffer, position, 1024);
                        bw.Write(buffer, 0, readbytes);
                    } while (readbytes > 0);
                }
            }

            using (BinaryReader br = new BinaryReader(GetType().Assembly.GetManifestResourceStream(resname_sub)))
            {
                using (BinaryWriter bw = new BinaryWriter(File.Create(Path.Combine(fullpath, sub_fname))))
                {
                    int readbytes, position = 0;
                    do
                    {
                        byte[] buffer = new byte[1024];
                        readbytes = br.Read(buffer, position, 1024);
                        bw.Write(buffer, 0, readbytes);
                    } while (readbytes > 0);
                }
            }

            //Return the path
            return fullpath;
        }

        private void UninstallRuntime(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            foreach (FileInfo fi in info.GetFiles())
                fi.Delete();
            info.Delete();
        }
    }
}
