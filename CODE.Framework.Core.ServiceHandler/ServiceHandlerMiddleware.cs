using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CODE.Framework.Core.ServiceHandler
{
    public class ServiceHandlerMiddleware
    {

        private readonly RequestDelegate _next;

        public ServiceHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var handler = new ServiceHandler(context);

            context.Response.WriteAsync("Made it into the Service handler: " + context.Request.Path);

            // Call the next delegate/middleware in the pipeline
            return Task.CompletedTask; //return _next(context);
        }
    }

    public static class ServiceHandlerMiddlewareExtensions
    {

        /// <summary>
        /// Hook up routed maps to service handlers.
        /// </summary>
        /// <param name="appBuilder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseServiceHandler(
            this IApplicationBuilder appBuilder)
        {
            var config = ServiceHandlerConfiguration.Current;

            foreach (var service in config.Services)
            {                
                appBuilder.MapWhen(
                  context =>
                  {
                      var requestPath = context.Request.Path.ToString().ToLower();
                      var servicePath = service.RouteBasePath.ToLower();
                      bool matched = 
                            requestPath == servicePath ||    
                            requestPath.StartsWith(servicePath + "/");

                      return matched;
                  }, 
                builder =>
                {
                    builder.UseMiddleware<ServiceHandlerMiddleware>();
                });
            }

            return appBuilder;
        }
    }
}
