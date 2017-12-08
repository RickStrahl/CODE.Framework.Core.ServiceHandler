﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sample.Services.Implementation;

namespace CODE.Framework.Core.ServiceHandler.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceHandler(config =>
            {
                config.Services.AddRange(new List<ServiceHandlerConfigurationInstance>
                {
                    new ServiceHandlerConfigurationInstance
                    {
                        ServiceTypeName = "Sample.Services.Implementation.UserService",
                        AssemblyName = "Sample.Services.Implementation",
                        //ServiceType = typeof(UserService),
                        RouteBasePath = "/api/users"
                    },
                    //new ServiceHandlerConfigurationInstance
                    //{
                    //    ServiceTypeName = "CustomerService",
                    //    AssemblyName = "CustomerService",
                    //    RouteBasePath = "/api/customers"
                    //}
                });
            });
            
            services.AddMvc(opt=>
            {
                
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

            app.UseMvc();
        }
    }
}
