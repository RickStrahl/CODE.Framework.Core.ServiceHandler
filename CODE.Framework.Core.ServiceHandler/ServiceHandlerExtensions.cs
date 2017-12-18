using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using CODE.Framework.Core.ServiceHandler.Properties;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
        public static IServiceCollection AddServiceHandler(this IServiceCollection services,
            Action<ServiceHandlerConfiguration> optionsAction)
        {
            // add strongly typed configuration
            services.AddOptions();
            services.AddRouting();

            var provider = services.BuildServiceProvider();
            var serviceConfiguration = provider.GetService<IConfiguration>();


            var config = new ServiceHandlerConfiguration();
            serviceConfiguration.Bind("ServiceHandler", config);

            ServiceHandlerConfiguration.Current = config;

            optionsAction?.Invoke(config);

            foreach (var svc in config.Services)
            {
                if (svc.ServiceType == null)
                {
                    Type type = ReflectionUtils.GetTypeFromName(svc.ServiceTypeName);
                    if (type == null)
                    {
                        if (ReflectionUtils.LoadAssembly(svc.AssemblyName) == null)
                            throw new ArgumentException(
                                string.Format(Resources.InvalidServiceType, svc.ServiceTypeName));
                        type = ReflectionUtils.GetTypeFromName(svc.ServiceTypeName);
                        if (type == null)
                            throw new ArgumentException(
                                string.Format(Resources.InvalidServiceType, svc.ServiceTypeName));
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
                            requestPath.StartsWith(servicePath.Replace("//", "/") + "/");

                        return matched;
                    },
                    builder =>
                    {

                        builder.UseCors(config.Cors.CorsPolicyName);


                        // ROUTINE SERVICE HANDLER
                        builder.UseRouter(routeBuilder =>
                        {
                             var interfaces = service.ServiceType.GetInterfaces();
                             if (interfaces.Length < 1)
                                    throw new NotSupportedException(Resources.HostedServiceRequiresAnInterface);

                            foreach (var method in service.ServiceType.GetMethods(
                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.DeclaredOnly))
                            {
                                // find service contract                                
                                var interfaceMethod = interfaces[0].GetMethod(method.Name);
                                if (interfaceMethod == null)
                                    continue;

                                var restAttribute = GetRestAttribute(interfaceMethod);
                                if (restAttribute == null)
                                    continue;

                                Func<HttpRequest, HttpResponse, RouteData, Task> exec =
                                    async (req, resp, routeData) =>
                                    {
                                        await resp.WriteAsync(service.ServiceType + " . " + method.Name);

                                        foreach (var kv in routeData.Values)
                                        {
                                            await resp.WriteAsync($"\r\n-- {kv.Key} - {kv.Value}");
                                        }

                                        //var handler = new ServiceHandler(context, serviceConfig);
                                        //await handler.ProcessRequest();                                
                                    };

                                var relativeRoute = restAttribute.Route;
                                if (relativeRoute == null)
                                    relativeRoute = method.Name;

                                string fullRoute =
                                    (service.RouteBasePath + "/" + relativeRoute).Replace("//", "/");
                                if (fullRoute.StartsWith("/"))
                                    fullRoute = fullRoute.Substring(1);
                                
                                routeBuilder.MapVerb(restAttribute.Method.ToString(), fullRoute, exec);
                                routeBuilder.MapVerb("OPTIONS", fullRoute, async (req, resp, route) =>
                                {
                                    resp.StatusCode = StatusCodes.Status204NoContent;
                                });
                                Debug.WriteLine("Created route for: " + fullRoute  +  " " +  restAttribute.Method);
                            }
                        });
                        // END ROUTING SERVICE HANDLER

                        // builder.UseMiddleware<ServiceHandlerMiddleware>();                        

                    });
            }

            return appBuilder;
        }


        

        /// <summary>
        /// Extracts the RestAttribute from a method's attributes
        /// </summary>
        /// <param name="method">The method to be inspected</param>
        /// <returns>The applied RestAttribute or a default RestAttribute.</returns>
        public static RestAttribute GetRestAttribute(MethodInfo method)
        {
            var customAttributes = method.GetCustomAttributes(typeof(RestAttribute), true);
            if (customAttributes.Length <= 0) return new RestAttribute();
            var restAttribute = customAttributes[0] as RestAttribute;
            return restAttribute ?? new RestAttribute();
        }


    }
}