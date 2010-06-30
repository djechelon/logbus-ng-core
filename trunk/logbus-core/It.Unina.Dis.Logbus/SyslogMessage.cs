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
using System.Globalization;
using System.Text.RegularExpressions;

namespace It.Unina.Dis.Logbus
{
    /// <summary>
    /// A syslog (RFC 5424) message
    /// </summary>
    /// <remarks>Currently, it can be serialized only into RFC5424 standard</remarks>
    [Serializable()]
    public struct SyslogMessage
    {

        /// <summary>
        ///	Facility that generated the message 
        /// </summary>
        public SyslogFacility Facility { get; set; }

        /// <summary>
        ///	Severity level of the message 
        /// </summary>
        public SyslogSeverity Severity { get; set; }

        /// <summary>
        ///	Time when the message was generated, if available 
        /// </summary>
        public DateTime? Timestamp { get; set; }

        /// <summary>
        ///	Syslog version 
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        ///	Hostname that generated the message, if available 
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        ///	Application that generated the message, if available 
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        ///	ID of process that generated the message, if available 
        /// </summary>
        public string ProcessID { get; set; }

        /// <summary>
        ///	Application-specific ID of the message, if available 
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        ///	Additional information generated by the source 
        /// </summary>
        public IDictionary<string, IDictionary<string, string>> Data { get; set; }

        /// <summary>
        ///	Human-readable text 
        /// </summary>
        public string Text { get; set; }

        public SyslogMessage(DateTime? timestamp, string host, SyslogFacility facility, SyslogSeverity level, string text)
            : this()
        {
            Timestamp = timestamp;
            Host = host;
            Facility = facility;
            Severity = level;
            Text = text;
        }

        #region Conversion

        private int PriVal
        {
            get
            {
                return (int)Facility * 8 + (int)Severity;
            }
        }

        /// <summary>
        ///	Converts the object into RFC5424 UTF-8 binary representation 
        /// </summary>
        /// <returns>
        /// A <see cref="System.Byte[]"/>
        /// </returns>
        public byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(ToRfc5424String());
        }

        /// <summary>
        /// Converts the object into RFC3164 string representation 
        /// </summary>
        /// <returns></returns>
        [Obsolete("You should avoid to use RFC 3164 as it's ambiguous and outdated")]
        public string ToRfc3164String()
        {
            StringBuilder ret = new StringBuilder();

            const string SPACE = @" ";

            //Encode Prival
            ret.AppendFormat("<{0}>", PriVal.ToString(CultureInfo.InvariantCulture));

            //Encode datetime
            //If unknown use local time
            ret.Append((Timestamp.HasValue) ? Timestamp.Value.ToString("MMM  dd hh:mm:ss") : DateTime.Now.ToString("MMM  dd hh:mm:ss"));
            ret.Append(SPACE);

            //Encode hostname
            ret.Append(Host);
            ret.Append(SPACE);

            //Start BODY with TAG/AppName
            ret.Append(ApplicationName);
            if (ProcessID != null) ret.AppendFormat("[{0}]:", ProcessID);
            else ret.Append(SPACE); //We freely chose space. Could use other illegal chars (ambiguity!!!)

            ret.Append(Text);

            return ret.ToString();
        }

        /// <summary>
        ///	Converts the object into RFC5424 UTF-8 string representation 
        /// </summary>
        /// <returns>
        /// </returns>
        public string ToRfc5424String()
        {
            StringBuilder ret = new StringBuilder();

            const string NILVALUE = @"-";
            const string SPACE = @" ";

            //Prival+version
            ret.AppendFormat("<{0}>{1}", PriVal.ToString(CultureInfo.InvariantCulture), Version.ToString(CultureInfo.InvariantCulture));
            ret.Append(SPACE);

            //Timestamp
            const string TIMESTAMP_FORMAT = @"yyyy-MM-dd\Thh:mm:ss\Z";
            ret.Append((Timestamp == null) ? NILVALUE : Timestamp.Value.ToString(TIMESTAMP_FORMAT));
            ret.Append(SPACE);

            //Hostname
            ret.Append((Host == null) ? NILVALUE : Host);
            ret.Append(SPACE);

            //AppName
            ret.Append((ApplicationName == null) ? NILVALUE : ApplicationName);
            ret.Append(SPACE);

            //procName
            ret.Append((ProcessID == null) ? NILVALUE : ProcessID);
            ret.Append(SPACE);

            //msgID
            ret.Append((MessageId == null) ? NILVALUE : MessageId);
            ret.Append(SPACE);

            //Structured Data			
            if (Data == null)
                ret.Append(NILVALUE);
            else
            {
                foreach (KeyValuePair<string, IDictionary<string, string>> kvp in Data)
                {
                    ret.Append(ToStringData(kvp.Key, kvp.Value));
                }
            }

            //Text
            if (Text != null)
            {
                byte[] BOM = { 0xef, 0xbb, 0xbf };
                ret.Append(SPACE);
                ret.Append(BOM);
                ret.Append(Text);
            }


            return ret.ToString();
        }

        /// <summary>
        ///	Converts the object into RFC5424 UTF-8 string representation 
        /// </summary>
        public override string ToString()
        {
            return ToRfc5424String();
        }

        private string ToStringData(string key, IDictionary<string, string> data)
        {
            StringBuilder ret = new StringBuilder();

            ret.Append('[');
            ret.Append(key);

            List<string> elements = new List<string>();
            foreach (KeyValuePair<string, string> kvp in data)
            {
                StringBuilder escape_builder = new StringBuilder();
                elements.Add(kvp.Key + "=\"" + Escape(kvp.Value, new char[] { '"', '\\', ']' }) + "\"");
            }

            if (elements.Count > 0)
            {
                ret.Append(' ');
                ret.Append(string.Join(" ", elements.ToArray()));
            }

            ret.Append(']');
            return ret.ToString();
        }

        private string Escape(string input, char[] specialChars)
        {
            StringBuilder ret = new StringBuilder();
            foreach (char c in input)
            {
                foreach (char comp in specialChars)
                {
                    if (c == comp)
                    {
                        ret.Append('\\');
                        break;
                    }
                    ret.Append(c);
                }
            }

            return ret.ToString();
        }

        #endregion

        #region Parsing
        /// <summary>
        /// Parses Syslog messages according to RFC3164
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private static SyslogMessage Parse3164(string payload)
        {
            SyslogMessage ret = new SyslogMessage();
            try
            {
                int pointer = 1;
                String new_payload = payload.Substring(pointer);

                //Calculate prival = Facility*8 + Severity...
                String prival = new_payload.Split('>')[0];
                Int32 severity = 0;
                ret.Facility = (SyslogFacility)Math.DivRem(Int32.Parse(prival), 8, out severity);
                ret.Severity = (SyslogSeverity)severity;
                pointer += prival.Length + 1;

                //Calculate Version...
                ret.Version = 0;

                //Calculate Timestamp...
                new_payload = payload.Substring(pointer);
                Int32 Month = GetMonthByName(new_payload.Substring(0, 3));
                pointer += 4;
                new_payload = payload.Substring(pointer);

                Int32 Day = 0;
                if (new_payload[0] == ' ')
                    Day = Int32.Parse(new_payload.Substring(1, 1));
                else
                    Day = Int32.Parse(new_payload.Substring(0, 2));
                pointer += 3;
                new_payload = payload.Substring(pointer);

                String timestamp = new_payload.Split(' ')[0];
                Int32 hour = Int32.Parse(timestamp.Split(':')[0]);
                Int32 minute = Int32.Parse(timestamp.Split(':')[1]);
                Int32 sec = Int32.Parse(timestamp.Split(':')[2]);
                ret.Timestamp = new DateTime(DateTime.Today.Year, Month, Day, hour, minute, sec, 0);
                pointer += timestamp.Length + 1;

                //Calculate HostIP...
                new_payload = payload.Substring(pointer);
                ret.Host = new_payload.Split(' ')[0];
                pointer += ret.Host.Length + 1;

                //Calculate AppName...
                new_payload = payload.Substring(pointer);
                String temp = new_payload.Split(' ')[0];
                if (temp.Contains("["))
                {
                    ret.ApplicationName = temp.Split('[')[0];
                    ret.ProcessID = temp.Split('[')[1].Split(']')[0];
                }
                else
                {
                    ret.ApplicationName = temp;
                    ret.ProcessID = null;
                }
                pointer += temp.Length + 1;

                //Calculate MessageID...
                ret.MessageId = null;

                //Calculate StructuredData...
                ret.Data = null;

                //Calculate Msg if present...
                if (pointer >= payload.Length)
                    ret.Text = null;
                else
                {
                    new_payload = payload.Substring(pointer);
                    ret.Text = new_payload;
                }
                //Return the SyslogMessage...
                return ret;
            }
            catch (FormatException ex)
            {
                ex.Data.Add("Payload", payload);
                throw ex;
            }
            catch (Exception e)
            {
                FormatException ex = new FormatException("Message not in Syslog format", e);
                ex.Data.Add("Payload", payload);
                throw ex;
            }
        }

        /// <summary>
        /// Parses Syslog messages according to RFC5424
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        private static SyslogMessage Parse5424(string payload)
        {
            SyslogMessage ret = new SyslogMessage();
            try
            {
                int pointer = 1;
                String new_payload = payload.Substring(pointer);

                //Calculate prival = Facility*8 + Severity...
                String prival = new_payload.Split('>')[0];
                Int32 severity = 0;
                ret.Facility = (SyslogFacility)Math.DivRem(Int32.Parse(prival), 8, out severity);
                ret.Severity = (SyslogSeverity)severity;
                pointer += prival.Length + 1;

                //Calculate Version...
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
                    for (int j = 1; j < new_payload.Length - 1; j++)
                    {
                        StructuredData += new_payload[j];
                        if (new_payload[j - 1] == '"' && new_payload[j] == ']' && new_payload[j + 1] == ' ')
                            break;
                    }
                    if (!StructuredData.EndsWith("]"))
                        StructuredData += ']';
                    ret.Data = new Dictionary<string, IDictionary<string, string>>();
                    StructuredData = StructuredData.Replace("\\[", "@*°");
                    StructuredData = StructuredData.Replace("\\]", "çà§");
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
                            SubSubElem[0] = SubSubElem[0].Replace("@*°", "\\[");
                            SubSubElem[0] = SubSubElem[0].Replace("çà§", "\\]");
                            SubSubElem[1] = SubSubElem[1].Replace("@*°", "\\[");
                            SubSubElem[1] = SubSubElem[1].Replace("çà§", "\\]");
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
                    //Controls the presence of BOM...
                    byte[] BOM = { 0xef, 0xbb, 0xbf };
                    if (new_payload[0] == BOM[0] && new_payload[1] == BOM[1] && new_payload[2] == BOM[2])
                        ret.Text = new_payload.Substring(3);
                    else
                        ret.Text = new_payload;
                }
                //Return the SyslogMessage...
                return ret;
            }
            catch (FormatException ex)
            {
                ex.Data.Add("Payload", payload);
                throw ex;
            }
            catch (Exception e)
            {
                FormatException ex = new FormatException("Message not in Syslog format", e);
                ex.Data.Add("Payload", payload);
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Argument is null</exception>
        /// <exception cref="FormatException">Message is not in Syslog format</exception>
        public static SyslogMessage Parse(string payload)
        {
            if (string.IsNullOrEmpty(payload)) throw new ArgumentNullException("payload");

            if (payload[0] != '<')
                //RFC5424 messages ALL start with LT, so this is either a No-PRI RFC3164 message or completely invalid message
                return Parse3164(payload);
            //Message begins with LT, so there must be GT
            else if (char.IsDigit(payload[payload.IndexOf('>') + 1]))
                //After GT there is a number. It should be the Version number of RFC5424
                return Parse5424(payload);
            else if (char.IsLetter(payload[payload.IndexOf('>') + 1]))
            {
                //It should be the name of a month. Trying with 3164
                return Parse3164(payload);
            }
            else
                //Definitely bad message
                throw new FormatException("The message is not formatted into any supported Syslog format");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Argument is null</exception>
        /// <exception cref="FormatException">Message is not in Syslog format</exception>
        public static SyslogMessage Parse(byte[] payload)
        {
            return Parse(System.Text.Encoding.UTF8.GetString(payload));
        }

        private static Int32 GetMonthByName(String Month)
        {
            Int32 Mon = 1;
            if (Month == "Jan")
                Mon = 1;
            else if (Month == "Feb")
                Mon = 2;
            else if (Month == "Mar")
                Mon = 3;
            else if (Month == "Apr")
                Mon = 4;
            else if (Month == "May")
                Mon = 5;
            else if (Month == "Jun")
                Mon = 6;
            else if (Month == "Jul")
                Mon = 7;
            else if (Month == "Aug")
                Mon = 8;
            else if (Month == "Sep")
                Mon = 9;
            else if (Month == "Oct")
                Mon = 10;
            else if (Month == "Nov")
                Mon = 11;
            else if (Month == "Dec")
                Mon = 12;

            return Mon;
        }

        #endregion
    }
}