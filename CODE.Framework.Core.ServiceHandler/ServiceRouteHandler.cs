using System;
using System.Diagnostics;
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
    public class ServiceRouteHandler
    {

        HttpRequest HttpRequest { get; }

        HttpResponse HttpResponse { get; }

        MethodInfo MethodToInvoke { get; }

        ServiceHandlerConfigurationInstance ServiceConfiguration { get; }

        RouteData RouteData { get; }


        public ServiceRouteHandler(HttpRequest request,
            HttpResponse response,
            RouteData routeData,
            ServiceHandlerConfigurationInstance serviceConfig,
            MethodInfo methodToCall)
        {
            HttpRequest = request;
            HttpResponse = response;
            RouteData = routeData;
            MethodToInvoke = methodToCall;

            ServiceConfiguration = serviceConfig;
        }

        public async Task ProcessRequest()
        {
            var context = new ServiceHandlerRequestContext()
            {
                HttpRequest = HttpRequest,
                HttpResponse = HttpResponse,
                ServiceConfig = ServiceConfiguration,
                Url = new ServiceHandlerRequestContextUrl()
                {
                    Url = UriHelper.GetDisplayUrl(HttpRequest),
                    UrlPath = HttpRequest.Path.Value.ToString(),
                    QueryString = HttpRequest.QueryString,
                    HttpMethod = HttpRequest.Method.ToUpper()
                }
            };

            try
            {
                if (context.ServiceConfig.HttpsMode == ControllerHttpsMode.RequireHttps &&
                    HttpRequest.Scheme != "https")
                    throw new UnauthorizedAccessException(Resources.ServiceMustBeAccessedOverHttps);

                if (ServiceConfiguration.OnAfterMethodInvoke != null)
                    await ServiceConfiguration.OnBeforeMethodInvoke(context);

                await ExecuteMethod(context);

                ServiceConfiguration.OnAfterMethodInvoke?.Invoke(context);

                if (string.IsNullOrEmpty(context.ResultJson))
                    context.ResultJson = JsonSerializationUtils.Serialize(context.ResultValue);

                SendJsonResponse(context, context.ResultValue);
            }
            catch (Exception ex)
            {
                var error = new ErrorResponse(ex);
                SendJsonResponse(context, error);
            }
        }


        public async Task ExecuteMethod(ServiceHandlerRequestContext handlerContext)
        {
            var config = ServiceHandlerConfiguration.Current;

            var httpVerb = handlerContext.HttpRequest.Method;

            if (httpVerb == "OPTIONS" && config.Cors.UseCorsPolicy)
            {
                // emty response - ASP.NET will provide CORS headers via applied policy
                handlerContext.HttpResponse.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            var serviceType = handlerContext.ServiceConfig.ServiceType;
            var inst = ReflectionUtils.CreateInstanceFromType(serviceType);
            if (inst == null)
                throw new InvalidOperationException(string.Format(Resources.UnableToCreateTypeInstance, serviceType));

            // parameter parsing
            var parameterList = new object[] { };
            object result = null;

            // simplistic - no parameters or single body post parameter
            var paramInfos = MethodToInvoke.GetParameters();
            if (paramInfos.Length > 0)
            {
                var parm = paramInfos[0];

                JsonSerializer serializer = new JsonSerializer();

                // there's always 1 parameter
                object parameterData = null;
                if (HttpRequest.ContentLength == null || HttpRequest.ContentLength < 1)
                    parameterData = ReflectionUtils.CreateInstanceFromType(parm.ParameterType);
                else
                {
                    using (var sw = new StreamReader(HttpRequest.Body))
                    using (JsonReader reader = new JsonTextReader(sw))
                    {
                        parameterData = serializer.Deserialize(reader, parm.ParameterType);
                    }
                }

                // Simple Parameter to object propery mapping
                if (RouteData != null)
                {
                    foreach (var kv in RouteData.Values)
                    {
                        var prop = parm.ParameterType.GetProperty(kv.Key,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty |
                            BindingFlags.IgnoreCase);
                        if (prop != null)
                        {
                            try
                            {
                                var val = ReflectionUtils.StringToTypedValue(kv.Value as string, prop.PropertyType);
                                ReflectionUtils.SetProperty(parameterData, kv.Key, val);
                            }
                            catch (Exception e)
                            {
                                throw new InvalidOperationException(
                                    string.Format("Unable set parameter from URL segment for property: {0}", kv.Key));
                            }
                        }
                    }
                }

                parameterList = new object[] {parameterData};
            }

            try
            {
                handlerContext.ResultValue = MethodToInvoke.Invoke(inst, parameterList);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(Resources.UnableToExecuteMethod, MethodToInvoke.Name,
                    ex.Message));
            }


            return;
        }


        static DefaultContractResolver CamelCaseNamingStrategy =
            new DefaultContractResolver {NamingStrategy = new CamelCaseNamingStrategy()};

        static DefaultContractResolver SnakeCaseNamingStrategy =
            new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()};

        static void SendJsonResponse(ServiceHandlerRequestContext context, object value)
        {
            var response = context.HttpResponse;

            response.ContentType = "application/json; charset=utf-8";

            JsonSerializer serializer = new JsonSerializer();

            if (context.ServiceConfig.JsonFormatMode == JsonFormatModes.CamelCase)
                serializer.ContractResolver = CamelCaseNamingStrategy;
            else if (context.ServiceConfig.JsonFormatMode == JsonFormatModes.SnakeCase)
                serializer.ContractResolver = SnakeCaseNamingStrategy;

#if DEBUG
            serializer.Formatting = Formatting.Indented;
#endif

            using (var sw = new StreamWriter(response.Body))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, value);
                }
            }
        }
                
    }
}

