using System;
using Microsoft.AspNetCore.Http;

namespace CODE.Framework.Core.ServiceHandler
{
    public class ServiceHandler
    {
        HttpContext Context { get; }
        ServiceHandlerServiceInstance ServiceInstance {get;}

        public ServiceHandler(HttpContext context, ServiceHandlerServiceInstance service)
        {
            Context = context;
            ServiceInstance = service;
        }

        public void ProcessRequest()
        {
            Context.Response.WriteAsync("Service Handler: " + this.ServiceInstance.ServiceTypeName);            
        }
    }
}
