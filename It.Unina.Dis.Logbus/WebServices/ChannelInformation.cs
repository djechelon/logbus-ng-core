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

using It.Unina.Dis.Logbus.Filters;

namespace It.Unina.Dis.Logbus.WebServices
{
    /// <summary>
    /// Hold detailed information about channels
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/wsdl")]
    public partial class ChannelInformation
    {

        private string idField;

        private string titleField;

        private string descriptionField;

        private long coalescenceWindowField;

        private string clientsField;

        private FilterBase filterField;

        /// <summary>
        /// Unique ID of channel
        /// </summary>
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <summary>
        /// Human-readable descriptive name
        /// </summary>
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <summary>
        /// Human readable long description
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <summary>
        /// Coalescence window in milliseconds
        /// </summary>
        public long coalescenceWindow
        {
            get
            {
                return this.coalescenceWindowField;
            }
            set
            {
                this.coalescenceWindowField = value;
            }
        }

        /// <summary>
        /// Number of clients subscribed to the channel
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
        public string clients
        {
            get
            {
                return this.clientsField;
            }
            set
            {
                this.clientsField = value;
            }
        }

        /// <summary>
        /// Channel filter
        /// </summary>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://www.dis.unina.it/logbus-ng/filters")]
        public FilterBase filter
        {
            get
            {
                return this.filterField;
            }
            set
            {
                this.filterField = value;
            }
        }
    }
}
