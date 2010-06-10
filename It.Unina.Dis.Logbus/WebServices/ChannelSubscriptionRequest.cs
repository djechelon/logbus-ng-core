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

namespace It.Unina.Dis.Logbus.WebServices
{
    /// <summary>
    /// Holds information about channel subscription
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    public partial class ChannelSubscriptionRequest
    {

        private KeyValuePair paramField;

        private string channelidField;

        private string transportField;

        /// <summary>
        /// Transport-specific configuration parameters
        /// </summary>
        /// <remarks>Defined in transport documentation</remarks>
        public KeyValuePair param
        {
            get
            {
                return this.paramField;
            }
            set
            {
                this.paramField = value;
            }
        }

        /// <summary>
        /// ID of channel to subscribe to
        /// </summary>
        [System.Xml.Serialization.XmlAttributeAttribute("channel-id")]
        public string channelid
        {
            get
            {
                return this.channelidField;
            }
            set
            {
                this.channelidField = value;
            }
        }

        /// <summary>
        /// ID of transport to use
        /// </summary>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string transport
        {
            get
            {
                return this.transportField;
            }
            set
            {
                this.transportField = value;
            }
        }
    }
}
