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
    /// Performs start and stop in an asynchronous manner
    /// </summary>
    public interface IAsyncRunnable
        : IRunnable
    {
        /// <summary>
        /// Begins an asynchronous start operation
        /// </summary>
        /// <returns>Token object required by EndStart</returns>
        IAsyncResult BeginStart();

        /// <summary>
        /// Completes the asynchronous Start
        /// </summary>
        /// <param name="result">Token returned by BeginStart</param>
        /// <remarks>This method blocks the calling thread until the start operation has completed</remarks>
        void EndStart(IAsyncResult result);

        /// <summary>
        /// Begins an asynchronous stop operation
        /// </summary>
        /// <returns>Token object required by EndStop</returns>
        IAsyncResult BeginStop();

        /// <summary>
        /// Completes the asynchronous Stop
        /// </summary>
        /// <param name="result">Token returned by BeginStop</param>
        /// <remarks>This method blocks the calling thread until the stop operation has completed</remarks>
        void EndStop(IAsyncResult result);
    }
}