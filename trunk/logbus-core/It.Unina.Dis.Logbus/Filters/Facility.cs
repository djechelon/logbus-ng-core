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

namespace It.Unina.Dis.Logbus.Filters
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
    public enum Facility
    {
        /// <summary>
        /// Kernel messages
        /// </summary>
        Kernel = 0,

        /// <summary>
        /// User-level messages
        /// </summary>
        User = 1,

        /// <summary>
        /// Mail system
        /// </summary>
        Mail = 2,

        /// <summary>
        /// System daemons
        /// </summary>
        System = 3,

        /// <summary>
        /// Security/authorization messages (note 1)
        /// </summary>
        Security = 4,

        /// <summary>
        /// Messages generated internally by syslogd
        /// </summary>
        Internally = 5,

        /// <summary>
        /// Line printer subsystem
        /// </summary>
        Printer = 6,

        /// <summary>
        /// Network news subsystem
        /// </summary>
        News = 7,

        /// <summary>
        /// UUCP subsystem
        /// </summary>
        Uucp = 8,

        /// <summary>
        /// Clock daemon (note 2) changed to cron
        /// </summary>
        Cron = 9,

        /// <summary>
        /// Security/authorization messages (note 1)
        /// </summary>
        Security2 = 10,

        /// <summary>
        /// FTP daemon
        /// </summary>
        Ftp = 11,

        /// <summary>
        /// NTP subsystem
        /// </summary>
        Ntp = 12,

        /// <summary>
        /// Log audit (note 1)
        /// </summary>
        Audit = 13,

        /// <summary>
        /// Log alert (note 1)
        /// </summary>
        Alert = 14,

        /// <summary>
        /// Clock daemon (note 2)
        /// </summary>
        Clock2 = 15,

        /// <summary>
        /// Local use 0  (local0)
        /// </summary>
        Local0 = 16,

        /// <summary>
        /// Local use 1  (local1)
        /// </summary>
        Local1 = 17,

        /// <summary>
        /// Local use 2  (local2)
        /// </summary>
        Local2 = 18,

        /// <summary>
        /// Local use 3  (local3)
        /// </summary>
        Local3 = 19,

        /// <summary>
        /// Local use 4  (local4)
        /// </summary>
        Local4 = 20,

        /// <summary>
        /// Local use 5  (local5)
        /// </summary>
        Local5 = 21,

        /// <summary>
        /// Local use 6  (local6)
        /// </summary>
        Local6 = 22,

        /// <summary>
        /// Local use 7  (local7)
        /// </summary>
        Local7 = 23
    }
}
