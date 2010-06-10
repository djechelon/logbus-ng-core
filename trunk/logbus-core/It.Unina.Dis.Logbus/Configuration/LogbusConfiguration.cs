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

using System.Xml.Serialization;
using It.Unina.Dis.Logbus.Filters;

namespace It.Unina.Dis.Logbus.Configuration
{
    /// <summary>
    /// Logbus configuration class
    /// </summary>
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.dis.unina.it/logbus/configuration")]
    [System.Xml.Serialization.XmlRootAttribute("logbus", Namespace = "http://www.dis.unina.it/logbus/configuration", IsNullable = false)]
    public partial class LogbusConfiguration : object, System.ComponentModel.INotifyPropertyChanged
    {

        private InboundChannelDefinition[] inchannelsField;

        private CustomFiltersConfiguration customfiltersField;

        private OutputTransportsConfiguration outtransportsField;

        private FilterBase corefilterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute("in-channels", IsNullable = true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("in-channel", IsNullable = false)]
        public InboundChannelDefinition[] inchannels
        {
            get
            {
                return this.inchannelsField;
            }
            set
            {
                this.inchannelsField = value;
                this.RaisePropertyChanged("inchannels");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("custom-filters")]
        public CustomFiltersConfiguration customfilters
        {
            get
            {
                return this.customfiltersField;
            }
            set
            {
                this.customfiltersField = value;
                this.RaisePropertyChanged("customfilters");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("out-transports")]
        public OutputTransportsConfiguration outtransports
        {
            get
            {
                return this.outtransportsField;
            }
            set
            {
                this.outtransportsField = value;
                this.RaisePropertyChanged("outtransports");
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("core-filter", IsNullable = true)]
        public FilterBase corefilter
        {
            get
            {
                return this.corefilterField;
            }
            set
            {
                this.corefilterField = value;
                this.RaisePropertyChanged("corefilter");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
