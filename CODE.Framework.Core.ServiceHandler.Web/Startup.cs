using System;
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
//using Sample.Services.Implementation;

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
            // Add service and create Policy with options
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddServiceHandler(config =>
            {
                config.Services.AddRange(new List<ServiceHandlerConfigurationInstance>
                {
                    new ServiceHandlerConfigurationInstance
                    {
                        ServiceType = typeof(UserService),
                        //ServiceTypeName = "Sample.Services.Implementation.UserService",
                        //AssemblyName = "Sample.Services.Implementation",
                        RouteBasePath = "/api/users"
                    },
                    new ServiceHandlerConfigurationInstance
                    {
                        ServiceTypeName = "Sample.Services.Implementation.CustomerService",
                        AssemblyName = "Sample.Services.Implementation",
                        RouteBasePath = "/api/customers"
                    }
                });
            });
            
            //services.AddMvc(opt=>
            //{
                
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ServiceHandlerConfiguration config)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}


            //if (config.Cors.UseCorsPolicy)
                //app.UseCors(config.Cors.CorsPolicyName);

            app.UseServiceHandler();


            //app.UseMvc();
        }
    }
}
