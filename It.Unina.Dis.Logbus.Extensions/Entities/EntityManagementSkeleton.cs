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

using System.Web.Services;
using System;
namespace It.Unina.Dis.Logbus.Entities
{
    public sealed class EntityManagementSkeleton
        : WebService, IEntityManagement
    {

        private readonly IEntityManagement _proxy;

        public EntityManagementSkeleton()
            :base()
        {
            _proxy = (Application[EntityPlugin.PLUGIN_ID] as IEntityManagement ?? AppDomain.CurrentDomain.GetData(EntityPlugin.PLUGIN_ID)) as IEntityManagement;

            if (_proxy==null) throw new InvalidOperationException("No Entity Manager proxy found");
        }

        #region IEntityManagement Membri di

        public LoggingEntity[] GetLoggingEntities()
        {
            return _proxy.GetLoggingEntities();
        }

        public LoggingEntity[] FindLoggingEntities(TemplateQuery query)
        {
            return _proxy.FindLoggingEntities(query);
        }

        #endregion
    }
}
