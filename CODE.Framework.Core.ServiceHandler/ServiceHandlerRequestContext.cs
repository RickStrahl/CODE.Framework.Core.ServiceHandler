using Microsoft.AspNetCore.Http;

namespace CODE.Framework.Core.ServiceHandler
{
    /// <summary>
    /// Holds Request related data through the lifetime ofr
    /// a service handler request.
    /// </summary>
    public class ServiceHandlerRequestContext
    {
        public HttpContext HttpContext { get; set; }

        public ServiceHandlerConfigurationInstance ServiceConfig { get; set; }

        public object ResultValue { get; set; }

        public string ResultJson { get; set; }
    
        public ServiceHandlerRequestContextUrl Url { get; set; } = new ServiceHandlerRequestContextUrl();        
    }

    public class ServiceHandlerRequestContextUrl
    {
        public string Url { get; internal set; }
        public string UrlPath { get; internal set; }
        public QueryString QueryString { get; internal set; }
        public string HttpMethod { get; internal set; }
    }
}

