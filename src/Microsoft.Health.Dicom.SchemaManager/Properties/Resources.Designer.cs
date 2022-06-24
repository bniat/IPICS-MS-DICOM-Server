﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.Health.Dicom.SchemaManager.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.Health.Dicom.SchemaManager.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applies the specified SQL schema update to the supplied connection string..
        /// </summary>
        internal static string ApplyCommandDescription {
            get {
                return ResourceManager.GetString("ApplyCommandDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to apply.
        /// </summary>
        internal static string ApplyCommandName {
            get {
                return ResourceManager.GetString("ApplyCommandName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The authentication type to use. Valid values are ManagedIdentity and ConnectionString..
        /// </summary>
        internal static string AuthenticationTypeDescription {
            get {
                return ResourceManager.GetString("AuthenticationTypeDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection string of the SQL server to apply the schema update..
        /// </summary>
        internal static string ConnectionStringOptionDescription {
            get {
                return ResourceManager.GetString("ConnectionStringOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must include a connection string..
        /// </summary>
        internal static string ConnectionStringRequiredValidation {
            get {
                return ResourceManager.GetString("ConnectionStringRequiredValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The schema migration is run without validating the schema version..
        /// </summary>
        internal static string ForceOptionDescription {
            get {
                return ResourceManager.GetString("ForceOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified schema version &quot;{0}&quot; is incompatible..
        /// </summary>
        internal static string IncompatibleVersionFormatString {
            get {
                return ResourceManager.GetString("IncompatibleVersionFormatString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applies versions between the current and latest version..
        /// </summary>
        internal static string LatestOptionDescription {
            get {
                return ResourceManager.GetString("LatestOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The client ID of the managed identity to be used..
        /// </summary>
        internal static string ManagedIdentityClientIdDescription {
            get {
                return ResourceManager.GetString("ManagedIdentityClientIdDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You must include only one option from [version, next, latest]..
        /// </summary>
        internal static string MutuallyExclusiveValidation {
            get {
                return ResourceManager.GetString("MutuallyExclusiveValidation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applies the next available version..
        /// </summary>
        internal static string NextOptionDescription {
            get {
                return ResourceManager.GetString("NextOptionDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Applies versions between the current and specified version..
        /// </summary>
        internal static string VersionOptionDescription {
            get {
                return ResourceManager.GetString("VersionOptionDescription", resourceCulture);
            }
        }
    }
}
