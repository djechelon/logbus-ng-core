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
    /// This class provides easy parsing of FFD messages from an existing Syslog source, or from injected messages
    /// </summary>
    public class FieldFailureDataParser
        : ILogCollector
    {
        private readonly ILogSource _theSource;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of FFDAParser
        /// </summary>
        public FieldFailureDataParser()
        {
        }

        /// <summary>
        /// Initializes FFDASource with an existing log source
        /// </summary>
        /// <param name="source">Active log source to monitor</param>
        public FieldFailureDataParser(ILogSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source", "Source must be an existing and active log source");

            _theSource = source;
            _theSource.MessageReceived += MessageReceivedHandler;
        }

        #endregion

        #region Events

        /// <summary>
        /// This event indicates that the parser successfully parsed an FFDA-related message, whether its content is
        /// </summary>
        public event EventHandler<FieldFailureEventArgs> GotFFD;

        /// <summary>
        /// Got heartbeat message
        /// </summary>
        public event EventHandler<SyslogMessageEventArgs> GotHeartbeat;

        /// <summary>
        /// FFDA-specific events
        /// </summary>
        public event EventHandler<FieldFailureEventArgs> 
            GotSST ,
            GotSEN ,
            GotEIS ,
            GotEIE ,
            GotRIS ,
            GotRIE ,
            GotSUP ,
            GotSDW ,
            GotCMP;

        #endregion

        private void MessageReceivedHandler(object sender, SyslogMessageEventArgs e)
        {
            Parse(e.Message);
        }

        private void Parse(SyslogMessage msg)
        {
            try
            {
                if (msg.MessageId == "HEARTBEAT" && msg.Severity == SyslogSeverity.Debug)
                {
                    if (GotHeartbeat != null)
                        GotHeartbeat(this, new SyslogMessageEventArgs(msg));
                    return;
                }
				
				if(msg.MessageId != "FFDA")
					return;
				
                FieldFailureDataInformation info = new FieldFailureDataInformation(msg);

                FieldFailureEventArgs e = new FieldFailureEventArgs
                                      {
                                          Host = info.Host,
                                          EventType = info.Event,
                                          FlowId = info.FlowId,
                                          LoggerName = info.Logger,
                                          Process = info.Process,
                                          Message = msg
                                      };

                if (GotFFD != null) GotFFD(this, e);

                switch (info.Event)
                {
                    case FieldFailureEvent.SUP:
                        {
                            if (GotSUP != null) GotSUP(this, e);
                            break;
                        }
                    case FieldFailureEvent.SDW:
                        {
                            if (GotSDW != null) GotSDW(this, e);
                            break;
                        }
                    case FieldFailureEvent.SST:
                        {
                            if (GotSST != null) GotSST(this, e);
                            break;
                        }
                    case FieldFailureEvent.SEN:
                        {
                            if (GotSEN != null) GotSEN(this, e);
                            break;
                        }
                    case FieldFailureEvent.EIS:
                        {
                            if (GotEIS != null) GotEIS(this, e);
                            break;
                        }
                    case FieldFailureEvent.EIE:
                        {
                            if (GotEIE != null) GotEIE(this, e);
                            break;
                        }
                    case FieldFailureEvent.RIS:
                        {
                            if (GotRIS != null) GotRIS(this, e);
                            break;
                        }
                    case FieldFailureEvent.RIE:
                        {
                            if (GotRIE != null) GotRIE(this, e);
                            break;
                        }
                    case FieldFailureEvent.CMP:
                        {
                            if (GotCMP != null) GotCMP(this, e);
                            break;
                        }
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        #region ILogCollector Membri di

        /// <summary>
        /// Implements ILogCollector.SubmitMessage
        /// </summary>
        public void SubmitMessage(SyslogMessage message)
        {
            Parse(message);
        }

        #endregion
    }
}