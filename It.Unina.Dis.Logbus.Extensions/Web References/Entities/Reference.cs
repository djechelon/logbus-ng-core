﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:2.0.50727.4952
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// Il codice sorgente è stato generato automaticamente da Microsoft.VSDesigner, versione 2.0.50727.4952.
// 
#pragma warning disable 1591

namespace It.Unina.Dis.Logbus.Entities {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="EntityManagement", Namespace="http://www.dis.unina.it/logbus-ng/em")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(LoggingEntityIdentifier))]
    public partial class EntityManagement : System.Web.Services.Protocols.SoapHttpClientProtocol, IEntityManagement {
        
        private System.Threading.SendOrPostCallback GetLoggingEntitiesOperationCompleted;
        
        private System.Threading.SendOrPostCallback FindLoggingEntitiesOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public EntityManagement() {
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetLoggingEntitiesCompletedEventHandler GetLoggingEntitiesCompleted;
        
        /// <remarks/>
        public event FindLoggingEntitiesCompletedEventHandler FindLoggingEntitiesCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#GetLoggingEntities", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("entities", Namespace="http://www.dis.unina.it/logbus-ng/em")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("entity", IsNullable=false)]
        public LoggingEntity[] GetLoggingEntities() {
            object[] results = this.Invoke("GetLoggingEntities", new object[0]);
            return ((LoggingEntity[])(results[0]));
        }
        
        /// <remarks/>
        public void GetLoggingEntitiesAsync() {
            this.GetLoggingEntitiesAsync(null);
        }
        
        /// <remarks/>
        public void GetLoggingEntitiesAsync(object userState) {
            if ((this.GetLoggingEntitiesOperationCompleted == null)) {
                this.GetLoggingEntitiesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetLoggingEntitiesOperationCompleted);
            }
            this.InvokeAsync("GetLoggingEntities", new object[0], this.GetLoggingEntitiesOperationCompleted, userState);
        }
        
        private void OnGetLoggingEntitiesOperationCompleted(object arg) {
            if ((this.GetLoggingEntitiesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetLoggingEntitiesCompleted(this, new GetLoggingEntitiesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:#FindLoggingEntities", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlArrayAttribute("entities", Namespace="http://www.dis.unina.it/logbus-ng/em")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("entity", IsNullable=false)]
        public LoggingEntity[] FindLoggingEntities([System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.dis.unina.it/logbus-ng/em")] TemplateQuery query) {
            object[] results = this.Invoke("FindLoggingEntities", new object[] {
                        query});
            return ((LoggingEntity[])(results[0]));
        }
        
        /// <remarks/>
        public void FindLoggingEntitiesAsync(TemplateQuery query) {
            this.FindLoggingEntitiesAsync(query, null);
        }
        
        /// <remarks/>
        public void FindLoggingEntitiesAsync(TemplateQuery query, object userState) {
            if ((this.FindLoggingEntitiesOperationCompleted == null)) {
                this.FindLoggingEntitiesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnFindLoggingEntitiesOperationCompleted);
            }
            this.InvokeAsync("FindLoggingEntities", new object[] {
                        query}, this.FindLoggingEntitiesOperationCompleted, userState);
        }
        
        private void OnFindLoggingEntitiesOperationCompleted(object arg) {
            if ((this.FindLoggingEntitiesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.FindLoggingEntitiesCompleted(this, new FindLoggingEntitiesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/em")]
    public partial class LoggingEntity : LoggingEntityIdentifier {
        
        private bool ffdaField;
        
        private System.DateTime lastActionField;
        
        private System.DateTime lastHeartbeatField;
        
        public LoggingEntity() {
            this.ffdaField = false;
        }
        
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
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime lastAction {
            get {
                return this.lastActionField;
            }
            set {
                this.lastActionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public System.DateTime lastHeartbeat {
            get {
                return this.lastHeartbeatField;
            }
            set {
                this.lastHeartbeatField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(LoggingEntity))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TemplateQuery))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/em")]
    public partial class LoggingEntityIdentifier {
        
        private string hostField;
        
        private string processField;
        
        private string loggerField;
        
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
        public string logger {
            get {
                return this.loggerField;
            }
            set {
                this.loggerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.4927")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/em")]
    public partial class TemplateQuery : LoggingEntityIdentifier {
        
        private bool ffdaField;
        
        private bool ffdaFieldSpecified;
        
        private string maxinactivityField;
        
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
        [System.Xml.Serialization.XmlElementAttribute("max-inactivity", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, DataType="integer")]
        public string maxinactivity {
            get {
                return this.maxinactivityField;
            }
            set {
                this.maxinactivityField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void GetLoggingEntitiesCompletedEventHandler(object sender, GetLoggingEntitiesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetLoggingEntitiesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetLoggingEntitiesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public LoggingEntity[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((LoggingEntity[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void FindLoggingEntitiesCompletedEventHandler(object sender, FindLoggingEntitiesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class FindLoggingEntitiesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal FindLoggingEntitiesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public LoggingEntity[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((LoggingEntity[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591