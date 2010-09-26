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
using It.Unina.Dis.Logbus.Filters;
namespace It.Unina.Dis.Logbus.Entities
{
    /// <summary>
    /// Used by EntityPlugin to create entity-specific channels
    /// </summary>
    internal sealed class EntityFilter
        : IFilter
    {
        private readonly string _host, _process, _logger;
        private readonly bool _ffdaOnly;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of entity filter EntityFilter
        /// </summary>
        /// <param name="host">Host that matches entity</param>
        /// <param name="process">Process ID/AppName that matches entity</param>
        /// <param name="logger">Logger that matches entity</param>
        public EntityFilter(string host, string process, string logger)
            : this(host, process, logger, false)
        { }

        /// <summary>
        /// Initializes a new instance of entity filter EntityFilter
        /// </summary>
        /// <param name="host">Host that matches entity</param>
        /// <param name="process">Process ID/AppName that matches entity</param>
        /// <param name="logger">Logger that matches entity</param>
        /// <param name="ffdaOnly">Whether to allow only FFDA messages or not</param>
        public EntityFilter(string host, string process, string logger, bool ffdaOnly)
        {
            if (host == null) throw new ArgumentNullException("host");
            if (process == null) throw new ArgumentNullException("process");
            if (logger == null) throw new ArgumentNullException("logger");
            _host = host;
            _process = process;
            _logger = logger;
            _ffdaOnly = ffdaOnly;
        }
        #endregion

        #region IFilter Membri di

        public bool IsMatch(SyslogMessage message)
        {
            SyslogAttributes attrs = message.GetAdvancedAttributes();
            return (
                _host == (message.Host ?? "") &&
                _process == (message.ProcessID ?? message.ApplicationName ?? "") &&
                _logger == (attrs.LogName ?? "") &&
                (!_ffdaOnly || _ffdaOnly && message.MessageId == "FFDA" && message.Severity == SyslogSeverity.Info));
        }

        #endregion
    }
}
