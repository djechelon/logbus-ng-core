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

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Provides a default instance of Logbus
    /// </summary>
    public sealed class LogbusSingletonHelper
    {
        private static ILogBus _instance;

        /// <summary>
        /// Gets the default instance of Logbus service
        /// </summary>
        public static ILogBus Instance
        {
            get
            {
                if (_instance == null)
                    lock (typeof (LogbusSingletonHelper))
                        if (_instance == null)
                            _instance = new LogbusService();
                return _instance;
            }
            internal set { _instance = value; }
        }
    }
}