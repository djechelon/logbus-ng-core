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

namespace It.Unina.Dis.Logbus.Loggers
{
    /// <summary>
    /// Provides logging services at a very high level
    /// </summary>
    /// <remarks>The interface reminds the log4net.ILog interface</remarks>
    public interface ILog
    {

        /// <summary>
        /// Symbolic name of log
        /// </summary>
        string LogName { get; set; }

        /// <summary>
        /// Gets or sets the ultimate destination of log messages
        /// </summary>
        ILogCollector Collector { get; set; }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">Text message to log</param>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Debug(string message);

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="format">Format of log message</param>
        /// <param name="args">Arguments for formatting</param>
        /// <exception cref="System.ArgumentNullException">format or args are null</exception>
        /// <exception cref="System.FormatException">Format is invalid</exception>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Debug(string format, params  object[] args);

        /// <summary>
        /// Logs an informational message
        /// </summary>
        /// <param name="message">Text message to log</param>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Info(string message);

        /// <summary>
        /// Logs an informational message
        /// </summary>
        /// <param name="format">Format of log message</param>
        /// <param name="args">Arguments for formatting</param>
        /// <exception cref="System.ArgumentNullException">format or args are null</exception>
        /// <exception cref="System.FormatException">Format is invalid</exception>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Info(string format, params  object[] args);

        /// <summary>
        /// Logs a notice message
        /// </summary>
        /// <param name="message">Text message to log</param>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Notice(string message);

        /// <summary>
        /// Logs a notice message
        /// </summary>
        /// <param name="format">Format of log message</param>
        /// <param name="args">Arguments for formatting</param>
        /// <exception cref="System.ArgumentNullException">format or args are null</exception>
        /// <exception cref="System.FormatException">Format is invalid</exception>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Notice(string format, params  object[] args);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="message">Text message to log</param>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Warning(string message);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="format">Format of log message</param>
        /// <param name="args">Arguments for formatting</param>
        /// <exception cref="System.ArgumentNullException">format or args are null</exception>
        /// <exception cref="System.FormatException">Format is invalid</exception>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Warning(string format, params  object[] args);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="message">Text message to log</param>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Error(string message);

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="format">Format of log message</param>
        /// <param name="args">Arguments for formatting</param>
        /// <exception cref="System.ArgumentNullException">format or args are null</exception>
        /// <exception cref="System.FormatException">Format is invalid</exception>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Error(string format, params  object[] args);

        /// <summary>
        /// Logs a critical message
        /// </summary>
        /// <param name="message">Text message to log</param>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Critical(string message);

        /// <summary>
        /// Logs a critical message
        /// </summary>
        /// <param name="format">Format of log message</param>
        /// <param name="args">Arguments for formatting</param>
        /// <exception cref="System.ArgumentNullException">format or args are null</exception>
        /// <exception cref="System.FormatException">Format is invalid</exception>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Critical(string format, params  object[] args);

        /// <summary>
        /// Logs an alert message
        /// </summary>
        /// <param name="message">Text message to log</param>
        void Alert(string message);

        /// <summary>
        /// Logs an alert message
        /// </summary>
        /// <param name="format">Format of log message</param>
        /// <param name="args">Arguments for formatting</param>
        /// <exception cref="System.ArgumentNullException">format or args are null</exception>
        /// <exception cref="System.FormatException">Format is invalid</exception>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Alert(string format, params  object[] args);

        /// <summary>
        /// Logs an emergency message
        /// </summary>
        /// <param name="message">Text message to log</param>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Emergency(string message);

        /// <summary>
        /// Logs an eemrgency message
        /// </summary>
        /// <param name="format">Format of log message</param>
        /// <param name="args">Arguments for formatting</param>
        /// <exception cref="System.ArgumentNullException">format or args are null</exception>
        /// <exception cref="System.FormatException">Format is invalid</exception>
        /// <exception cref="It.Unina.Dis.Logbus.LogbusException">Error occurred during logging</exception>
        void Emergency(string format, params  object[] args);
    }
}
