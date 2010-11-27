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
using System.ComponentModel;
using System.Diagnostics;
using System.Security;

namespace It.Unina.Dis.Logbus.InChannels
{
    /// <summary>
    /// Provides listening features for Windows Event Log
    /// </summary>
    /// <remarks></remarks>
    internal sealed class WindowsEventLogListener
        : IInboundChannel
    {
        private EventLog _log;

        #region Constructor/Destructor

        public WindowsEventLogListener()
        {
            Hostname = ".";
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32Windows:
                    {
                        break;
                    }
                default:
                    {
                        throw new PlatformNotSupportedException("This is supported only under Windows OS");
                    }
            }
            Configuration = new Dictionary<string, string>();
        }

        ~WindowsEventLogListener()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed) return;

            GC.SuppressFinalize(this);

            try
            {
                if (!Running)
                    Stop();
            }
            catch { }

            if (disposing)
            {
            }

            Disposed = true;
        }

        private bool Disposed { get; set; }

        #endregion

        private void LogEntryWritten(object sender, EntryWrittenEventArgs e)
        {
            if (MessageReceived != null) MessageReceived(this, new SyslogMessageEventArgs((SyslogMessage)e.Entry));
        }

        public string Hostname { get; set; }

        public string LogName { get; set; }

        #region IInboundChannel Membri di

#pragma warning disable 67
        public event EventHandler<ParseErrorEventArgs> ParseError;
#pragma warning restore 67

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
        public event EventHandler<CancelEventArgs> Starting;

        /// <remarks/>
        public event EventHandler<CancelEventArgs> Stopping;

        /// <remarks/>
        public event EventHandler Started;

        /// <remarks/>
        public event EventHandler Stopped;

        /// <remarks/>
        public event UnhandledExceptionEventHandler Error;

        public void Start()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            try
            {
                if (Running) throw new InvalidOperationException("Windows Event Log Listener is already started");


                if (LogName == null)
                    throw new LogbusException("Missing configuration parameter: logName");

                if (Starting != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Starting(this, e);
                    if (e.Cancel) return;
                }

                try
                {
                    new EventLogPermission(EventLogPermissionAccess.Administer, Hostname).Demand();
                }
                catch (SecurityException ex)
                {
                    throw new LogbusException(
                        "Unable to start Windows Event Log listener. Missing security clearance for the chosen machine",
                        ex);
                }

                try
                {
                    _log = new EventLog(LogName, Hostname);
                    _log.EntryWritten += LogEntryWritten;
                    Running = true;

                    if (Started != null) Started(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    throw new LogbusException("Unable to start Windows Event Log Listener. Unable to get log", ex);
                }
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        public void Stop()
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            try
            {
                if (!Running) throw new InvalidOperationException("Windows Event Log Listener is not running");

                if (Stopping != null)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    Stopping(this, e);
                    if (e.Cancel) return;
                }

                _log.EntryWritten -= LogEntryWritten;
                _log.Close();
                _log = null;
                Running = false;

                if (Stopped != null) Stopped(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new UnhandledExceptionEventArgs(ex, true));
                throw;
            }
        }

        public bool Running { get; private set; }
        #endregion

        #region IConfigurable Membri di

        public string GetConfigurationParameter(string key)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            switch (key)
            {
                case "host":
                    {
                        return Hostname;
                    }
                case "logName":
                    {
                        return LogName;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter is not supported");
                    }
            }
        }

        public void SetConfigurationParameter(string key, string value)
        {
            if (Disposed) throw new ObjectDisposedException(GetType().FullName);
            switch (key)
            {
                case "host":
                    {
                        Hostname = value ?? ".";
                        break;
                    }
                case "logName":
                    {
                        LogName = value;
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Configuration parameter is not supported");
                    }
            }
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
            set
            {
                if (Disposed) throw new ObjectDisposedException(GetType().FullName);
                foreach (KeyValuePair<string, string> kvp in value)
                {
                    SetConfigurationParameter(kvp.Key, kvp.Value);
                }
            }
        }

        #endregion
    }
}