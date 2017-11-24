using System;
using Microsoft.AspNetCore.Http;

namespace CODE.Framework.Core.ServiceHandler
{
    public class ServiceHandler
    {
        HttpContext Context { get; }

        public ServiceHandler(HttpContext context)
        {
            Context = context;    
        }
    }
}
