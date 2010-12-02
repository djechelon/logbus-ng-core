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
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Services.Protocols;
using It.Unina.Dis.Logbus.Configuration;
using It.Unina.Dis.Logbus.Filters;
using It.Unina.Dis.Logbus.Loggers;
using It.Unina.Dis.Logbus.Utils;
using It.Unina.Dis.Logbus.Entities.Configuration;
using System.Text;

namespace It.Unina.Dis.Logbus.Entities
{
    /// <summary>
    /// Plugin that manages Logbus-ng entities, where the entity is user-defined by configuration
    /// </summary>
    public sealed class EntityPlugin
        : MarshalByRefObject, IPlugin, IEntityManagement
    {
        private readonly static EntityPluginConfiguration DefaultConfiguration;

        /// <summary>
        /// Avoids ambigousness
        /// </summary>
        public const string PLUGIN_ID = "Logbus.EntityManager";

        private const int QUEUE_SIZE = 32768;

        private ILogBus _logbus;
        private readonly IFifoQueue<SyslogMessage> _messageQueue;
        private readonly Thread _workerThread;

        private readonly DataColumn _colHost,
                                    _colProc,
                                    _colLogger,
                                    _colModule,
                                    _colClass,
                                    _colMethod,
                                    _colAppName,
                                    _colFfda,
                                    _colLastAction,
                                    _colLastHeartbeat,
                                    _colChannelId,
                                    _colFfdaChannelId;

        private readonly bool _isHostKey, _isProcessKey, _isLoggerKey, _isModuleKey, _isClassKey, _isMethodKey;

        private readonly UniqueConstraint _primaryKey;
        private readonly DataTable _entityTable;
        private EntityPluginConfiguration _config;
        private Timer _tmrStatistics;

        #region Constructor/Destructor

        static EntityPlugin()
        {
            DefaultConfiguration = new EntityPluginConfiguration
                                       {

                                           entitykey = new[]
                                                         {
                                                                 FieldType.host,
                                                                 FieldType.process,
                                                                 FieldType.logger
                                                         }
                                       };
        }

        /// <summary>
        /// Initializes a new instance of EntityPlugin
        /// </summary>
        public EntityPlugin()
        {
            _messageQueue = new FastFifoQueue<SyslogMessage>(QUEUE_SIZE);
            _tmrStatistics = new Timer(LogStatistics, null, new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 0));

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
            _colModule = new DataColumn("Module", typeof(string))
                             {
                                 AllowDBNull = true,
                                 ReadOnly = true,
                                 Unique = false
                             };
            _colClass = new DataColumn("Class", typeof(string))
                            {
                                AllowDBNull = true,
                                ReadOnly = true,
                                Unique = false
                            };
            _colMethod = new DataColumn("Method", typeof(string))
                             {
                                 AllowDBNull = true,
                                 Unique = false,
                                 ReadOnly = true
                             };
            _colAppName = new DataColumn("AppName", typeof(string))
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
            _colChannelId = new DataColumn("ChannelId", typeof(string))
                                {
                                    AllowDBNull = true,
                                    ReadOnly = false,
                                    Unique = false
                                };
            _colFfdaChannelId = new DataColumn("FfdaChannelId", typeof(string))
                                    {
                                        AllowDBNull = true,
                                        ReadOnly = false,
                                        Unique = false
                                    };

            _entityTable.Columns.Add(_colHost);
            _entityTable.Columns.Add(_colProc);
            _entityTable.Columns.Add(_colLogger);
            _entityTable.Columns.Add(_colAppName);
            _entityTable.Columns.Add(_colFfda);
            _entityTable.Columns.Add(_colLastAction);
            _entityTable.Columns.Add(_colLastHeartbeat);
            _entityTable.Columns.Add(_colChannelId);
            _entityTable.Columns.Add(_colFfdaChannelId);

            try
            {
                _config = ConfigurationManager.GetSection("logbus-entityplugin") as EntityPluginConfiguration;

            }
            catch { }
            finally
            {
                if (_config == null || _config.entitykey == null || _config.entitykey.Length == 0) _config = DefaultConfiguration;
            }

            List<DataColumn> primaryKey = new List<DataColumn>(3);
            foreach (FieldType field in _config.entitykey)
            {
                switch (field)
                {
                    case FieldType.host:
                        {
                            if (_isHostKey) throw new InvalidOperationException("Primary key configuration broken");
                            primaryKey.Add(_colHost);
                            _isHostKey = true;
                            break;
                        }
                    case FieldType.process:
                        {
                            if (_isProcessKey) throw new InvalidOperationException("Primary key configuration broken");
                            primaryKey.Add(_colProc);
                            _isProcessKey = true;
                            break;
                        }
                    case FieldType.logger:
                        {
                            if (_isLoggerKey) throw new InvalidOperationException("Primary key configuration broken");
                            primaryKey.Add(_colLogger);
                            _isLoggerKey = true;
                            break;
                        }
                    case FieldType.module:
                        {
                            if (_isModuleKey) throw new InvalidOperationException("Primary key configuration broken");
                            primaryKey.Add(_colModule);
                            _isModuleKey = true;
                            break;
                        }
                    case FieldType.@class:
                        {
                            if (_isClassKey) throw new InvalidOperationException("Primary key configuration broken");
                            primaryKey.Add(_colClass);
                            _isClassKey = true;
                            break;
                        }
                    case FieldType.method:
                        {
                            if (_isMethodKey) throw new InvalidOperationException("Primary key configuration broken");
                            primaryKey.Add(_colMethod);
                            _isMethodKey = true;
                            break;
                        }
                }
            }

            _primaryKey = new UniqueConstraint("Primary", primaryKey.ToArray(), true);
            _entityTable.Constraints.Add(_primaryKey);
        }

        ~EntityPlugin()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            GC.SuppressFinalize(this);
            _tmrStatistics.Dispose();

            _workerThread.Abort();
            _workerThread.Join();

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

            _logbus = logbus;
            logbus.MessageReceived += LogbusMessageReceived;
        }

        /// <summary>
        /// Implements IPlugin.Unregister
        /// </summary>
        public void Unregister()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            _logbus.MessageReceived -= LogbusMessageReceived;
        }

        /// <summary>
        /// Implements ILogSupport.Log
        /// </summary>
        public ILog Log { private get; set; }

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

            WsdlSkeletonDefinition ret = new WsdlSkeletonDefinition
                                             {
                                                 SkeletonType = typeof(EntityManagementSkeleton),
                                                 UrlFileName = "EntityManagement"
                                             };
            return new[] { ret };
        }

        /// <summary>
        /// Implements IPlugin.GetPluginRoot
        /// </summary>
        public MarshalByRefObject GetPluginRoot()
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
            Dispose(true);
        }

        #endregion

        private void LogbusMessageReceived(object sender, SyslogMessageEventArgs e)
        {
            SyslogMessage message = e.Message;
            if (message.Facility == SyslogFacility.Internally) return; //Skip all syslog-internal messages
            _messageQueue.Enqueue(message);
        }

        private string GetQuery(SyslogMessage message)
        {
            List<string> criteria = new List<string>(3);
            SyslogAttributes attrs = message.GetAdvancedAttributes();
            foreach (DataColumn column in _primaryKey.Columns)
            {
                string colName = null, value = null;
                if (column == _colHost)
                {
                    colName = _colHost.ColumnName;
                    value = message.Host;
                }
                else if (column == _colProc)
                {
                    colName = _colProc.ColumnName;
                    value = message.ProcessID ?? message.ApplicationName;
                }
                else if (column == _colLogger)
                {
                    colName = _colLogger.ColumnName;
                    value = attrs.LogName;
                }
                else if (column == _colModule)
                {
                    colName = _colModule.ColumnName;
                    value = attrs.ModuleName;
                }
                else if (column == _colClass)
                {
                    colName = _colClass.ColumnName;
                    value = attrs.ClassName;
                }
                else if (column == _colMethod)
                {
                    colName = _colMethod.ColumnName;
                    value = attrs.MethodName;
                }
                criteria.Add(string.Format("{0} = '{1}'", colName, value));
            }
            return string.Join(" AND ", criteria.ToArray());
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

                    string host = message.Host ?? "",
                           process = message.ProcessID ?? message.ApplicationName ?? "",
                           logger = attrs.LogName ?? "";

                    bool ffda = message.MessageId == "FFDA";
                    if (message.MessageId == "HEARTBEAT" && message.Severity == SyslogSeverity.Debug)
                        lastHb = message.Timestamp;
                    try
                    {
                        string entityName = null; //Suppress error about missing initialization
                        try
                        {
                            DataRow newRow = _entityTable.Rows.Add(
                                host,
                                process,
                                logger,
                                message.ApplicationName ?? "",
                                ffda,
                                message.Timestamp,
                                lastHb,
                                DBNull.Value,
                                DBNull.Value
                                );

                            entityName = GetEntityName(newRow);

                            Log.Debug("Acquired new entity: {0}, {1}FFDA-enabled",
                                      entityName,
                                      ffda ? "" : "not ");

                            //Now creating channel for new entity
                            IFilter entityFilter = new EntityFilter(host, process, logger);
                            string description = string.Format("Channel monitoring logs from entity {0}",
                                                               entityName);
                            do
                            {
                                string randomChannelId = "em_" + Randomizer.RandomAlphanumericString(15);
                                try
                                {
                                    _logbus.CreateChannel(randomChannelId, "EntityManager auto-generated", entityFilter,
                                                          description, 0);
                                    //Edit row accordingly
                                    newRow[_colChannelId] = randomChannelId;
                                    break;
                                }
                                catch (LogbusException) //Duplicate channel ID
                                {
                                    continue;
                                }
                            } //Not necessarily a poor choice. With 15 chars we have billions of opportunities.
                            //In a real system, we can't have more than thousands of entities. This algorithm
                            //might go into stall only if randomizer is not "random" enough
                            while (true);

                            if (ffda) //Create FFDA channel too
                            {
                                entityFilter = new EntityFilter(host, process, logger, true);
                                description = string.Format("Channel monitoring FFDA logs from entity {0}",
                                                            entityName);
                                do
                                {
                                    string randomChannelId = "em_" + Randomizer.RandomAlphanumericString(15);
                                    try
                                    {
                                        _logbus.CreateChannel(randomChannelId, "EntityManager auto-generated",
                                                              entityFilter,
                                                              description, 0);
                                        //Edit row accordingly
                                        newRow[_colFfdaChannelId] = randomChannelId;
                                        break;
                                    }
                                    catch (LogbusException) //Duplicate channel ID
                                    {
                                        continue;
                                    }
                                } //Like above
                                while (true);
                            }
                        }
                        catch (ConstraintException)
                        {
                            //We suppose we are trying to insert a duplicate primary key, then now we switch to update
                            DataRow existingRow = _entityTable.Select(GetQuery(message))[0];
                            bool oldFfda = (bool)existingRow[_colFfda];

                            existingRow.BeginEdit();
                            existingRow[_colFfda] = ffda | oldFfda;

                            if (lastHb != null)
                                existingRow[_colLastHeartbeat] = message.Timestamp;
                            else
                                existingRow[_colLastAction] = message.Timestamp;

                            existingRow.EndEdit();

                            if (ffda && !oldFfda)
                            {
                                Log.Debug("Entity {0} is now FFDA-enabled",
                                          entityName);

                                //Create FFDA channel
                                IFilter entityFilter = new EntityFilter(host, process, logger, true);
                                string description =
                                    string.Format("Channel monitoring FFDA logs from entity {0}",
                                                  entityName);
                                do
                                {
                                    string randomChannelId = "em_" + Randomizer.RandomAlphanumericString(15);
                                    try
                                    {
                                        _logbus.CreateChannel(randomChannelId, "EntityManager auto-generated",
                                                              entityFilter,
                                                              description, 0);
                                        //Edit row accordingly
                                        existingRow[_colFfdaChannelId] = randomChannelId;
                                        break;
                                    }
                                    catch (LogbusException) //Duplicate channel ID
                                    {
                                        continue;
                                    }
                                } //Like above
                                while (true);
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Unable to add an entity row into the data table");
                        Log.Debug(string.Format("Error details: {0}", ex.Message));
                    }
                }
            }
            catch (ThreadAbortException) { }
        }

        /// <remarks/>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private void LogStatistics(object state)
        {
            Log.Debug("Entity Manager status. Pending {0} messages", _messageQueue.Count);
            ThreadPriority oldPrio = _workerThread.Priority,
                newPrio = (_messageQueue.Count > QUEUE_SIZE / 2)
                                          ? ThreadPriority.AboveNormal
                                          : ThreadPriority.Normal;

            if (oldPrio != newPrio)
            {
                Log.Debug("Entity Manager switching priority to {0} to deliver messages faster", Enum.GetName(typeof(ThreadPriority), newPrio));
                _workerThread.Priority = newPrio;
            }
        }

        private string GetEntityName(DataRow row)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('(');

            bool gotFirst = false;

            if (_isHostKey)
            {
                sb.Append(row[_colHost]);
                gotFirst = true;
            }

            if (_isProcessKey)
            {
                if (gotFirst) sb.Append('|');
                sb.Append(row[_colProc]);
                gotFirst = true;
            }

            if (_isLoggerKey)
            {
                if (gotFirst) sb.Append('|');
                sb.Append(row[_colLogger]);
                gotFirst = true;
            }

            if (_isModuleKey)
            {
                if (gotFirst) sb.Append('|');
                sb.Append(row[_colModule]);
                gotFirst = true;
            }

            if (_isClassKey)
            {
                if (gotFirst) sb.Append('|');
                sb.Append(row[_colClass]);
                gotFirst = true;
            }

            if (_isMethodKey)
            {
                if (gotFirst) sb.Append('|');
                sb.Append(row[_colMethod]);
            }

            sb.Append(')');

            return sb.ToString();
        }

        #region IEntityManagement Membri di

        /// <summary>
        /// Implements IEntityManagement.GetLoggingEntities
        /// </summary>
        public LoggingEntity[] GetLoggingEntities()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            return FindLoggingEntities(null);
        }

        /// <summary>
        /// Implements IEntityManagement.FindLoggingEntities
        /// </summary>
        public LoggingEntity[] FindLoggingEntities(TemplateQuery query)
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);

            DataRow[] rows;

            if (query == null)
                rows = _entityTable.Select();
            else
            {
                if (!_isHostKey && !string.IsNullOrEmpty(query.host))
                    throw new ArgumentException("Host is not part of entity definition");
                if (!_isProcessKey && !string.IsNullOrEmpty(query.process))
                    throw new ArgumentException("Process is not part of entity definition");
                if (!_isLoggerKey && !string.IsNullOrEmpty(query.logger))
                    throw new ArgumentException("Logger is not part of entity definition");
                if (!_isModuleKey && !string.IsNullOrEmpty(query.module))
                    throw new ArgumentException("Module is not part of entity definition");
                if (!_isClassKey && !string.IsNullOrEmpty(query.@class))
                    throw new ArgumentException("Class is not part of entity definition");
                if (!_isMethodKey && !string.IsNullOrEmpty(query.method))
                    throw new ArgumentException("Method is not part of entity definition");

                List<string> filters = new List<string>(6);
                if (!string.IsNullOrEmpty(query.appName))
                    filters.Add(string.Format("{0} = \"{1}\"", _colAppName.ColumnName, query.appName));
                if (!string.IsNullOrEmpty(query.host))
                    filters.Add(string.Format("{0} = \"{1}\"", _colHost.ColumnName, query.host));
                if (!string.IsNullOrEmpty(query.logger))
                    filters.Add(string.Format("{0} = \"{1}\"", _colLogger.ColumnName, query.logger));
                if (!string.IsNullOrEmpty(query.process))
                    filters.Add(string.Format("{0} = \"{1}\"", _colProc.ColumnName, query.process));
                if (query.ffdaSpecified)
                    filters.Add(string.Format("{0} = {1}", _colFfda.ColumnName, query.ffda));
                int maxinactivity;
                if (!string.IsNullOrEmpty(query.maxinactivity) &&
                    int.TryParse(query.maxinactivity, NumberStyles.Integer, CultureInfo.InvariantCulture,
                                 out maxinactivity))
                {
                    DateTime lastActivity = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(maxinactivity));
                    filters.Add(string.Format("({0} >= {2} OR {1} >= {2}}", _colLastAction.ColumnName,
                                              _colLastHeartbeat.ColumnName,
                                              lastActivity.ToString(CultureInfo.InvariantCulture)));
                }

                string filter = string.Join(" AND ", filters.ToArray());
                rows = _entityTable.Select(filter);
            }

            LoggingEntity[] ret = new LoggingEntity[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                ret[i] = new LoggingEntity
                             {
                                 host = (_isHostKey) ? (string)rows[i][_colHost] : null,
                                 process = (_isProcessKey) ? (string)rows[i][_colProc] : null,
                                 logger = (_isLoggerKey) ? (string)rows[i][_colLogger] : null,
                                 module = (_isModuleKey) ? (string)rows[i][_colModule] : null,
                                 @class = (_isClassKey) ? (string)rows[i][_colClass] : null,
                                 method = (_isMethodKey) ? (string)rows[i][_colMethod] : null,
                                 appName = (string)rows[i][_colAppName],
                                 ffda = (bool)rows[i][_colFfda],
                                 lastAction = (DateTime)rows[i][_colLastAction],
                                 lastHeartbeatSpecified = !(rows[i][_colLastHeartbeat] is DBNull),
                                 lastHeartbeat =
                                     (rows[i][_colLastHeartbeat] is DBNull)
                                         ? default(DateTime)
                                         : (DateTime)rows[i][_colLastHeartbeat],
                                 channelId = (rows[i][_colChannelId] is DBNull) ? null : (string)rows[i][_colChannelId],
                                 ffdaChannelId =
                                     (rows[i][_colFfdaChannelId] is DBNull) ? null : (string)rows[i][_colFfdaChannelId]
                             };
            }

            return ret;
        }

        /// <summary>
        /// Implements IEntityManagement.GetEntityDefinition
        /// </summary>
        public EntityDefinition GetEntityDefinition()
        {
            return new EntityDefinition
                       {
                           host = _isHostKey,
                           process = _isProcessKey,
                           logger = _isLoggerKey,
                           module = _isModuleKey,
                           @class = _isClassKey,
                           method = _isMethodKey
                       };
        }

        #endregion

        #region Proxy factory

        private static string UserAgent
        {
            get { return string.Format("LogbusEntityClient/{0}", typeof(EntityPlugin).Assembly.GetName().Version); }
        }

        /// <summary>
        /// Returns the default Entity Manager proxy for the given endpoint URL
        /// </summary>
        /// <param name="endpointUrl">URL endpoint for Entity Manager</param>
        /// <returns></returns>
        public static IEntityManagement GetProxy(string endpointUrl)
        {
            return new EntityManagement { Url = endpointUrl, UserAgent = UserAgent };
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
                throw new InvalidOperationException(
                    "Logbus-ng client is not configured. Cannot guess default endpoint URL", ex);
            }
        }

        #endregion

    }
}