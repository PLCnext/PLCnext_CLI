﻿#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace PlcNext.Common.Tools.SDK {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.phoenixcontact.com/schema/sdkmanifest")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.phoenixcontact.com/schema/sdkmanifest", IsNullable=false)]
    public partial class SdkManifest {
        
        private TargetDefinition[] targetsField;
        
        private MakeExecutableDefinition makeExecutableField;
        
        private string schemaVersionField;
        
        public SdkManifest() {
            this.schemaVersionField = "1.0";
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Target", IsNullable=false)]
        public TargetDefinition[] Targets {
            get {
                return this.targetsField;
            }
            set {
                this.targetsField = value;
            }
        }
        
        /// <remarks/>
        public MakeExecutableDefinition MakeExecutable {
            get {
                return this.makeExecutableField;
            }
            set {
                this.makeExecutableField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schemaVersion {
            get {
                return this.schemaVersionField;
            }
            set {
                this.schemaVersionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.phoenixcontact.com/schema/sdkmanifest")]
    public partial class TargetDefinition {
        
        private string nameField;
        
        private string versionField;
        
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
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.phoenixcontact.com/schema/sdkmanifest")]
    public partial class MakeExecutableDefinition {
        
        private string pathField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
            }
        }
    }
}
