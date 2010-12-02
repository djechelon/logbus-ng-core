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
using System.Collections.Generic;
using It.Unina.Dis.Logbus.Design;
using It.Unina.Dis.Logbus.Filters;

namespace It.Unina.Dis.Logbus.Entities
{
    /// <summary>
    /// Filters messages from a specific logging entity
    /// </summary>
    [CustomFilter("logbus-entity", Description = "Only messages from a certain entity match this filter")]
    public sealed class EntityFilter
        : ICustomFilter
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of entity filter EntityFilter
        /// </summary>
        public EntityFilter()
        {
            FfdaOnly = false;
        }

        /// <summary>
        /// Initializes a new instance of entity filter EntityFilter
        /// </summary>
        /// <param name="host">Host that matches entity</param>
        /// <param name="process">Process ID/AppName that matches entity</param>
        /// <param name="logger">Logger that matches entity</param>
        public EntityFilter(string host, string process, string logger)
            : this(host, process, logger, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of entity filter EntityFilter
        /// </summary>
        /// <param name="host">Host that matches entity</param>
        /// <param name="process">Process ID/AppName that matches entity</param>
        /// <param name="logger">Logger that matches entity</param>
        /// <param name="module">Module that matches the entity</param>
        /// <param name="class">Class that matches the entity</param>
        /// <param name="method">Method that matches the entity</param>
        /// <param name="ffdaOnly">Whether to allow only FFDA messages or not</param>
        public EntityFilter(string host, string process, string logger, string module, string @class, string method, bool ffdaOnly)
            : this(host, process, logger, ffdaOnly)
        {
            Module = module;
            Class = @class;
            Method = method;
        }

        /// <summary>
        /// Initializes a new instance of entity filter EntityFilter
        /// </summary>
        /// <param name="host">Host that matches entity</param>
        /// <param name="process">Process ID/AppName that matches entity</param>
        /// <param name="logger">Logger that matches entity</param>
        /// <param name="module">Module that matches the entity</param>
        /// <param name="class">Class that matches the entity</param>
        /// <param name="method">Method that matches the entity</param>
        public EntityFilter(string host, string process, string logger, string module, string @class, string method)
            : this(host, process, logger, module, @class, @method, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of entity filter EntityFilter
        /// </summary>
        /// <param name="host">Host that matches entity</param>
        /// <param name="process">Process ID/AppName that matches entity</param>
        /// <param name="logger">Logger that matches entity</param>
        /// <param name="ffdaOnly">Whether to allow only FFDA messages or not</param>
        public EntityFilter(string host, string process, string logger, bool ffdaOnly)
        {
            Host = host;
            Process = process;
            Logger = logger;
            FfdaOnly = ffdaOnly;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Host that generated the message
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Process that generated the message
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// Logger that generated the message
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// Module that generated the message
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// Class that generated the message
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Method that generated the message
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Whether to accept or not only FFDA messages
        /// </summary>
        public bool FfdaOnly { get; set; }

        #endregion

        #region IFilter Membri di

        public bool IsMatch(SyslogMessage message)
        {
            SyslogAttributes attrs = message.GetAdvancedAttributes();

            if (Host != null)
                if (Host != (message.Host ?? string.Empty)) return false;

            if (Process != null)
                if (Process != (message.ProcessID ?? string.Empty)) return false;

            if (Logger != null)
                if (Logger != (attrs.LogName ?? string.Empty)) return false;

            if (Module != null)
                if (Module != (attrs.ModuleName ?? string.Empty)) return false;

            if (Class != null)
                if (Class != (attrs.ClassName ?? string.Empty)) return false;

            if (Method != null)
                if (Method != (attrs.MethodName ?? string.Empty)) return false;

            if (FfdaOnly)
                return ((message.MessageId == "FFDA" && message.Severity == SyslogSeverity.Info)
                        || (message.MessageId == "HEARTBEAT" && message.Severity == SyslogSeverity.Debug));

            return true;
        }

        #endregion

        /// <summary>
        /// Casts to FilterBase to ease use from clients
        /// </summary>
        /// <param name="filter">Filter to cast</param>
        /// <returns>An XML proxy to the filter</returns>
        public static implicit operator FilterBase(EntityFilter filter)
        {
            CustomFilter ret = new CustomFilter
                                   {
                                       name = "logbus-entity"
                                   };


            List<FilterParameter> @params = new List<FilterParameter>(4);
            if (filter.Host != null)
                @params.Add(new FilterParameter { name = "host", value = filter.Host });
            if (filter.Process != null)
                @params.Add(new FilterParameter { name = "process", value = filter.Process });
            if (filter.Logger != null)
                @params.Add(new FilterParameter { name = "logger", value = filter.Logger });
            if (filter.Module != null)
                @params.Add(new FilterParameter { name = "module", value = filter.Module });
            if (filter.Class != null)
                @params.Add(new FilterParameter { name = "class", value = filter.Class });
            if (filter.Method != null)
                @params.Add(new FilterParameter { name = "method", value = filter.Method });

            @params.Add(new FilterParameter { name = "ffdaOnly", value = filter.FfdaOnly });

            ret.parameter = @params.ToArray();

            return ret;
        }

        #region ICustomFilter Membri di

        public IEnumerable<FilterParameter> Configuration
        {
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                foreach (FilterParameter parameter in value)
                {
                    switch (parameter.name)
                    {
                        case "host":
                            {
                                Host = (parameter.value as string) ?? string.Empty;
                                break;
                            }
                        case "process":
                            {
                                Process = (parameter.value as string) ?? string.Empty;
                                break;
                            }
                        case "logger":
                            {
                                Logger = (parameter.value as string) ?? string.Empty;
                                break;
                            }
                        case "module":
                            {
                                Module = (parameter.value as string) ?? string.Empty;
                                break;
                            }
                        case "class":
                            {
                                Class = (parameter.value as string) ?? string.Empty;
                                break;
                            }
                        case "method":
                            {
                                Method = (parameter.value as string) ?? string.Empty;
                                break;
                            }
                        case "ffdaOnly":
                            {
                                FfdaOnly = (bool)parameter.value;
                                break;
                            }
                        default:
                            {
                                throw new NotSupportedException(
                                    string.Format("Configuration parameter not supported: {0}", parameter.name));
                            }
                    }
                }
            }
        }

        #endregion
    }
}