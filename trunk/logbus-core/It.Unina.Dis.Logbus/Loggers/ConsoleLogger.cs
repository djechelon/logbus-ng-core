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
    /// Outputs log messages to the console according to a standard output format:
    /// {timestamp} - {severity}: {message}
    /// Timestamp is formatted according to the yyyy-MM-dd-hh-mm-ss format
    /// </summary>
    internal sealed class ConsoleLogger
        : ILogger
    {
        #region ILogCollector Membri di

        void ILogCollector.SubmitMessage(SyslogMessage message)
        {
            string tstamp = (message.Timestamp.HasValue) ? message.Timestamp.Value.ToString("yyyy-MM-dd-HH:mm:ss") : "-";
            System.Console.WriteLine("{0} - {1}: {2}",
                tstamp,
                System.Enum.GetName(typeof(SyslogSeverity), message.Severity),
                message.Text
             );
        }

        #endregion

        #region IConfigurable Membri di

        string IConfigurable.GetConfigurationParameter(string key)
        {
            throw new System.NotSupportedException("Configuration is not supported");
        }

        void IConfigurable.SetConfigurationParameter(string key, string value)
        {
            throw new System.NotSupportedException("Configuration is not supported");
        }

        System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, string>> IConfigurable.Configuration
        {
            set { throw new System.NotSupportedException("Configuration is not supported"); }
        }

        #endregion
    }
}
