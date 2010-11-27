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
using System.Configuration.Install;
using System.ServiceProcess;

namespace It.Unina.Dis.Logbus.Services
{
    /// <summary>
    /// Default installer for Logbus-ng service
    /// </summary>
    [RunInstaller(true)]
    public class LogbusDaemonInstaller : Installer
    {
        private readonly ServiceProcessInstaller _serviceProcessInstaller;
        private readonly ServiceInstaller _serviceInstaller;


        /// <summary>
        /// Initializes a new instance of LogbusDaemonInstaller
        /// </summary>
        public LogbusDaemonInstaller()
        {
            _serviceProcessInstaller = new ServiceProcessInstaller
                                           {
                                               Account = ServiceAccount.LocalSystem,
                                               Username = null,
                                               Password = null,
                                           };

            _serviceInstaller = new ServiceInstaller
                                    {
                                        ServiceName = "Logbus-ng",
                                        StartType = ServiceStartMode.Automatic
                                    };

            Installers.AddRange(new Installer[]
                                    {
                                        _serviceProcessInstaller,
                                        _serviceInstaller
                                    });
        }
    }
}
