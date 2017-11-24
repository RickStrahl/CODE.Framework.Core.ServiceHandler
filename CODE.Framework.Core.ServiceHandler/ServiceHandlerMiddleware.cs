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

            context.Response.WriteAsync("Made it into the Service handler: " + context.Request.PathBase);

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
                appBuilder.Map(service.RouteBasePath, builder =>
                {
                    builder.UseMiddleware<ServiceHandlerMiddleware>();
                });
            }

            return appBuilder;
        }
    }
}
