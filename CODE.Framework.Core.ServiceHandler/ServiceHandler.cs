using System;
using Microsoft.AspNetCore.Http;

namespace CODE.Framework.Core.ServiceHandler
{
    public class ServiceHandler
    {
        HttpContext Context { get; }
        ServiceHandlerConfigurationInstance ServiceConfigInstance {get;}

        public ServiceHandler(HttpContext context, ServiceHandlerConfigurationInstance serviceConfig)
        {
            Context = context;
            ServiceConfigInstance = serviceConfig;
        }

        public void ProcessRequest()
        {
            Context.Response.WriteAsync("Service Handler: " + this.ServiceConfigInstance.ServiceTypeName);            
        }
    }
}
