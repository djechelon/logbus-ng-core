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
    /// This class provides easy parsing of FFD messages from an existing Syslog source
    /// </summary>
    public class FFDASource
    {

        private ILogSource the_source;

        #region Constructor

        /// <summary>
        /// Initializes FFDASource with an existing log source
        /// </summary>
        /// <param name="source">Active log source to monitor</param>
        public FFDASource(ILogSource source)
        {
            if (source == null) throw new ArgumentNullException("source", "Source must be an existing and active log source");

            the_source = source;
            the_source.MessageReceived += new EventHandler<SyslogMessageEventArgs>(the_source_MessageReceived);
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
        public event EventHandler<FFDAEventArgs> GotSST, GotSEN;

        #endregion

        private void the_source_MessageReceived(object sender, SyslogMessageEventArgs e)
        {
            if (e.Message.MessageId == "FFDA")
            {
                string txt = e.Message.Text;
                if (txt == null) return;

                if (txt.StartsWith("SST")) ;//SST
                else if (txt.StartsWith("SEN")) ;//SEN
                else if (txt.StartsWith("EIS")) ;//EIS
                else if (txt.StartsWith("EIE")) ;//EIE
                else if (txt.StartsWith("RIS")) ;//RIS
                else if (txt.StartsWith("RIE")) ;//RIE
                else if (txt.StartsWith("CMP")) ;//Complaint
            }
            throw new NotImplementedException();
        }
    }
}
