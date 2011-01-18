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

using System;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace It.Unina.Dis.Logbus.Configuration
{
    /// <summary>
    /// Helper class that provides configuration objects
    /// </summary>
    public static class ConfigurationHelper
    {
        private static LogbusServerConfiguration _core;
        private static LogbusClientConfiguration _client;
        private static LogbusLoggerConfiguration _source;

        /// <summary>
        /// Obsolete server configuration accessor
        /// </summary>
        [Obsolete("CoreConfiguration is deprecated. Use ServerConfiguration instead.")]
        public static LogbusServerConfiguration CoreConfiguration
        {
            get { return ServerConfiguration; }
        }

        /// <summary>
        /// Configuration for Logbus server
        /// </summary>
        public static LogbusServerConfiguration ServerConfiguration
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (_core != null) return _core;
                try
                {
                    //Try to auto-configure. If fails, skip for now. Somebody MUST then provide proper configuration
                    _core = ConfigurationManager.GetSection("logbus-server") as LogbusServerConfiguration;
                    return _core;
                }
                catch
                {
                    return null;
                }
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { _core = value; }
        }

        /// <summary>
        /// Configuration for log sources
        /// </summary>
        public static LogbusLoggerConfiguration SourceConfiguration
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (_source != null) return _source;
                try
                {
                    //Try to auto-configure. If fails, skip for now. Somebody MUST then provide proper configuration
                    _source = ConfigurationManager.GetSection("logbus-logger") as LogbusLoggerConfiguration;
                    return _source;
                }
                catch
                {
                    return null;
                }
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { _source = value; }
        }

        /// <summary>
        /// Configuration for monitor clients
        /// </summary>
        public static LogbusClientConfiguration ClientConfiguration
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (_client != null) return _client;
                try
                {
                    //Try to auto-configure. If fails, skip for now. Somebody MUST then provide proper configuration
                    _client = ConfigurationManager.GetSection("logbus-client") as LogbusClientConfiguration;
                    return _client;
                }
                catch
                {
                    return null;
                }
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set { _client = value; }
        }
    }
}