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
namespace It.Unina.Dis.Logbus.Entities
{
    /// <summary>
    /// Plugin that manages Logbus-ng entities, where the entity is user-defined by configuration
    /// </summary>
    public sealed class EntityPlugin
        : IPlugin
    {
        private ILogBus _logbus;
        private BlockingFifoQueue<SyslogMessage> message_queue;
        private Thread worker_thread;

        private DataColumn colHost, colProc, colModule, colLogger, colClass, colMethod, colFFDA, colLastAction;
        private UniqueConstraint primary_key;
        private DataTable entity_table;

        #region Constructor/Destructor

        public EntityPlugin()
        {
            message_queue = new BlockingFifoQueue<SyslogMessage>();

            worker_thread = new Thread(WorkerLoop);
            worker_thread.IsBackground = true;
            worker_thread.Name = "EntityPlugin.WorkerLoop";
            worker_thread.Start();

            entity_table = new DataTable("Entities");
            colHost = new DataColumn("Host", typeof(string))
            {
                AllowDBNull = true,
                ReadOnly = true,
                Unique = false
            };
            colProc = new DataColumn("Process", typeof(string))
            {
                AllowDBNull = true,
                ReadOnly = true,
                Unique = false
            };
            colModule = new DataColumn("Module", typeof(string))
            {
                AllowDBNull = true,
                ReadOnly = true,
                Unique = false
            };
            colLogger = new DataColumn("Logger", typeof(string))
            {
                AllowDBNull = true,
                ReadOnly = true,
                Unique = false
            };
            colClass = new DataColumn("Class", typeof(string))
            {
                AllowDBNull = true,
                ReadOnly = true,
                Unique = false
            };
            colMethod = new DataColumn("Method", typeof(string))
            {
                AllowDBNull = true,
                ReadOnly = true,
                Unique = false
            };
            colFFDA = new DataColumn("FFDA", typeof(bool))
            {
                AllowDBNull = false,
                DefaultValue = false,
                ReadOnly = false,
                Unique = false
            };
            colLastAction = new DataColumn("LastAction", typeof(DateTime))
            {
                AllowDBNull = false,
                ReadOnly = false,
                Unique = false,
                DateTimeMode = DataSetDateTime.Utc,
            };

            entity_table.Columns.Add(colHost);
            entity_table.Columns.Add(colProc);
            entity_table.Columns.Add(colModule);
            entity_table.Columns.Add(colLogger);
            entity_table.Columns.Add(colClass);
            entity_table.Columns.Add(colMethod);
            entity_table.Columns.Add(colFFDA);
            entity_table.Columns.Add(colLastAction);

            primary_key = new UniqueConstraint(new DataColumn[] { colHost, colProc, colModule, colLogger, colClass, colMethod }, true)
            {
                ConstraintName = "Primary"
            };
            entity_table.Constraints.Add(primary_key);
        }

        ~EntityPlugin()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            worker_thread.Interrupt();

            if (disposing)
            {
                message_queue.Dispose();
                entity_table.Dispose();
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
            get { return "Logbus.EntityManager"; }
        }

        /// <summary>
        /// Implements IPlugin.GetWsdlSkeletons
        /// </summary>
        public WsdlSkeletonDefinition[] GetWsdlSkeletons()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Implements IPlugin.GetPluginRoot
        /// </summary>
        public System.MarshalByRefObject GetPluginRoot()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
            throw new System.NotImplementedException();
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
            message_queue.Enqueue(e.Message);
        }

        private void WorkerLoop()
        {
            try
            {
                while (true)
                {
                    SyslogMessage message = message_queue.Dequeue();

                    if (message.Facility == SyslogFacility.Internally) continue; //Skip all syslog-internal messages

                    SyslogAttributes attrs = message.GetAdvancedAttributes();

                    bool ffda = message.MessageId == "FFDA";
                    try
                    {
                        try
                        {

                            entity_table.Rows.Add(
                                message.Host,
                                message.ProcessID ?? message.ApplicationName,
                                attrs.ModuleName,
                                attrs.LogName,
                                attrs.ClassName,
                                attrs.MethodName,
                                ffda,
                                message.Timestamp
                                );
                        }
                        catch (ConstraintException)
                        {
                            //We suppose we are trying to insert a duplicate primary key, then now we switch to update
                            object[] keys = new object[]{
                                message.Host,
                                message.ProcessID ?? message.ApplicationName,
                                attrs.ModuleName,
                                attrs.LogName,
                                attrs.ClassName,
                                attrs.MethodName
                            };
                            DataRow existingRow = entity_table.Rows.Find(keys);

                            if (existingRow[colFFDA].Equals((true))) continue; //No need to update a correct row
                            existingRow.BeginEdit();
                            existingRow[colFFDA] = ffda;
                            existingRow[colLastAction] = message.Timestamp;
                            existingRow.EndEdit();

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

    }
}
