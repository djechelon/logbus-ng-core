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

namespace It.Unina.Dis.Logbus.FieldFailureData
{
    /// <summary>
    /// FFDA-information related to Syslog messages
    /// </summary>
    public class FieldFailureDataInformation
    {
        /// <summary>
        /// Initializes a new instance of FFDAInformation
        /// </summary>
        public FieldFailureDataInformation()
        {
        }

        /// <summary>
        /// Initializes a new instance of FFDAInformation
        /// </summary>
        /// <param name="message">Syslog message to analyze</param>
        /// <exception cref="System.InvalidOperationException">Message is not FFDA</exception>
        public FieldFailureDataInformation(SyslogMessage message)
        {
            try
            {
                if (message.MessageId == "FFDA" && message.Severity == SyslogSeverity.Info)
                {
                    string txt = message.Text;
                    if (txt == null) throw new InvalidOperationException("Message is not FFD");

                    SyslogAttributes advancedAttrs = message.GetAdvancedAttributes();

                    Host = message.Host;
                    Process = message.ProcessID ?? message.ApplicationName;
                    Logger = advancedAttrs.LogName;


                    string prefix = txt.Substring(0, 3);
                    if (txt.Contains("-") && txt.Length > 4)
                    {
                        FlowId = txt.Substring(4);
                    }

                    Event = (FieldFailureEvent) Enum.Parse(typeof (FieldFailureEvent), prefix.ToUpper());
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Message is not FFD", ex);
            }
        }

        /// <summary>
        /// Host that generated the message
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Process ID (or name, if ID not specified) that generated the message
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// Logger that generated the message
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// Identification of control flow
        /// </summary>
        public string FlowId { get; set; }

        /// <summary>
        /// Event type
        /// </summary>
        public FieldFailureEvent Event { get; set; }
    }
}