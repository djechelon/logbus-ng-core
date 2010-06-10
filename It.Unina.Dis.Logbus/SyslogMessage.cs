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
            //<165>1 2003-08-24T05:14:15.000003-07:00 192.0.2.1 myproc 8710 - - %% It’s time to make the do-nuts.
            SyslogMessage ret = new SyslogMessage();
            try
            {
                String new_payload = payload.Substring(1);
                //new_payload = 165>1 2003-08-24T05:14:15.000003-07:00 192.0.2.1 myproc 8710 - - %% It’s time to make the do-nuts.
            
                // Calculte prival = Facility*8 + Severity...
                String prival = new_payload.Split('>')[0];
                Int32 facility = 0;
                ret.Severity = (SyslogSeverity)Math.DivRem(Int32.Parse(prival), 8, out facility);
                ret.Facility = (SyslogFacility)facility;
                
                // Calclate Version...
                new_payload = new_payload.Substring(prival.Length + 1);
                // new_payload = 1 2003-08-24T05:14:15.000003-07:00 192.0.2.1 myproc 8710 - - %% It’s time to make the do-nuts.
                ret.Version = Int32.Parse(new_payload.Substring(0, 1));
                
                //Calculate Timestamp...
                new_payload = new_payload.Substring(2);
                // new_payload = 2003-08-24T05:14:15.000003-07:00 192.0.2.1 myproc 8710 - - %% It’s time to make the do-nuts.
                String timestamp = new_payload.Split(' ')[0];
                // timestamp = 2003-08-24T05:14:15.000003-07:00
                if (timestamp != "-")
                {
                    String[] elem = timestamp.Split('T');
                    // elem[0] = 2003-08-24
                    Int32 year = Int32.Parse(elem[0].Split('-')[0]);
                    Int32 month = Int32.Parse(elem[0].Split('-')[1]);
                    Int32 day = Int32.Parse(elem[0].Split('-')[2]);
                    // elem[1] = 05:14:15.000003-07:00
                    String[] elem2;
                    Int32 fusoH = 0;
                    Int32 fusoM = 0;
                    if (elem[1].Contains("-"))
                    {
                        elem2 = elem[1].Split('-');
                        fusoH = Int32.Parse(elem2[1].Split(':')[0]) * -1;
                        fusoM = Int32.Parse(elem2[1].Split(':')[1]) * -1;
                    }
                    else if (elem[1].Contains("+"))
                    {
                        elem2 = elem[1].Split('+');
                        fusoH = Int32.Parse(elem2[1].Split(':')[0]);
                        fusoM = Int32.Parse(elem2[1].Split(':')[1]);
                    }
                    else
                    {
                        elem2 = elem[1].Split('Z');
                    }
                    // elem2[0] = 05:14:15.000003
                    Int32 hour = Int32.Parse(elem2[0].Split(':')[0]);
                    Int32 minute = Int32.Parse(elem2[0].Split(':')[1]);
                    Int32 sec = Int32.Parse(elem2[0].Split(':')[2].Split('.')[0]);
                    Int32 msec = Int32.Parse(elem2[0].Split(':')[2].Split('.')[1].Substring(0,3));
                        
                    ret.Timestamp = new DateTime(year, month, day, hour, minute, sec, msec);
                    ret.Timestamp = ret.Timestamp.Value.AddHours(fusoH);
                    ret.Timestamp = ret.Timestamp.Value.AddHours(fusoM);
                }
                else
                    ret.Timestamp = null;
                
                //Calculate HostIP...
                new_payload = new_payload.Substring(timestamp.Length + 1);
                // new_payload = 192.0.2.1 myproc 8710 - - %% It’s time to make the do-nuts.
                ret.Host = (new_payload.Split(' ')[0] == "-") ? null : new_payload.Split(' ')[0];
                
                //Calculate AppName...
                new_payload = new_payload.Substring(ret.Host.Length + 1);
                // new_payload = myproc 8710 - - %% It’s time to make the do-nuts.
                ret.ApplicationName = (new_payload.Split(' ')[0] == "-") ? null : new_payload.Split(' ')[0];

                //Calculate ProcID...
                new_payload = new_payload.Substring(ret.ApplicationName.Length + 1);
                // new_payload = 8710 - - %% It’s time to make the do-nuts.
                ret.ProcessID = (new_payload.Split(' ')[0] == "-") ? null : new_payload.Split(' ')[0];

                //Calculate MessageID...
                new_payload = new_payload.Substring(ret.ProcessID.Length + 1);
                // new_payload = - - %% It’s time to make the do-nuts.
                ret.MessageId = (new_payload.Split(' ')[0] == "-") ? null : new_payload.Split(' ')[0];

                //Calculate StructuredData...
                new_payload = new_payload.Substring(ret.MessageId.Length + 1);
                // new_payload = - %% It’s time to make the do-nuts.
                String StructuredData = new_payload.Split(' ')[0];
                if (StructuredData != "-")
                {
                    ret.Data = new Dictionary<string, IDictionary<string, string>>();
                    String[] elementi = StructuredData.Split('[', ']');
                    for (int i = 0; i < elementi.Length; i++)
                    {
                        String[] SubElem = elementi[i].Split(' ');
                        String key = SubElem[0];
                        Dictionary<string, string> values = new Dictionary<string, string>();
                        for (int k = 1; k < SubElem.Length; k++)
                        {
                            String[] SubSubElem = SubElem[k].Split('=');
                            values.Add(SubSubElem[0], SubSubElem[1]);
                        }
                        ret.Data.Add(key, values);
                    }
                }
                else
                    ret.Data = null;

                //Calculate Msg...
                new_payload = new_payload.Substring(StructuredData.Length + 1);
                // new_payload = %% It’s time to make the do-nuts.
                // Controls the presence of BOM...
                byte[] BOM = { 0xEF, 0xBB, 0xBF };
                if (new_payload[0] == BOM[0] && new_payload[1] == BOM[1] && new_payload[2] == BOM[2])
                    ret.Text = new_payload.Substring(3);
                else
                    ret.Text = new_payload;

                // Return the SyslogMessage...
                return ret;
            }
            catch (Exception e)
            {
                FormatException ex = new FormatException("Incorrect start of message...");
                ex.Data.Add("Payload", payload);
                throw ex;
            }
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
