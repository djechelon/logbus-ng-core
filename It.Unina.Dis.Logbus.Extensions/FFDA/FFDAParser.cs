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
        public FFDAParser() { }

        /// <summary>
        /// Initializes FFDASource with an existing log source
        /// </summary>
        /// <param name="source">Active log source to monitor</param>
        public FFDAParser(ILogSource source)
        {
            if (source == null) throw new ArgumentNullException("source", "Source must be an existing and active log source");

            _theSource = source;
            _theSource.MessageReceived += new EventHandler<SyslogMessageEventArgs>(the_source_MessageReceived);
        }

        #endregion

        #region Events

        /// <summary>
        /// This event indicates that the parser successfully parsed an FFDA-related message, whether its content is
        /// </summary>
        public event EventHandler<FFDAEventArgs> GotFFDA;

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
                if (msg.MessageId == "FFDA")
                {
                    string txt = msg.Text;
                    if (txt == null) return;

                    SyslogAttributes advancedAttrs = msg.GetAdvancedAttributes();
                    FFDAEventArgs e = new FFDAEventArgs
                                          {
                                              Host = msg.Host,
                                              Process = msg.ProcessID ?? msg.ApplicationName,
                                              LoggerName = advancedAttrs.LogName
                                          };

                    string prefix = txt.Substring(0, 3);
                    if (txt[3] == '-' && txt.Length > 4)
                    {
                        e.FlowId = txt.Substring(4);
                    }

                    e.EventType = (FFDAEvent)Enum.Parse(typeof(FFDAEvent), prefix.ToUpper());

                    if (GotFFDA != null) GotFFDA(this, e);

                    switch (e.EventType)
                    {
                        case FFDAEvent.SUP: { if (GotSUP != null) GotSUP(this, e); break; }
                        case FFDAEvent.SDW: { if (GotSDW != null) GotSDW(this, e); break; }
                        case FFDAEvent.SST: { if (GotSST != null) GotSST(this, e); break; }
                        case FFDAEvent.SEN: { if (GotSEN != null) GotSEN(this, e); break; }
                        case FFDAEvent.RIS: { if (GotRIS != null) GotRIS(this, e); break; }
                        case FFDAEvent.RIE: { if (GotRIE != null) GotRIE(this, e); break; }
                        case FFDAEvent.EIS: { if (GotEIS != null) GotEIS(this, e); break; }
                        case FFDAEvent.EIE: { if (GotEIE != null) GotEIE(this, e); break; }
                        case FFDAEvent.CMP: { if (GotCMP != null) GotCMP(this, e); break; }
                    }
                }
            }
            catch (FormatException) { } //Someone is fooling us with an unknown FFDA prefix, or it's just a newer specification
            catch (IndexOutOfRangeException) { } //skip!
            catch { }

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
