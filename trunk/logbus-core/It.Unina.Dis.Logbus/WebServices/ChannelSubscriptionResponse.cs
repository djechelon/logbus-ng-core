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
    /// Response to channel subscription. Contains client ID and client instructions
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    public partial class ChannelSubscriptionResponse
    {

        private KeyValuePair paramField;

        private string clientidField;

        /// <summary>
        /// Client-specific configuration parameters.
        /// Defined by transport documentation
        /// </summary>
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
        /// Client ID for future requests
        /// </summary>
        [System.Xml.Serialization.XmlAttributeAttribute("client-id")]
        public string clientid
        {
            get
            {
                return this.clientidField;
            }
            set
            {
                this.clientidField = value;
            }
        }
    }
}
