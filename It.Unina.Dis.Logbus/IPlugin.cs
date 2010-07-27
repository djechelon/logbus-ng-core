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
using It.Unina.Dis.Logbus.Loggers;
namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Represents a Logbus plugin
    /// </summary>
    public interface IPlugin
        : IDisposable
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
        /// Sets a log to the plugin for internal logging
        /// </summary>
        ILog Log { set; }

        /// <summary>
        /// Unique plugin name
        /// </summary>
        string Name { get; }

        WsdlSkeletonDefinition[] GetWsdlSkeletons();

        MarshalByRefObject GetPluginRoot();
    }

    public struct WsdlSkeletonDefinition
    {
        string Url;
        Type skeleton_type;
    }
}
