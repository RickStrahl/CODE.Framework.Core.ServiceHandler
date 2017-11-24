using System;
using System.Collections.Generic;
using System.Text;

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
        public string ServiceTypeName
        {
            get { return _ServiceTypeName; }
            set { _ServiceTypeName = value; }
        }
        private string _ServiceTypeName = string.Empty;

        public string AssemblyName { get; set; }

        public string RouteBasePath { get; set; }

        /// <summary>
        /// You can specify a specific type to bind rather than
        /// providing 
        /// </summary>
        public Type ServiceType { get; set; }

    }
}
