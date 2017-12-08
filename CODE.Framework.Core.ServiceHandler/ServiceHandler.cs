using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Westwind.Utilities;

namespace CODE.Framework.Core.ServiceHandler
{
    public class ServiceHandler
    {
        HttpContext HttpContext { get; }

        ServiceHandlerConfigurationInstance ServiceConfigInstance {get;}

        IRouteBuilder Routes;

        public ServiceHandler(HttpContext context, 
                              ServiceHandlerConfigurationInstance serviceConfig)
        {
            HttpContext = context;
            ServiceConfigInstance = serviceConfig;            
        }

        public async Task ProcessRequest()
        {
            var context = new ServiceHandlerRequestContext() {
                 HttpContext = HttpContext,
                 ServiceConfig = ServiceConfigInstance
            };


            if (ServiceConfigInstance.OnAfterMethodInvoke != null)
                await ServiceConfigInstance.OnBeforeMethodInvoke(context);

            var inst = ReflectionUtils.CreateInstanceFromType(ServiceConfigInstance.ServiceType);
            await HttpContext.Response.WriteAsync("Service Handler: " + inst);

            if (ServiceConfigInstance.OnAfterMethodInvoke != null)
                await ServiceConfigInstance.OnBeforeMethodInvoke(context);
            
            //string method = routeContext.RouteData.Values[]
        }
    }
}

