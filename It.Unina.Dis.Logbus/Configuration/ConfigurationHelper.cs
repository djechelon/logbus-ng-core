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

using System.Configuration;
namespace It.Unina.Dis.Logbus.Configuration
{
    public sealed class ConfigurationHelper
    {
        private ConfigurationHelper() { }

        public static LogbusCoreConfiguration CoreConfiguration
        {
            get
            {
                try
                {
                    //Try to auto-configure. If fails, skip for now. Somebody MUST then provide proper configuration
                    return ConfigurationManager.GetSection("logbus-core") as LogbusCoreConfiguration;
                }
                catch { return null; }
            }
        }

        public static LogbusSourceConfiguration SourceConfiguration
        {
            get
            {
                try
                {
                    //Try to auto-configure. If fails, skip for now. Somebody MUST then provide proper configuration
                    return ConfigurationManager.GetSection("logbus-source") as LogbusSourceConfiguration;
                }
                catch { return null; }
            }
        }

        public static LogbusClientConfiguration ClientConfiguration
        {
            get
            {
                try
                {
                    //Try to auto-configure. If fails, skip for now. Somebody MUST then provide proper configuration
                    return ConfigurationManager.GetSection("logbus-client") as LogbusClientConfiguration;
                }
                catch { return null; }
            }
        }
    }
}
