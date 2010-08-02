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

using System.ServiceProcess;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Encapsulates Logbus-ng service within a system daemon (Windows Service)
    /// </summary>
    public class LogbusDaemon
        :ServiceBase
    {
        private ILogBus instance;

        /// <summary>
        /// Instance of Logbus-ng core
        /// </summary>
        public ILogBus LogbusInstance
        {
            get
            {
                if (instance == null) instance = LogbusSingletonHelper.Instance;
                return instance;
            }
            set { instance = value; }
        }

        /// <remarks/>
        protected override void Dispose(bool disposing)
        {
            if (disposing) LogbusInstance.Dispose();

            base.Dispose(disposing);
        }

        /// <remarks/>
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            LogbusInstance.Start();
        }

        /// <remarks/>
        protected override void OnStop()
        {
            base.OnStop();

            LogbusInstance.Stop();
        }
    }
}
