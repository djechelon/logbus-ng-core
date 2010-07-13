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
namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// Defines the string-string configuration facility to ease dynamic instantiation
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        /// Gets a configuration parameter by name
        /// </summary>
        /// <param name="key">Name of configuration parameter</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Key is null</exception>
        /// <exception cref="NotSupportedException">Key is not supported by this object</exception>
        string GetConfigurationParameter(string key);

        /// <summary>
        /// Sets a configuration parameter by key
        /// </summary>
        /// <param name="key">Name of configuration parameter</param>
        /// <param name="value">Value to set. Can be null</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Key is null</exception>
        /// <exception cref="ArgumentException">Value not accepted/supported</exception>
        /// <exception cref="NotSupportedException">Key is not supported by this transport factory</exception>
        void SetConfigurationParameter(string key, string value);

        /// <summary>
        /// Gets or sets all configuration parameters in one shot
        /// </summary>
        /// <exception cref="ArgumentNullException">List is null</exception>
        /// <exception cref="NotSupportedException">One of the keys is not supported</exception>
        /// <exception cref="ArgumentException">One of the values is not accepted as valid</exception>
        IEnumerable<KeyValuePair<string, string>> Configuration { set; }
    }
}
