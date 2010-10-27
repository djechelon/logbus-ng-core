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
namespace It.Unina.Dis.Logbus.FFDA
{
    /// <summary>
    /// This class provides easy parsing of FFD messages from an existing Syslog source, or from injected messages
    /// </summary>
    public class FFDAParser
        : ILogCollector
    {

        private readonly ILogSource _theSource;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of FFDAParser
        /// </summary>
        public FFDAParser() { }

        /// <summary>
        /// Initializes FFDASource with an existing log source
        /// </summary>
        /// <param name="source">Active log source to monitor</param>
        public FFDAParser(ILogSource source)
        {
            if (source == null) throw new ArgumentNullException("source", "Source must be an existing and active log source");

            _theSource = source;
            _theSource.MessageReceived += the_source_MessageReceived;
        }

        #endregion

        #region Events

        /// <summary>
        /// This event indicates that the parser successfully parsed an FFDA-related message, whether its content is
        /// </summary>
        public event EventHandler<FFDAEventArgs> GotFFDA;

        /// <summary>
        /// Got heartbeat message
        /// </summary>
        public event EventHandler<SyslogMessageEventArgs> GotHeartbeat;

        /// <summary>
        /// FFDA-specific events
        /// </summary>
        public event EventHandler<FFDAEventArgs> GotSST, GotSEN, GotEIS, GotEIE, GotRIS, GotRIE, GotSUP, GotSDW, GotCMP;

        #endregion

        private void the_source_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            Parse(e.Message);
        }

        private void Parse(SyslogMessage msg)
        {
            try
            {
                if (msg.MessageId == "HEARTBEAT" && msg.Severity == SyslogSeverity.Debug && GotHeartbeat != null)
                    GotHeartbeat(this, new SyslogMessageEventArgs(msg));

                FFDAInformation info = new FFDAInformation(msg);

                FFDAEventArgs e = new FFDAEventArgs
                                      {
                                          Host = info.Host,
                                          EventType = info.Event,
                                          FlowId = info.FlowId,
                                          LoggerName = info.Logger,
                                          Process = info.Process,
                                          Message = msg
                                      };

                if (GotFFDA != null) GotFFDA(this, e);

                switch (info.Event)
                {
                    case FFDAEvent.SUP:
                        {
                            if (GotSUP != null) GotSUP(this, e); break;
                        }
                    case FFDAEvent.SDW:
                        {
                            if (GotSDW != null) GotSDW(this, e); break;
                        }
                    case FFDAEvent.SST:
                        {
                            if (GotSST != null) GotSST(this, e); break;
                        }
                    case FFDAEvent.SEN:
                        {
                            if (GotSEN != null) GotSEN(this, e); break;
                        }
                    case FFDAEvent.EIS:
                        {
                            if (GotEIS != null) GotEIS(this, e); break;
                        }
                    case FFDAEvent.EIE:
                        {
                            if (GotEIE != null) GotEIE(this, e); break;
                        }
                    case FFDAEvent.RIS:
                        {
                            if (GotRIS != null) GotRIS(this, e); break;
                        }
                    case FFDAEvent.RIE:
                        {
                            if (GotRIE != null) GotRIE(this, e); break;
                        }
                    case FFDAEvent.CMP:
                        {
                            if (GotCMP != null) GotCMP(this, e); break;
                        }
                }
            }
            catch (InvalidOperationException) { }

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
