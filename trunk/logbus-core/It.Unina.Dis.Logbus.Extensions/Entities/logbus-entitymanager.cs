﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:2.0.50727.4927
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by wsdl, Version=2.0.50727.1432.
// 
namespace It.Unina.Dis.Logbus.Entities {
    using System.Xml.Serialization;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Diagnostics;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.Web.Services.WebServiceBindingAttribute(Name="EntityManagement", Namespace="http://www.dis.unina.it/logbus-ng/em")]
    public interface IEntityManagement {
        
        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetLoggingEntities", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("entities", Namespace="http://www.dis.unina.it/logbus-ng/em")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("entity", IsNullable=false)]
        LoggingEntity[] GetLoggingEntities();
        
        /// <remarks/>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#FindLoggingEntities", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("entities", Namespace="http://www.dis.unina.it/logbus-ng/em")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("entity", IsNullable=false)]
        LoggingEntity[] FindLoggingEntities([System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.dis.unina.it/logbus-ng/em")] TemplateQuery query);
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TemplateQuery))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/em")]
    public partial class LoggingEntity {
        
        private string hostField;
        
        private string processField;
        
        private string moduleField;
        
        private string loggerField;
        
        private string classField;
        
        private string methodField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string host {
            get {
                return this.hostField;
            }
            set {
                this.hostField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string process {
            get {
                return this.processField;
            }
            set {
                this.processField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string module {
            get {
                return this.moduleField;
            }
            set {
                this.moduleField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string logger {
            get {
                return this.loggerField;
            }
            set {
                this.loggerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string @class {
            get {
                return this.classField;
            }
            set {
                this.classField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string method {
            get {
                return this.methodField;
            }
            set {
                this.methodField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/em")]
    public partial class TemplateQuery : LoggingEntity {
        
        private bool ffdaField;
        
        private bool ffdaFieldSpecified;
        
        private ulong activityTimeoutField;
        
        private bool activityTimeoutFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool ffda {
            get {
                return this.ffdaField;
            }
            set {
                this.ffdaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ffdaSpecified {
            get {
                return this.ffdaFieldSpecified;
            }
            set {
                this.ffdaFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ulong activityTimeout {
            get {
                return this.activityTimeoutField;
            }
            set {
                this.activityTimeoutField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool activityTimeoutSpecified {
            get {
                return this.activityTimeoutFieldSpecified;
            }
            set {
                this.activityTimeoutFieldSpecified = value;
            }
        }
    }
}
