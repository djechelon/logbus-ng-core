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
using System.Web.Services;

namespace It.Unina.Dis.Logbus.Entities
{
    /// <summary>
    /// WSDL skeleton class for EntityPlugin
    /// </summary>
    public sealed class EntityManagementSkeleton
        : WebService, IEntityManagement
    {

        private readonly IEntityManagement _proxy;

        /// <summary>
        /// Initializes a new instance of EntityManagerSkeleton
        /// </summary>
        public EntityManagementSkeleton()
        {
            _proxy =
                (Application[EntityPlugin.PLUGIN_ID] as IEntityManagement ??
                 AppDomain.CurrentDomain.GetData(EntityPlugin.PLUGIN_ID)) as IEntityManagement;

            if (_proxy == null)
                throw new InvalidOperationException(
                    "No Entity Manager proxy found. Perhaps it has not been activated in App.config/Web.config.");
        }

        #region IEntityManagement Membri di

        /// <summary>
        /// Implements IEntityManagement.GetLoggingEntities
        /// </summary>
        public LoggingEntity[] GetLoggingEntities()
        {
            return _proxy.GetLoggingEntities();
        }

        /// <summary>
        /// Implements IEntityManagement.FindLoggingEntities
        /// </summary>
        public LoggingEntity[] FindLoggingEntities(TemplateQuery query)
        {
            return _proxy.FindLoggingEntities(query);
        }

        /// <summary>
        /// Implements IEntityManagement.GetEntityDefinition
        /// </summary>
        /// <returns></returns>
        public EntityDefinition GetEntityDefinition()
        {
            return _proxy.GetEntityDefinition();
        }

        #endregion
    }
}
