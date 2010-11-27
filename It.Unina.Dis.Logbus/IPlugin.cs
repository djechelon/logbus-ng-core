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

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Represents a Logbus plugin
    /// </summary>
    public interface IPlugin
        : ILogSupport, IDisposable
    {
        /// <summary>
        /// Tells a plugin to register on Logbus
        /// </summary>
        /// <param name="logbus"></param>
        void Register(ILogBus logbus);

        /// <summary>
        /// Tells the plugin to unregister
        /// </summary>
        void Unregister();

        /// <summary>
        /// Unique plugin name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the types and API names for WSDL interface
        /// </summary>
        WsdlSkeletonDefinition[] GetWsdlSkeletons();

        /// <summary>
        /// Gets the root object that needs to be copied into ASP.NET AppDomain for Web Services API
        /// </summary>
        /// <remarks>Returns null if no WS API is supported by this plugin</remarks>
        MarshalByRefObject GetPluginRoot();
    }

    /// <summary>
    /// Pairs the type of the WSDL skeleton with the desired URL file name
    /// </summary>
    public struct WsdlSkeletonDefinition
    {
        /// <summary>
        /// File name of the URL
        /// </summary>
        /// <example>If set to "Acme", final URL will be http://logbus-host/Acme.asmx</example>
        /// <remarks>It must not end with .asmx as it will be added by the runtime</remarks>
        public string UrlFileName;

        /// <summary>
        /// Type for skeleton class
        /// </summary>
        public Type SkeletonType;
    }
}