using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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

        public async Task Invoke( HttpContext context,
                            IHostingEnvironment host)
        {
            var requestPath = context.Request.Path.ToString().ToLower();

            var serviceConfig = ServiceHandlerConfiguration.Current.Services.FirstOrDefault(sv => (requestPath + "/").StartsWith(sv.RouteBasePath.ToLower()+ "/"));
            if (serviceConfig == null)
            {
                await _next(context);
                return;
            }
            
            var handler = new ServiceHandler(context, serviceConfig);            
            await handler.ProcessRequest();

            // Call the next delegate/middleware in the pipeline
            return;
        }
    }
}
