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

using System.Configuration;
using System.Xml.Serialization;
using System;
using System.Xml;
namespace It.Unina.Dis.Logbus.Configuration
{
    /// <summary>
    /// Configuration section handler for App.config/Web.config
    /// </summary>
    public class LogbusSourceConfigurationSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Membri di

        /// <summary>
        /// Read configuration and return a <see cref="It.Unina.Dis.Logbus.Configuration.LogbusSourceConfiguration"/> object by design contract
        /// </summary>
        /// <returns></returns>
        object IConfigurationSectionHandler.Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            try
            {
                return new XmlSerializer(typeof(LogbusSourceConfiguration)).Deserialize(new XmlNodeReader(section));
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        #endregion
    }
}
