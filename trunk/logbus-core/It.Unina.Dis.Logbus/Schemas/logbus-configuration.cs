﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Il codice è stato generato da uno strumento.
//     Versione runtime:2.0.50727.4927
//
//     Le modifiche apportate a questo file possono provocare un comportamento non corretto e andranno perse se
//     il codice viene rigenerato.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/configuration")]
[System.Xml.Serialization.XmlRootAttribute("logbus-core", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=false)]
public partial class LogbusCoreConfiguration {
    
    private InboundChannelDefinition[] inchannelsField;
    
    private CustomFiltersConfiguration customfiltersField;
    
    private OutputTransportsConfiguration outtransportsField;
    
    private FilterBase corefilterField;
    
    private string outChannelFactoryTypeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayAttribute("in-channels", IsNullable=true)]
    [System.Xml.Serialization.XmlArrayItemAttribute("in-channel", IsNullable=false)]
    public InboundChannelDefinition[] inchannels {
        get {
            return this.inchannelsField;
        }
        set {
            this.inchannelsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("custom-filters")]
    public CustomFiltersConfiguration customfilters {
        get {
            return this.customfiltersField;
        }
        set {
            this.customfiltersField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("out-transports")]
    public OutputTransportsConfiguration outtransports {
        get {
            return this.outtransportsField;
        }
        set {
            this.outtransportsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("core-filter", IsNullable=true)]
    public FilterBase corefilter {
        get {
            return this.corefilterField;
        }
        set {
            this.corefilterField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string outChannelFactoryType {
        get {
            return this.outChannelFactoryTypeField;
        }
        set {
            this.outChannelFactoryTypeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/configuration")]
[System.Xml.Serialization.XmlRootAttribute("in-channel", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=false)]
public partial class InboundChannelDefinition {
    
    private KeyValuePair[] paramField;
    
    private string typeField;
    
    private string nameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("param")]
    public KeyValuePair[] param {
        get {
            return this.paramField;
        }
        set {
            this.paramField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/configuration")]
[System.Xml.Serialization.XmlRootAttribute("param", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=false)]
public partial class KeyValuePair {
    
    private string nameField;
    
    private string valueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("parameter", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=true)]
public partial class FilterParameter {
    
    private object valueField;
    
    private string nameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
    public object value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/configuration")]
[System.Xml.Serialization.XmlRootAttribute("custom-filters", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=false)]
public partial class CustomFiltersConfiguration {
    
    private CustomFilterDefinition[] customfilterField;
    
    private AssemblyToScan[] scanassemblyField;
    
    private string factoryField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("custom-filter")]
    public CustomFilterDefinition[] customfilter {
        get {
            return this.customfilterField;
        }
        set {
            this.customfilterField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("scan-assembly")]
    public AssemblyToScan[] scanassembly {
        get {
            return this.scanassemblyField;
        }
        set {
            this.scanassemblyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string factory {
        get {
            return this.factoryField;
        }
        set {
            this.factoryField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/configuration")]
[System.Xml.Serialization.XmlRootAttribute("custom-filter", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=false)]
public partial class CustomFilterDefinition {
    
    private string nameField;
    
    private string descriptionField;
    
    private string typeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string description {
        get {
            return this.descriptionField;
        }
        set {
            this.descriptionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/configuration")]
[System.Xml.Serialization.XmlRootAttribute("scan-assembly", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=false)]
public partial class AssemblyToScan {
    
    private string assemblyField;
    
    private string codebaseField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string assembly {
        get {
            return this.assemblyField;
        }
        set {
            this.assemblyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string codebase {
        get {
            return this.codebaseField;
        }
        set {
            this.codebaseField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/configuration")]
[System.Xml.Serialization.XmlRootAttribute("out-transports", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=false)]
public partial class OutputTransportsConfiguration {
    
    private AssemblyToScan[] scanassemblyField;
    
    private OutputTransportDefinition[] outtransportField;
    
    private string factoryField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("scan-assembly")]
    public AssemblyToScan[] scanassembly {
        get {
            return this.scanassemblyField;
        }
        set {
            this.scanassemblyField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("out-transport")]
    public OutputTransportDefinition[] outtransport {
        get {
            return this.outtransportField;
        }
        set {
            this.outtransportField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string factory {
        get {
            return this.factoryField;
        }
        set {
            this.factoryField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/configuration")]
[System.Xml.Serialization.XmlRootAttribute("out-transport", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=false)]
public partial class OutputTransportDefinition {
    
    private string tagField;
    
    private string factoryField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string tag {
        get {
            return this.tagField;
        }
        set {
            this.tagField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string factory {
        get {
            return this.factoryField;
        }
        set {
            this.factoryField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlIncludeAttribute(typeof(CustomFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(PropertyFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(FacilityEqualsFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(SeverityFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(MessageRegexMatchFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(MessageRegexNotMatchFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(FalseFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(TrueFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(NotFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(OrFilter))]
[System.Xml.Serialization.XmlIncludeAttribute(typeof(AndFilter))]
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("core-filter", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=true)]
public abstract partial class FilterBase {
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("Custom", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class CustomFilter : FilterBase {
    
    private FilterParameter[] parameterField;
    
    private string nameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("parameter", IsNullable=true)]
    public FilterParameter[] parameter {
        get {
            return this.parameterField;
        }
        set {
            this.parameterField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("Property", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class PropertyFilter : FilterBase {
    
    private string valueField;
    
    private Property propertyNameField;
    
    private ComparisonOperator comparisonField;
    
    /// <remarks/>
    public string value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public Property propertyName {
        get {
            return this.propertyNameField;
        }
        set {
            this.propertyNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public ComparisonOperator comparison {
        get {
            return this.comparisonField;
        }
        set {
            this.comparisonField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
public enum Property {
    
    /// <remarks/>
    Timestamp,
    
    /// <remarks/>
    Severity,
    
    /// <remarks/>
    Facility,
    
    /// <remarks/>
    Host,
    
    /// <remarks/>
    ApplicationName,
    
    /// <remarks/>
    ProcessID,
    
    /// <remarks/>
    MessageID,
    
    /// <remarks/>
    Data,
    
    /// <remarks/>
    Text,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
public enum ComparisonOperator {
    
    /// <remarks/>
    eq,
    
    /// <remarks/>
    neq,
    
    /// <remarks/>
    geq,
    
    /// <remarks/>
    gt,
    
    /// <remarks/>
    lt,
    
    /// <remarks/>
    leq,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("FacilityEquals", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class FacilityEqualsFilter : FilterBase {
    
    private FacilityEqualsFilterFacility facilityField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public FacilityEqualsFilterFacility facility {
        get {
            return this.facilityField;
        }
        set {
            this.facilityField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
public enum FacilityEqualsFilterFacility {
    
    /// <remarks/>
    Kernel,
    
    /// <remarks/>
    User,
    
    /// <remarks/>
    Mail,
    
    /// <remarks/>
    System,
    
    /// <remarks/>
    Security,
    
    /// <remarks/>
    Internally,
    
    /// <remarks/>
    Printer,
    
    /// <remarks/>
    News,
    
    /// <remarks/>
    UUCP,
    
    /// <remarks/>
    Cron,
    
    /// <remarks/>
    Security2,
    
    /// <remarks/>
    FTP,
    
    /// <remarks/>
    NTP,
    
    /// <remarks/>
    Audit,
    
    /// <remarks/>
    Alert,
    
    /// <remarks/>
    Clock2,
    
    /// <remarks/>
    Local0,
    
    /// <remarks/>
    Local1,
    
    /// <remarks/>
    Local2,
    
    /// <remarks/>
    Local3,
    
    /// <remarks/>
    Local4,
    
    /// <remarks/>
    Local5,
    
    /// <remarks/>
    Local6,
    
    /// <remarks/>
    Local7,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("Severity", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class SeverityFilter : FilterBase {
    
    private ComparisonOperator comparisonField;
    
    private Severity severityField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public ComparisonOperator comparison {
        get {
            return this.comparisonField;
        }
        set {
            this.comparisonField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public Severity severity {
        get {
            return this.severityField;
        }
        set {
            this.severityField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
public enum Severity {
    
    /// <remarks/>
    Emergency,
    
    /// <remarks/>
    Alert,
    
    /// <remarks/>
    Critical,
    
    /// <remarks/>
    Error,
    
    /// <remarks/>
    Warning,
    
    /// <remarks/>
    Notice,
    
    /// <remarks/>
    Info,
    
    /// <remarks/>
    Debug,
}

/// <remarks/>
[System.Xml.Serialization.XmlIncludeAttribute(typeof(MessageRegexNotMatchFilter))]
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("MessageRegexMatch", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class MessageRegexMatchFilter : FilterBase {
    
    private string patternField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(Form=System.Xml.Schema.XmlSchemaForm.Qualified)]
    public string pattern {
        get {
            return this.patternField;
        }
        set {
            this.patternField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("MessageRegexNotMatch", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class MessageRegexNotMatchFilter : MessageRegexMatchFilter {
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("False", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class FalseFilter : FilterBase {
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("True", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class TrueFilter : FilterBase {
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("Not", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class NotFilter : FilterBase {
    
    private FilterBase filterField;
    
    /// <remarks/>
    public FilterBase filter {
        get {
            return this.filterField;
        }
        set {
            this.filterField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("Or", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class OrFilter : FilterBase {
    
    private FilterBase[] filterField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("filter")]
    public FilterBase[] filter {
        get {
            return this.filterField;
        }
        set {
            this.filterField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/filters")]
[System.Xml.Serialization.XmlRootAttribute("And", Namespace="http://www.dis.unina.it/logbus-ng/filters", IsNullable=false)]
public partial class AndFilter : FilterBase {
    
    private FilterBase[] filterField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("filter")]
    public FilterBase[] filter {
        get {
            return this.filterField;
        }
        set {
            this.filterField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.dis.unina.it/logbus-ng/configuration")]
[System.Xml.Serialization.XmlRootAttribute("in-channels", Namespace="http://www.dis.unina.it/logbus-ng/configuration", IsNullable=true)]
public partial class InboundChannelsConfiguration {
    
    private InboundChannelDefinition[] inchannelField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("in-channel")]
    public InboundChannelDefinition[] inchannel {
        get {
            return this.inchannelField;
        }
        set {
            this.inchannelField = value;
        }
    }
}
