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
using System.Xml;
using System.Text;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// A syslog (RFC 5424) message
    /// </summary>
    /// <remarks>Currently, it can be serialized only into RFC5424 standard</remarks>
    [Serializable()]
    public struct SyslogMessage
    {


        public SyslogFacility Facility
        {
            get;
            set;
        }

        public SyslogSeverity Severity
        {
            get;
            set;
        }

        public DateTime? Timestamp
        {
            get;
            set;
        }

        public int Version
        {
            get;
            set;
        }
        public string Host
        {
            get;
            set;
        }


        public string ApplicationName
        {
            get;
            set;
        }

        public string ProcessID
        {
            get;
            set;
        }

        public string MessageId
        {
            get;
            set;
        }

        public IDictionary<string, IDictionary<string, string>> Data
        {
            get;
            set;
        }


        public string Text
        {
            get;
            set;
        }



        public SyslogMessage(DateTime? timestamp, string host, SyslogFacility facility, SyslogSeverity level, string text, IDictionary<string, IDictionary<string, string>> data)
            : this()
        {
            Timestamp = timestamp;
            Host = host;
            Facility = facility;
            Severity = level;
            Text = text;
            Data = data;
        }

        public byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }

        public override string ToString()
        {
            int prival = (int)Facility * 8 + (int)Severity;

            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException">Thrown when message is not Syslog-compliant</exception>
        public static SyslogMessage Parse(string payload)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="FormatException">Thrown when message is not Syslog-compliant</exception>
        public static SyslogMessage Parse(byte[] payload)
        {
            return Parse(System.Text.Encoding.UTF8.GetString(payload));
        }
    }
}
