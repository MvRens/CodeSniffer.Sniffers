﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sniffer.DotNetversion {
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
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sniffer.DotNetversion.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to The names of the .NET versions, as specified in the project files, which should be considered critical..
        /// </summary>
        internal static string HelpCriticalSummary {
            get {
                return ResourceManager.GetString("HelpCriticalSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If True, only projects in Solution files will be considered. Otherwise all project files will be checked. Defaults to False..
        /// </summary>
        internal static string HelpSolutionsOnlySummary {
            get {
                return ResourceManager.GetString("HelpSolutionsOnlySummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Checks the .NET version used by C# projects in the repository..
        /// </summary>
        internal static string HelpSummary {
            get {
                return ResourceManager.GetString("HelpSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If a filter starts and ends with a forward slash (/), it is considered a regular expression. If it contains either an asterisk (*) or question mark (?), these are considered wildcards for respectively matching 0 or more characters (*) or exactly one character (?). Otherwise, the version must match exactly (case-insensitive)..
        /// </summary>
        internal static string HelpWarnDescription {
            get {
                return ResourceManager.GetString("HelpWarnDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The names of the .NET versions, as specified in the project files, which should cause a warning..
        /// </summary>
        internal static string HelpWarnSummary {
            get {
                return ResourceManager.GetString("HelpWarnSummary", resourceCulture);
            }
        }
    }
}
