using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CODE.Framework.Core.ServiceHandler.Properties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Westwind.Utilities;

namespace CODE.Framework.Core.ServiceHandler
{
    public class ServiceRouHandler
    {

        private HttpRequest HttpRequest { get; }
        HttpResponse HttpResponse { get;  }

        ServiceHandlerConfigurationInstance ServiceConfiguration {get;}

        IRouteBuilder Routes;

        public ServiceHandler(HttpRequest request,
                              HttpResponse response,
                              IRouter router,
                              ServiceHandlerConfigurationInstance serviceConfig)
        {
            HttpContext = context;
            ServiceConfiguration = serviceConfig;            
        }

        public async Task ProcessRequest()
        {
            var context = new ServiceHandlerRequestContext()
            {
                HttpContext = HttpContext,
                ServiceConfig = ServiceConfiguration,
                Url = new ServiceHandlerRequestContextUrl()
                {
                    Url = UriHelper.GetDisplayUrl(HttpContext.Request),
                    UrlPath = HttpContext.Request.Path.Value.ToString(),
                    QueryString = HttpContext.Request.QueryString,
                    HttpMethod = HttpContext.Request.Method.ToUpper()
                }
            };

            try
            {
                if (context.ServiceConfig.HttpsMode == ControllerHttpsMode.RequireHttps && HttpContext.Request.Scheme != "https")
                    throw new UnauthorizedAccessException(Resources.ServiceMustBeAccessedOverHttps);

                if (ServiceConfiguration.OnAfterMethodInvoke != null)
                    await ServiceConfiguration.OnBeforeMethodInvoke(context);

                await ExecuteMethod(context);

                ServiceConfiguration.OnAfterMethodInvoke?.Invoke(context);

                if (string.IsNullOrEmpty(context.ResultJson))
                    context.ResultJson = JsonSerializationUtils.Serialize(context.ResultValue);

                SendJsonResponse(context, context.ResultValue);
            }
            catch(Exception ex)
            {
                var error = new ErrorResponse(ex);
                SendJsonResponse(context, error);
            }
        }


        public async Task ExecuteMethod(ServiceHandlerRequestContext handlerContext)
        {
            var config = ServiceHandlerConfiguration.Current;

            var httpVerb = handlerContext.HttpContext.Request.Method;

            if (httpVerb == "OPTIONS" && config.Cors.UseCorsPolicy)
            {
                // emty response - ASP.NET will provide CORS headers via applied policy
                handlerContext.HttpContext.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            // get the adjusted - methodname.
            var lowerPath = handlerContext.Url.UrlPath;
            if (!lowerPath.EndsWith("/"))
                lowerPath += "/";

            var basePath = ServiceConfiguration.RouteBasePath;
            if (!basePath.EndsWith("/"))
                basePath += "/";

            var path = lowerPath.Replace(basePath.ToLower(), "");

          // pick up the configured service implementation type and create an instance
            var serviceType = handlerContext.ServiceConfig.ServiceType;
            var inst = ReflectionUtils.CreateInstanceFromType(serviceType);

            if (inst == null)
                throw new InvalidOperationException(string.Format(Resources.UnableToCreateTypeInstance, serviceType));
            
            // No explicitly defined contract interface found. Therefore, we try to use one implicitly
            var interfaces = serviceType.GetInterfaces();
            if (interfaces.Length < 1)
                throw new NotSupportedException(Resources.HostedServiceRequiresAnInterface);

            // assume service interface is first interface
            var contractType = interfaces[0];
            
            var methodToInvoke = GetMethodNameFromUrlFragmentAndContract(path, handlerContext.Url.HttpMethod, contractType);
            if (methodToInvoke == null)
            {
                methodToInvoke = GetMethodNameFromUrlFragmentAndContract(path, handlerContext.Url.HttpMethod, serviceType);
                if (methodToInvoke == null)
                    throw new InvalidOperationException(string.Format(Resources.ServiceMethodDoesntExist, methodToInvoke.Name, handlerContext.Url.HttpMethod));
            }


            var parameterList = new object[] { };
            object result = null;

            if(HttpContext.Request.ContentLength > 0)
            {
                // simplistic - no parameters or single body post parameter
                var paramInfos = methodToInvoke.GetParameters();
                if (paramInfos.Length > 0)
                {
                    var parm = paramInfos[0];

                    JsonSerializer serializer = new JsonSerializer();
                    

                    using (var sw = new StreamReader(HttpContext.Request.Body))
                    {
                        using (JsonReader reader = new JsonTextReader(sw))
                        {
                            var parameterData = serializer.Deserialize(reader, parm.ParameterType);
                            parameterList = new object[] {parameterData};
                        }
                    }
                }
            }

            try
            {
                handlerContext.ResultValue = methodToInvoke.Invoke(inst, parameterList);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(string.Format(Resources.UnableToExecuteMethod, methodToInvoke.Name,ex.Message));
            }

            
            return;
        }


        static void SendJsonResponse(ServiceHandlerRequestContext context, object value)
        {
            var response = context.HttpContext.Response;

            response.ContentType = "application/json; charset=utf-8";            

            JsonSerializer serializer = new JsonSerializer();
#if DEBUG
            serializer.Formatting = Formatting.Indented;
#endif
            if (context.ServiceConfig.JsonFormatMode != JsonFormatModes.ProperCase)
            {
                var resolver = serializer.ContractResolver as DefaultContractResolver;
                if (context.ServiceConfig.JsonFormatMode == JsonFormatModes.CamelCase)
                    resolver.NamingStrategy = new CamelCaseNamingStrategy();
                else if(context.ServiceConfig.JsonFormatMode == JsonFormatModes.SnakeCase)
                    resolver.NamingStrategy = new SnakeCaseNamingStrategy();
            }

            using (var sw = new StreamWriter(response.Body))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, value);
                }                
            }        
        }


        /// <summary>
        /// Extracts the name of the method a REST call was aimed at based on the provided url "fragment" (URL minus the root URL part and minus the method name),
        /// the HTTP method (get, post, put, ...) and the contract type
        /// </summary>
        /// <param name="urlFragment">The URL fragment.</param>
        /// <param name="httpMethod">The HTTP method. Note: Can by string.empty for extensionless URL</param>
        /// <param name="serviceType">Service contract type.</param>
        /// <returns>Method picked as a match within the contract (or null if no matching method was found)</returns>
        /// <remarks>
        /// Methods are picked based on a number of parameters for each fragment and HTTP method.
        /// 
        /// Example URL Fragment: /CustomerSearch/Smith (HTTP-GET)
        /// 
        /// In this case, the "CustomerSearch" part of the fragment is considered a good candidate for a method name match.
        /// The method thus looks at the contract definition and searches for methods of the same name (case insensitive!)
        /// as well as the Rest(Name="xxx") attribute on each method to see if there is a match. If a match is found, the HTTP-Method is also
        /// compared and has to be a match (there could be two methods of the same exposed name, but differing HTTP methods/verbs).
        /// 
        /// If no matching method is found, "CustomerSearch" is considered to be a parameter rather than a method name, and therefore, the method
        /// name is assumed to be empty (the default method). Therefore, a method with a [Rest(Name="")] with a matching HTTP method is searched for.
        /// For a complete match, the method in question would thus have to have the following attribute declared: [Rest(Name="", Method=RestMethods.Get)]
        /// </remarks>
        public static MethodInfo GetMethodNameFromUrlFragmentAndContract(string urlFragment, string httpMethod, Type serviceType)
        {

            var tokens = urlFragment.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            string urlMethod;
            if (tokens.Length == 0)
                urlMethod = string.Empty;
            else
                urlMethod = tokens[0];

            string[] parameters = { };
            if (tokens.Length > 1)
                parameters = tokens.Skip(1).ToArray();
            
            var methodInfos = serviceType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase);
            
            // We first check for methods on the interface: Attribute Name and Method
            foreach (var method in methodInfos)
            {
                var mi = FindMethod(method,urlMethod, httpMethod);
                if (mi != null)
                    return mi;                
            }

            throw new ArgumentException($"Couldn't resolve method name: \"{urlMethod}\" with HTTP method: \"{httpMethod}\" in contract: \"{serviceType.Name}\".");

            //var localMethod = serviceType.GetMethod(urlMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.IgnoreCase);
            //return FindMethod(localMethod, urlMethod, httpMethod);           
        }


        static MethodInfo FindMethod(MethodInfo method,string urlMethod, string httpMethod)
        {          
            var restAttribute = GetRestAttribute(method);
            var methodName = method.Name;
            if (restAttribute != null && restAttribute.Name != null)
                methodName = restAttribute.Name;
            var httpMethodForMethod = restAttribute?.Method.ToString().ToUpper() ?? "GET";
            if (httpMethodForMethod == httpMethod && string.Equals(methodName, urlMethod, StringComparison.CurrentCultureIgnoreCase))
                return method;
            if (httpMethodForMethod == "POSTORPUT" &&
                (string.Equals("POST", httpMethod, StringComparison.OrdinalIgnoreCase) ||
                string.Equals("PUT", httpMethod, StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(methodName, urlMethod, StringComparison.CurrentCultureIgnoreCase))
                return method;

            return null;
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

        ///// <summary>
        ///// Extracts the RestUrlParameterAttribute from a property's attributes
        ///// </summary>
        ///// <param name="property">The property.</param>
        ///// <returns>The applied RestUrlParameterAttribute or a default RestUrlParameterAttribute</returns>
        //public static RestUrlParameterAttribute GetRestUrlParameterAttribute(PropertyInfo property)
        //{
        //    var customAttributes = property.GetCustomAttributes(typeof(RestUrlParameterAttribute), true);
        //    if (customAttributes.Length <= 0) return new RestUrlParameterAttribute();
        //    var restAttribute = customAttributes[0] as RestUrlParameterAttribute;
        //    return restAttribute ?? new RestUrlParameterAttribute();
        //}
    }

    public enum ControllerHttpsMode
    {
        Undefined,
        Http,
        RequireHttps,
        RequireHttpsExceptLocalhost
    }
}

