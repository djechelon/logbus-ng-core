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

using System.Collections.Generic;
using System.Diagnostics;
using System;
namespace It.Unina.Dis.Logbus.InChannels
{
    /// <summary>
    /// Provides listening features for Windows Event Log
    /// </summary>
    /// <remarks></remarks>
    internal sealed class WindowsEventLogListener
        : IInboundChannel, ILogSource
    {

        private EventLog _log;

        #region Constructor/Destructor

        public WindowsEventLogListener()
        {
            switch (System.Environment.OSVersion.Platform)
            {
                case System.PlatformID.Win32NT:
                case System.PlatformID.Win32Windows:
                    { break; }
                default:
                    {
                        throw new System.PlatformNotSupportedException("This is supported only under Windows OS");
                    }
            }
            Configuration = new Dictionary<string, string>();

            //Trick to suppress compilation warnings. Don't edit (from the constructor, no listener is attached yet)
            if (ParseError != null) ParseError(null, null);
        }

        ~WindowsEventLogListener()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) return;
            try { Stop(); }
            catch { }

            if (disposing)
            {
            }

            Disposed = true;
        }

        private bool Disposed
        {
            get;
            set;
        }
        #endregion

        public bool Running { get; private set; }

        void log_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs((SyslogMessage)e.Entry));
        }

        public string Hostname
        {
            get;
            set;
        }

        public string LogName
        {
            get;
            set;
        }

        #region IInboundChannel Membri di

        public string Name
        {
            get;
            set;
        }

        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (Running) throw new InvalidOperationException("Windows Event Log Listener is already started");

            if (Hostname == null)
            {
                try
                {
                    Hostname = GetConfigurationParameter("host");
                }
                catch (KeyNotFoundException)
                {
                    Hostname = ".";
                }
            }
            if (LogName == null)
            {
                try
                {
                    LogName = GetConfigurationParameter("logName");
                }
                catch (KeyNotFoundException)
                {
                    throw new LogbusException("Missing configuration parameter: logName");
                }
            }

            try
            {
                new EventLogPermission(EventLogPermissionAccess.Administer, Hostname).Demand();
            }
            catch (System.Security.SecurityException ex)
            {
                throw new LogbusException("Unable to start channel. Missing security clearance for the chosen machine", ex);
            }

            _log = new EventLog(LogName, Hostname);
            _log.EntryWritten += new EntryWrittenEventHandler(log_EntryWritten);
        }

        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            if (!Running) throw new InvalidOperationException("Windows Event Log Listener is not running");

            _log.EntryWritten -= log_EntryWritten;
            _log.Close();
            _log = null;
        }

        public event EventHandler<ParseErrorEventArgs> ParseError;

        #endregion

        #region ILogSource Membri di

        public event EventHandler<SyslogMessageEventArgs> MessageReceived;

        #endregion

        #region IDisposable Membri di

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region IRunnable Membri di

        /// <remarks/>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Starting;

        /// <remarks/>
        public event EventHandler<System.ComponentModel.CancelEventArgs> Stopping;

        /// <remarks/>
        public event EventHandler Started;

        /// <remarks/>
        public event EventHandler Stopped;

        /// <remarks/>
        public event UnhandledExceptionEventHandler Error;

        #endregion

        #region IConfigurable Membri di

        public string GetConfigurationParameter(string key)
        {
            throw new NotImplementedException();
        }

        public void SetConfigurationParameter(string key, string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// List of supported configuration parameters:
        /// <list>
        /// <item><c>host</c>(optional): hostname of which to collect logs (local system MUST be authorized via Windows Event Log facility).
        /// If not set, using "."</item>
        /// <item><c>logName</c>: name of Event Log to listen for messages</item>
        /// </list>
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Configuration
        {
            set { throw new NotImplementedException(); }
        }

        #endregion
    }
}
