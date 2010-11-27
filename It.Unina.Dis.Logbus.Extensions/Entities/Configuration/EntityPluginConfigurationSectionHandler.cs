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
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace It.Unina.Dis.Logbus.Entities.Configuration
{
    class EntityPluginConfigurationSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Membri di

        /// <summary>
        /// Read configuration and return a <see cref="It.Unina.Dis.Logbus.Entities.Configuration.EntityPluginConfiguration"/> object by design contract
        /// </summary>
        /// <returns></returns>
        object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
        {
            try
            {
                return new XmlSerializer(typeof(EntityPluginConfiguration)).Deserialize(new XmlNodeReader(section));
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        #endregion

    }
}
