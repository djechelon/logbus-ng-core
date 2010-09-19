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

using System.Runtime.Serialization;
using System;

namespace It.Unina.Dis.Logbus.Configuration
{
    /// <summary>
    /// Configuration error for Logbus
    /// </summary>
    public class LogbusConfigurationException
        :LogbusException
    {

        /// <remarks/>
        public LogbusConfigurationException()
            : base() { }

        /// <remarks/>
        public LogbusConfigurationException(string message)
            : base(message) { }

        /// <remarks/>
        protected LogbusConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <remarks/>
        public LogbusConfigurationException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Object that represents Logbus configuration
        /// </summary>
        public object ConfigurationObject
        { get; set; }
    }
}
