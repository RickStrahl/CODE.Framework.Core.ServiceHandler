﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CODE.Framework.Core.ServiceHandler.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CODE.Framework.Core.ServiceHandler.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to The hosted service contract must implement a service interface..
        /// </summary>
        internal static string HostedServiceRequiresAnInterface {
            get {
                return ResourceManager.GetString("HostedServiceRequiresAnInterface", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid Service type: {0}.
        /// </summary>
        internal static string InvalidServiceType {
            get {
                return ResourceManager.GetString("InvalidServiceType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only a single object parameter can be passed to a service method &quot;{0}&quot;.
        /// </summary>
        internal static string OnlySingleParametersAreAllowedOnServiceMethods {
            get {
                return ResourceManager.GetString("OnlySingleParametersAreAllowedOnServiceMethods", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service method {0} doesn&apos;t exist or can&apos;t be called with Http Verb {1}. Method not invoked..
        /// </summary>
        internal static string ServiceMethodDoesntExist {
            get {
                return ResourceManager.GetString("ServiceMethodDoesntExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Service must be called over a secure connection (HTTPS).
        /// </summary>
        internal static string ServiceMustBeAccessedOverHttps {
            get {
                return ResourceManager.GetString("ServiceMustBeAccessedOverHttps", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Couldn&apos;t create type instance: {0}.
        /// </summary>
        internal static string UnableToCreateTypeInstance {
            get {
                return ResourceManager.GetString("UnableToCreateTypeInstance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to execute method {0}: {1}.
        /// </summary>
        internal static string UnableToExecuteMethod {
            get {
                return ResourceManager.GetString("UnableToExecuteMethod", resourceCulture);
            }
        }
    }
}
