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

using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using It.Unina.Dis.Logbus.WebServices;

namespace It.Unina.Dis.Logbus.Services
{
    /// <summary>
    /// Encapsulates Logbus-ng service within a system daemon (Windows Service)
    /// </summary>
    public sealed class LogbusDaemon
        : ServiceBase
    {
        private ILogBus _instance;

        /// <summary>
        /// Initializes a new instance of LogbusDaemon
        /// </summary>
        public LogbusDaemon()
        {
            HttpPort = 8065; //Default

            ServiceName = "Logbus-ng";

            CanPauseAndContinue = true;
            CanStop = true;

            //Just stop
            CanShutdown = false;
            //We are supposed to run on servers, not laptops!
            //(but servers may still have UPSs)
            CanHandlePowerEvent = false;
            //No, we run as SYSTEM
            CanHandleSessionChangeEvent = false;
        }

        /// <summary>
        /// Gets or sets whether this service must spawn or not the HTTP server for SOAP
        /// interface of Logbus-ng
        /// </summary>
        public bool WebApplicationEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HTTP port used by the web application
        /// </summary>
        /// <remarks>In order for this to be effective, you must also set
        /// <see cref="WebApplicationEnabled"/> to <c>true</c></remarks>
        public int HttpPort
        {
            get;
            set;
        }

        /// <summary>
        /// Instance of Logbus-ng core
        /// </summary>
        public ILogBus LogbusInstance
        {
            get { return _instance ?? (_instance = LogbusSingletonHelper.Instance); }
            set { _instance = value; }
        }

        /// <remarks/>
        protected override void Dispose(bool disposing)
        {
            if (WebApplicationEnabled)
                try
                {
                    WebServiceActivator.Stop();
                }
                catch { }

            if (disposing) LogbusInstance.Dispose();

            base.Dispose(disposing);
        }

        /// <remarks/>
        protected override bool CanRaiseEvents
        {
            get { return false; }
        }

        /// <remarks/>
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            Process pc = Process.GetCurrentProcess();
            Directory.SetCurrentDirectory
                (pc.MainModule.FileName.Substring(0, pc.MainModule.FileName.LastIndexOf(Path.PathSeparator)));

            if (!LogbusInstance.Running)
            {
                LogbusInstance.Start();
                if (WebApplicationEnabled) WebServiceActivator.Start(LogbusInstance, HttpPort);
            }
        }

        /// <remarks/>
        protected override void OnStop()
        {
            if (LogbusInstance.Running)
            {
                if (WebApplicationEnabled) WebServiceActivator.Stop();

                LogbusInstance.Stop();
            }

            base.OnStop();
        }

        /// <remarks/>
        protected override void OnPause()
        {
            base.OnPause();

            if (LogbusInstance.Running)
                foreach (IInboundChannel channel in LogbusInstance.InboundChannels)
                {
                    if (channel.Running) channel.Stop();
                }
        }

        /// <remarks/>
        protected override void OnContinue()
        {
            base.OnContinue();

            if (LogbusInstance.Running)
                foreach (IInboundChannel channel in LogbusInstance.InboundChannels)
                {
                    if (!channel.Running) channel.Start();
                }
        }
    }
}