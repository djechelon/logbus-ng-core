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
namespace It.Unina.Dis.Logbus.Configuration
{
    /// <summary>
    /// Configuration section handler for App.config/Web.config
    /// </summary>
    public class LogbusConfigSectionHandler: IConfigurationSectionHandler
    {
       

        #region IConfigurationSectionHandler Membri di

        /// <summary>
        /// Read configuration and return a <see cref="LogbusConfiguration"/> object by design contract
        /// </summary>
        /// <returns></returns>
        
        object IConfigurationSectionHandler.Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            throw new System.NotImplementedException();
            throw new ConfigurationException();
        }

        #endregion
    }
}
