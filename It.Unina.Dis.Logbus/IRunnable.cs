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
using System;
namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Interface for facilities that need to be started before use.
    /// Controls start and stop, and these operations are cancellable
    /// </summary>
    public interface IRunnable
    {
        #region Events
        /// <summary>
        /// Facility is starting
        /// </summary>
        event EventHandler<CancelEventArgs> Starting;

        /// <summary>
        /// Facility is stopping
        /// </summary>
        event EventHandler<CancelEventArgs> Stopping;

        /// <summary>
        /// Facility has started
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Facility has stopped
        /// </summary>
        event EventHandler Stopped;

        /// <summary>
        /// An error occurred in the core
        /// </summary>
        event UnhandledExceptionEventHandler Error;
        #endregion

        /// <summary>
        /// Starts the facility
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the facility
        /// </summary>
        void Stop();

    }
}
