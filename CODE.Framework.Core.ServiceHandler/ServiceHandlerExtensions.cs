using System;
using CODE.Framework.Core.ServiceHandler.Properties;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Westwind.Utilities;

namespace CODE.Framework.Core.ServiceHandler
{
    public static class ServiceHandlerExtensions
    {

        /// <summary>
        /// Configure the service and make it so you can inject 
        /// IOptions<ServiceHandlerConfiguration>
        /// You can also 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceHandler(this IServiceCollection services,  Action<ServiceHandlerConfiguration> optionsAction)
        {
            // add strongly typed configuration
            services.AddOptions();

            var provider = services.BuildServiceProvider();
            var serviceConfiguration = provider.GetService<IConfiguration>();


            var config = new ServiceHandlerConfiguration();
            serviceConfiguration.Bind("ServiceHandler", config);

            ServiceHandlerConfiguration.Current = config;
            
            optionsAction?.Invoke(config);

            foreach(var svc in config.Services)
            {
                if (svc.ServiceType == null)
                {
                    Type type = ReflectionUtils.GetTypeFromName(svc.ServiceTypeName);
                    if (type == null)
                    {
                        if (ReflectionUtils.LoadAssembly(svc.AssemblyName) == null)
                            throw new ArgumentException(string.Format(Resources.InvalidServiceType, svc.ServiceTypeName));
                        type = ReflectionUtils.GetTypeFromName(svc.ServiceTypeName);
                        if (type == null)
                            throw new ArgumentException(string.Format(Resources.InvalidServiceType, svc.ServiceTypeName));
                    }
                    svc.ServiceType = type;
                }
            }

            // Add configured instance to DI
            services.AddSingleton(config);

            // Add service and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy(config.Cors.CorsPolicyName,
                    builder =>
                    {
                        if (config.Cors.AllowedOrigins == "*")
                            builder = builder.AllowAnyOrigin();
                        else if (!string.IsNullOrEmpty(config.Cors.AllowedOrigins))
                        {
                            var origins = config.Cors.AllowedOrigins.Split(new[] {',', ';'},
                                StringSplitOptions.RemoveEmptyEntries);

                            builder.WithOrigins(origins);
                        }

                        if (!string.IsNullOrEmpty(config.Cors.AllowedMethods))
                        {
                            var methods = config.Cors.AllowedMethods.Split(new[] {',', ';'},
                                StringSplitOptions.RemoveEmptyEntries);
                            builder.WithMethods(methods);
                        }

                        if (!string.IsNullOrEmpty(config.Cors.AllowedHeaders))
                        {
                            var headers = config.Cors.AllowedHeaders.Split(new[] {',', ';'},
                                StringSplitOptions.RemoveEmptyEntries);
                            builder.WithHeaders(headers);
                        }

                        if (config.Cors.AllowCredentials)
                            builder.AllowCredentials();
                    });
            });

            return services;
        }


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
                // conditionally route to service handler base on RouteBasePath
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
                    builder.UseCors(config.Cors.CorsPolicyName);
                    builder.UseMiddleware<ServiceHandlerMiddleware>();
                    
                });
            }

        

            return appBuilder;
        }

    }
}