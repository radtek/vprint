﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VPrinting.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://192.168.53.117/PtfWebService/Authentication.asmx")]
        public string VoucherAllocationPrinting_Authentication_Authentication {
            get {
                return ((string)(this["VoucherAllocationPrinting_Authentication_Authentication"]));
            }
            set {
                this["VoucherAllocationPrinting_Authentication_Authentication"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://192.168.53.117/ptfwebservice/PartyManagement.asmx")]
        public string VoucherAllocationPrinting_PartyManagement_PartyManagement {
            get {
                return ((string)(this["VoucherAllocationPrinting_PartyManagement_PartyManagement"]));
            }
            set {
                this["VoucherAllocationPrinting_PartyManagement_PartyManagement"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://192.168.53.117/ptfwebservice/VoucherNumberingAllocationPrinting.asmx")]
        public string VoucherAllocationPrinting_VoucherNumberingAllocationPrinting_VoucherNumberingAllocationPrinting {
            get {
                return ((string)(this["VoucherAllocationPrinting_VoucherNumberingAllocationPrinting_VoucherNumberingAllo" +
                    "cationPrinting"]));
            }
            set {
                this["VoucherAllocationPrinting_VoucherNumberingAllocationPrinting_VoucherNumberingAllo" +
                    "cationPrinting"] = value;
            }
        }
    }
}
