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

        Kernel = 0,     // kernel messages
        User = 1,     // user-level messages
        Mail = 2,     // mail system
        System = 3,     // system daemons
        Security = 4,     // security/authorization messages (note 1)
        Internally = 5,     // messages generated internally by syslogd
        Printer = 6,     // line printer subsystem
        News = 7,     // network news subsystem
        Uucp = 8,     // UUCP subsystem
        Cron = 9,     // clock daemon (note 2) changed to cron
        Security2 = 10,    // security/authorization messages (note 1)
        Ftp = 11,    // FTP daemon
        Ntp = 12,    // NTP subsystem
        Audit = 13,    // log audit (note 1)
        Alert = 14,    // log alert (note 1)
        Clock2 = 15,    // clock daemon (note 2)
        Local0 = 16,    // local use 0  (local0)
        Local1 = 17,    // local use 1  (local1)
        Local2 = 18,    // local use 2  (local2)
        Local3 = 19,    // local use 3  (local3)
        Local4 = 20,    // local use 4  (local4)
        Local5 = 21,    // local use 5  (local5)
        Local6 = 22,    // local use 6  (local6)
        Local7 = 23,    // local use 7  (local7)
    }
}
