using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
        public static IServiceCollection AddServiceHandler(this IServiceCollection services, Action<ServiceHandlerConfiguration> optionsAction)
        {
            // add strongly typed configuration
            services.AddOptions();

            var provider = services.BuildServiceProvider();
            var serviceConfiguration = provider.GetService<IConfiguration>();

            var section = serviceConfiguration.GetSection("ServiceHandlerConfiguration");
            // read settings from DbResourceConfiguration in Appsettings.json
            services.Configure<ServiceHandlerConfiguration>(section);
            
            provider = services.BuildServiceProvider();
            var configData = provider.GetRequiredService<IOptions<ServiceHandlerConfiguration>>();

            ServiceHandlerConfiguration config;

            if (configData != null && configData.Value != null )
                config = configData.Value;                
            else
               config = new ServiceHandlerConfiguration();

            ServiceHandlerConfiguration.Current = config;
            
            optionsAction?.Invoke(config);

            return services;
        }



    }
}