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

namespace It.Unina.Dis.Logbus.Api
{
    /// <summary>
    /// Provides logging services at a very high level
    /// </summary>
    /// <remarks>The interface reminds the log4net.ILog interface</remarks>
    public interface ILog
    {
        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">Text message to log</param>
        void Debug(string message);

        /// <summary>
        /// Logs an informational message
        /// </summary>
        /// <param name="message">Text message to log</param>
        void Info(string message);

        /// <summary>
        /// Logs a notice message
        /// </summary>
        /// <param name="message">Text message to log</param>
        void Notice(string message);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">Text message to log</param>
        void Warning(string message);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Text message to log</param>
        void Error(string message);

        /// <summary>
        /// Logs a critical message
        /// </summary>
        /// <param name="message">Text message to log</param>
        void Critical(string message);

        /// <summary>
        /// Logs an alert message
        /// </summary>
        /// <param name="message">Text message to log</param>
        void Alert(string message);

        /// <summary>
        /// Logs an emergency message
        /// </summary>
        /// <param name="message">Text message to log</param>
        void Emergency(string message);

    }
}
