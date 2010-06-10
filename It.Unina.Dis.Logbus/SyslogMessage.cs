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
            SyslogMessage ret = new SyslogMessage();
            try
            {
                int pointer = 1;
                String new_payload = payload.Substring(pointer);
                
                // Calculte prival = Facility*8 + Severity...
                String prival = new_payload.Split('>')[0];
                Int32 severity = 0;
                ret.Facility = (SyslogFacility)Math.DivRem(Int32.Parse(prival), 8, out severity);
                ret.Severity = (SyslogSeverity)severity;
                pointer += prival.Length + 1;

                // Calclate Version...
                new_payload = payload.Substring(pointer);
                ret.Version = Int32.Parse(new_payload.Substring(0, 1));
                pointer += 2;

                //Calculate Timestamp...
                new_payload = payload.Substring(pointer);
                String timestamp = new_payload.Split(' ')[0];

                if (timestamp != "-")
                {
                    String[] elem = timestamp.Split('T');

                    Int32 year = Int32.Parse(elem[0].Split('-')[0]);
                    Int32 month = Int32.Parse(elem[0].Split('-')[1]);
                    Int32 day = Int32.Parse(elem[0].Split('-')[2]);

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
                    Int32 hour = Int32.Parse(elem2[0].Split(':')[0]);
                    Int32 minute = Int32.Parse(elem2[0].Split(':')[1]);
                    Int32 sec = Int32.Parse(elem2[0].Split(':')[2].Split('.')[0]);
                    Int32 msec = Int32.Parse(elem2[0].Split(':')[2].Split('.')[1].Substring(0, 3));

                    ret.Timestamp = new DateTime(year, month, day, hour, minute, sec, msec);
                    ret.Timestamp = ret.Timestamp.Value.AddHours(fusoH);
                    ret.Timestamp = ret.Timestamp.Value.AddHours(fusoM);
                }
                else
                    ret.Timestamp = null;
                pointer += timestamp.Length + 1;

                //Calculate HostIP...
                new_payload = payload.Substring(pointer);
                ret.Host = (new_payload.Split(' ')[0] == "-") ? null : new_payload.Split(' ')[0];
                pointer += (ret.Host == null) ? 2 : ret.Host.Length + 1;

                //Calculate AppName...
                new_payload = payload.Substring(pointer);
                ret.ApplicationName = (new_payload.Split(' ')[0] == "-") ? null : new_payload.Split(' ')[0];
                pointer += (ret.ApplicationName == null) ? 2 : ret.ApplicationName.Length + 1;

                //Calculate ProcID...
                new_payload = payload.Substring(pointer);
                ret.ProcessID = (new_payload.Split(' ')[0] == "-") ? null : new_payload.Split(' ')[0];
                pointer += (ret.ProcessID == null) ? 2 : ret.ProcessID.Length + 1;

                //Calculate MessageID...
                new_payload = payload.Substring(pointer);
                ret.MessageId = (new_payload.Split(' ')[0] == "-") ? null : new_payload.Split(' ')[0];
                pointer += (ret.MessageId == null) ? 2 : ret.MessageId.Length + 1;

                //Calculate StructuredData...
                new_payload = payload.Substring(pointer);
                String StructuredData = "[";
                if (new_payload.StartsWith("-"))
                {
                    ret.Data = null;
                }
                else
                {
                    for (int j = 1; j < new_payload.Length-1; j++)
                    {
                        StructuredData += new_payload[j];
                        if(new_payload[j-1] == '"' && new_payload[j] == ']' && new_payload[j+1] == ' ')
                            break;
                    }
                    if (!StructuredData.EndsWith("]"))
                        StructuredData += ']';
                    ret.Data = new Dictionary<string, IDictionary<string, string>>();
                    String[] elementi = StructuredData.Split('[', ']');
                    for (int i = 0; i < elementi.Length; i++)
                    {
                        if (elementi[i].Length == 0)
                            continue;
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
                pointer += StructuredData.Length + 1;

                //Calculate Msg if present...
                if (pointer >= payload.Length)
                    ret.Text = null;
                else
                {
                    new_payload = payload.Substring(pointer);
                    // Controls the presence of BOM...
                    byte[] BOM = { 0xEF, 0xBB, 0xBF };
                    if (new_payload[0] == BOM[0] && new_payload[1] == BOM[1] && new_payload[2] == BOM[2])
                        ret.Text = new_payload.Substring(3);
                    else
                        ret.Text = new_payload;
                }
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
