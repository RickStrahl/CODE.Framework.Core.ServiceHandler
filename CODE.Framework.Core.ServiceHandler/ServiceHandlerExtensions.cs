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


            ServiceHandlerConfiguration configData = new ServiceHandlerConfiguration();
            serviceConfiguration.Bind("ServiceHandler", configData);

          
            //var section = serviceConfiguration.GetSection("ServiceHandler");
            //// read settings from DbResourceConfiguration in Appsettings.json
            //services.Configure<ServiceHandlerConfiguration>(section);



            //provider = services.BuildServiceProvider();
            //var configData = provider.GetRequiredService<IOptions<ServiceHandlerConfiguration>>();

            ServiceHandlerConfiguration config;

            //if (configData != null && configData.Value != null )
            //    config = configData.Value;                
            //else
            //   config = new ServiceHandlerConfiguration();

            config = configData;

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