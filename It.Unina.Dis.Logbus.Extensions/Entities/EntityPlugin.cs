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
using System.Data;
using It.Unina.Dis.Logbus.Utils;
using System.Threading;
using It.Unina.Dis.Logbus.Configuration;
using System.Text.RegularExpressions;
namespace It.Unina.Dis.Logbus.Entities
{
    /// <summary>
    /// Plugin that manages Logbus-ng entities, where the entity is user-defined by configuration
    /// </summary>
    public sealed class EntityPlugin
        : MarshalByRefObject, IPlugin, IEntityManagement
    {
        /// <summary>
        /// Avoids ambigousness
        /// </summary>
        public const string PLUGIN_ID = "Logbus.EntityManager";

        private ILogBus _logbus;
        private readonly BlockingFifoQueue<SyslogMessage> _messageQueue;
        private readonly Thread _workerThread;

        private readonly DataColumn _colHost, _colProc, _colLogger, _colFfda, _colLastAction, _colLastHeartbeat;
        private readonly UniqueConstraint _primaryKey;
        private readonly DataTable _entityTable;

        #region Constructor/Destructor

        public EntityPlugin()
        {
            _messageQueue = new BlockingFifoQueue<SyslogMessage>();

            _workerThread = new Thread(WorkerLoop) { IsBackground = true, Name = "EntityPlugin.WorkerLoop" };
            _workerThread.Start();

            _entityTable = new DataTable("Entities");

            _colHost = new DataColumn("Host", typeof(string))
            {
                AllowDBNull = true,
                ReadOnly = true,
                Unique = false
            };
            _colProc = new DataColumn("Process", typeof(string))
            {
                AllowDBNull = true,
                ReadOnly = true,
                Unique = false
            };
            _colLogger = new DataColumn("Logger", typeof(string))
            {
                AllowDBNull = true,
                ReadOnly = true,
                Unique = false
            };
            _colFfda = new DataColumn("FFDA", typeof(bool))
            {
                AllowDBNull = false,
                DefaultValue = false,
                ReadOnly = false,
                Unique = false
            };
            _colLastAction = new DataColumn("LastAction", typeof(DateTime))
            {
                AllowDBNull = false,
                ReadOnly = false,
                Unique = false,
                DateTimeMode = DataSetDateTime.Utc,
            };
            _colLastHeartbeat = new DataColumn("LstHeartbeat", typeof(DateTime))
            {
                AllowDBNull = true,
                ReadOnly = false,
                Unique = false,
                DateTimeMode = DataSetDateTime.Utc,
            };

            _entityTable.Columns.Add(_colHost);
            _entityTable.Columns.Add(_colProc);
            _entityTable.Columns.Add(_colLogger);
            _entityTable.Columns.Add(_colFfda);
            _entityTable.Columns.Add(_colLastAction);
            _entityTable.Columns.Add(_colLastHeartbeat);

            _primaryKey = new UniqueConstraint(new DataColumn[] { _colHost, _colProc, _colLogger }, true)
            {
                ConstraintName = "Primary"
            };
            _entityTable.Constraints.Add(_primaryKey);
        }

        ~EntityPlugin()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            _workerThread.Interrupt();

            if (disposing)
            {
                _messageQueue.Dispose();
                _entityTable.Dispose();
            }

            _disposed = true;
        }


        private volatile bool _disposed;
        #endregion

        #region IPlugin Membri di

        /// <summary>
        /// Implements IPlugin.Register
        /// </summary>
        public void Register(ILogBus logbus)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            this._logbus = logbus;
            logbus.MessageReceived += new EventHandler<SyslogMessageEventArgs>(logbus_MessageReceived);
        }

        /// <summary>
        /// Implements IPlugin.Unregister
        /// </summary>
        public void Unregister()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            _logbus.MessageReceived -= logbus_MessageReceived;
        }

        /// <summary>
        /// Implements ILogSupport.Log
        /// </summary>
        public It.Unina.Dis.Logbus.Loggers.ILog Log
        {
            private get;
            set;
        }

        /// <summary>
        /// Implements IPlugin.Name
        /// </summary>
        public string Name
        {
            get { return PLUGIN_ID; }
        }

        /// <summary>
        /// Implements IPlugin.GetWsdlSkeletons
        /// </summary>
        public WsdlSkeletonDefinition[] GetWsdlSkeletons()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            WsdlSkeletonDefinition ret = new WsdlSkeletonDefinition()
                                             {
                                                 SkeletonType = typeof(EntityManagementSkeleton),
                                                 UrlFileName = "EntityManagement"
                                             };
            return new WsdlSkeletonDefinition[] { ret };
        }

        /// <summary>
        /// Implements IPlugin.GetPluginRoot
        /// </summary>
        public System.MarshalByRefObject GetPluginRoot()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            return this;
        }

        #endregion

        #region IDisposable Membri di

        /// <summary>
        /// Implements IDisposable.Dispose
        /// </summary>
        public void Dispose()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            throw new System.NotImplementedException();
        }

        #endregion

        private void logbus_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            SyslogMessage message = e.Message;
            if (message.Facility == SyslogFacility.Internally) return; //Skip all syslog-internal messages
            _messageQueue.Enqueue(message);
        }

        private void WorkerLoop()
        {
            try
            {
                while (true)
                {
                    SyslogMessage message = _messageQueue.Dequeue();

                    //if (message.Facility == SyslogFacility.Internally) continue; //Redundant

                    SyslogAttributes attrs = message.GetAdvancedAttributes();
                    DateTime? lastHb = null;

                    bool ffda = message.MessageId == "FFDA";
                    if (message.MessageId == "HEARTBEAT" && message.Severity == SyslogSeverity.Debug)
                        lastHb = message.Timestamp;
                    try
                    {
                        try
                        {
                            _entityTable.Rows.Add(
                                message.Host ?? "",
                                message.ProcessID ?? message.ApplicationName ?? "",
                                attrs.LogName ?? "",
                                ffda,
                                message.Timestamp,
                                lastHb
                                );

                            Log.Debug("Acquired new entity: ({0}|{1}|{2}), {3}FFDA-enabled",
                                message.Host ?? "NULL",
                                message.ProcessID ?? message.ApplicationName ?? "NULL",
                                attrs.LogName ?? "NULL",
                                ffda ? "" : "not ");
                        }
                        catch (ConstraintException)
                        {
                            //We suppose we are trying to insert a duplicate primary key, then now we switch to update
                            object[] keys = new object[]{
                                message.Host ?? "",
                                message.ProcessID ?? message.ApplicationName ?? "",
                                attrs.LogName ?? "",
                            };
                            DataRow existingRow = _entityTable.Rows.Find(keys);
                            bool oldFfda = (bool)existingRow[_colFfda];

                            existingRow.BeginEdit();
                            existingRow[_colFfda] = ffda | oldFfda;

                            if (lastHb != null)
                                existingRow[_colLastHeartbeat] = message.Timestamp;
                            else
                                existingRow[_colLastAction] = message.Timestamp;

                            existingRow.EndEdit();

                            if (ffda && !oldFfda)
                                Log.Debug("Entity ({0}|{1}|{2}) is now FFDA-enabled",
                                    message.Host ?? "NULL",
                                    message.ProcessID ?? message.ApplicationName ?? "NULL",
                                    attrs.LogName ?? "NULL");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Unable to add an entity row into the data table");
                        Log.Debug(string.Format("Error details: {0}", ex.Message));
                    }
                }
            }
            catch (ThreadInterruptedException) { }
        }

        #region IEntityManagement Membri di

        /// <summary>
        /// Implements IEntityManagement.GetLoggingEntities
        /// </summary>
        public LoggingEntity[] GetLoggingEntities()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            DataRow[] rows = _entityTable.Select();
            LoggingEntity[] ret = new LoggingEntity[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                ret[i] = new LoggingEntity()
                             {
                                 host = (string)rows[i][_colHost],
                                 process = (string)rows[i][_colProc],
                                 logger = (string)rows[i][_colLogger],
                                 ffda = (bool)rows[i][_colFfda],
                                 lastAction = (DateTime)rows[i][_colLastAction],
                                 lastHeartbeat = (rows[i][_colLastHeartbeat] is DBNull) ? DateTime.MinValue : (DateTime)rows[i][_colLastHeartbeat]
                             };
            }

            return ret;
        }

        /// <summary>
        /// Implements IEntityManagement.FindLoggingEntities
        /// </summary>
        public LoggingEntity[] FindLoggingEntities(TemplateQuery query)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            throw new NotImplementedException();
        }

        #endregion

        #region Proxy factory
        /// <summary>
        /// Returns the default Entity Manager proxy for the given endpoint URL
        /// </summary>
        /// <param name="endpointUrl">URL endpoint for Entity Manager</param>
        /// <returns></returns>
        public static IEntityManagement GetProxy(string endpointUrl)
        {
            return new EntityManagement() { Url = endpointUrl };
        }

        /// <summary>
        /// Returns the default Entity Manager proxy for the default endpoint URL according to
        /// Logbus client configuration
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Logbus-ng client is not configured</exception>
        public static IEntityManagement GetProxy()
        {
            try
            {
                string endpointUrl = ConfigurationHelper.ClientConfiguration.endpoint.subscriptionUrl;
                if (string.IsNullOrEmpty(endpointUrl))
                    throw new InvalidOperationException(
                        "Logbus-ng client is not configured. Cannot guess default endpoint URL");

                return
                    GetProxy(Regex.Replace(endpointUrl,
                                           "LogbusSubscription(?!.*LogbusSubscription)", "EntityManagement"));
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Logbus-ng client is not configured. Cannot guess default endpoint URL", ex);
            }
        }
        #endregion
    }
}
