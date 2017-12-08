using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CODE.Framework.Core.ServiceHandler
{
    public class ServiceHandlerConfiguration
    {
        /// <summary>
        /// Configuration Singleton you can use in lieu of Dependency Injection
        /// of IOptions injection
        /// </summary>
        public static ServiceHandlerConfiguration Current { get; set; }

        /// <summary>
        /// Holds a list of services that are configured to be handled.
        /// </summary>
        public List<ServiceHandlerConfigurationInstance> Services { get; set; } = new List<ServiceHandlerConfigurationInstance>();
        
    }


    public class ServiceHandlerConfigurationInstance
    {
        /// <summary>
        /// You can specify a specific type to bind rather than
        /// providing 
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// If you can't provide a type instance you can provide
        /// the type as string and get it dynamically loaded.
        /// Use fully qualified typename (namespace.typename)        
        /// You have to also specify the AssemblyName
        /// </summary>
        public string ServiceTypeName { get; set; }
        

        /// <summary>
        /// If specifying a type name you also have to specify the
        /// name of the assembly to load. Specify only the type.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// The base route to access this service instance
        /// Example: /api/users
        /// </summary>
        public string RouteBasePath { get; set; }

        /// <summary>
        /// Optional hook method fired before the service method
        /// is invoked. Method signature is async.
        /// </summary>
        public Func<ServiceHandlerRequestContext, Task> OnBeforeMethodInvoke { get; set; }

        /// <summary>
        /// Optional hook method fired after the service method is
        /// is invoked. Method signature is async.
        /// </summary>
        public Func<ServiceHandlerRequestContext, Task> OnAfterMethodInvoke { get; set; }
        
    }
}
