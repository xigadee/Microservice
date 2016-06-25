using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Xigadee;

namespace Test.Xigadee.Api.Server
{
    public static class WebApiConfig
    {
        public class MyCorrsPolicy: ICorsPolicyProvider
        {
            public async Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return new CorsPolicy() { AllowAnyHeader = true, AllowAnyMethod = true, AllowAnyOrigin = true };
            }
        }

        public static void Register(HttpConfiguration config)
        {
            config.EnableCors(new MyCorrsPolicy());

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //config.Filters.Add(CreateBlobLoggingFilter());
            config.Filters.Add(new WebApiVersionHeaderFilter());
            config.Formatters.Insert(0, new ByteArrayMediaTypeFormatter()); // Add before any of the default formatters

            config.MapHttpAttributeRoutes();

            // Convention-based routing.
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
