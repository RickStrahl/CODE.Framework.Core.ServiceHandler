﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace CODE.Framework.Core.ServiceHandler.Web.Generic
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceHandler(config =>
            {
                // configuration comes from appsettings.j

                //config.Services.AddRange(new List<ServiceHandlerConfigurationInstance>
                //{
                //    new ServiceHandlerConfigurationInstance
                //    {
                //        ServiceType = typeof(UserService),
                //        //ServiceTypeName = "Sample.Services.Implementation.UserService",
                //        //AssemblyName = "Sample.Services.Implementation",
                //        RouteBasePath = "/api/users"
                //    },
                //    new ServiceHandlerConfigurationInstance
                //    {
                //        ServiceTypeName = "Sample.Services.Implementation.CustomerService",
                //        AssemblyName = "Sample.Services.Implementation",
                //        RouteBasePath = "/api/customers"
                //    }
                //});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseServiceHandler();
        }
    }
}
