using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
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
                        


            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
