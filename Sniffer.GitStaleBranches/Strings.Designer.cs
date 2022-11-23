﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sniffer.GitStaleBranches {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Sniffer.GitStaleBranches.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Determines which branches are considered final merge targets, typically &quot;master&quot;, &quot;main&quot; and/or &quot;develop&quot;. Branches which are merged into one of these branches are considered finished and should be removed unless further commits are being pushed.
        ///
        ///Note that only commits in one of these branches will trigger a check for stale branches..
        /// </summary>
        internal static string HelpMergeBranches {
            get {
                return ResourceManager.GetString("HelpMergeBranches", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Branches which haven&apos;t had any commits in this many days are considered stale and should be updated or closed. The branches in the &quot;MergeBranches&quot; set are ignored for this check. Leave empty or set to 0 to disable this check..
        /// </summary>
        internal static string HelpStaleAfterDays {
            get {
                return ResourceManager.GetString("HelpStaleAfterDays", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Checks for remote branches which should have been deleted, for example, because the Git Flow has been finished..
        /// </summary>
        internal static string HelpSummary {
            get {
                return ResourceManager.GetString("HelpSummary", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Branch has been merged into {0}.
        /// </summary>
        internal static string ResultMerged {
            get {
                return ResourceManager.GetString("ResultMerged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to On {0} by {1} &lt;{2}&gt; in commit {3}..
        /// </summary>
        internal static string ResultMergedOutput {
            get {
                return ResourceManager.GetString("ResultMergedOutput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Branch had last commit {0} days ago.
        /// </summary>
        internal static string ResultStale {
            get {
                return ResourceManager.GetString("ResultStale", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to On {0} by {1} &lt;{2}&gt; in commit {3}..
        /// </summary>
        internal static string ResultStaleOutput {
            get {
                return ResourceManager.GetString("ResultStaleOutput", resourceCulture);
            }
        }
    }
}
