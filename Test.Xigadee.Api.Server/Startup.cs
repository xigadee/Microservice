using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Unity.WebApi;
using Xigadee;

[assembly: OwinStartup(typeof(Test.Xigadee.Api.Server.Startup))]

namespace Test.Xigadee.Api.Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                var config = new HttpConfiguration();

                //config.DependencyResolver = 
                config.EnableCors(new OpenCorrsPolicy());
                // Web API configuration and services
                // Configure Web API to use only bearer token authentication.
                //config.SuppressDefaultHostAuthentication();
                config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

                //config.Filters.Add(CreateBlobLoggingFilter());
                config.Filters.Add(new WebApiVersionHeaderFilter());
                //config.Formatters.Insert(0, new ByteArrayMediaTypeFormatter()); // Add before any of the default formatters

                //Enable attribute based routing for HTTP verbs.
                config.MapHttpAttributeRoutes();

                // Add additional convention-based routing for the default controller.
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "v1/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );
                
                app.UseWebApi(config);

                //app.UseJwtBearerAuthentication(
                //Service.Initialise();
                //config.DependencyResolver =  new UnityDependencyResolver(Service.Unity);
                GlobalConfiguration.Configuration.DependencyResolver = config.DependencyResolver;
                GlobalConfiguration.Configure((c) => Register(c));

                //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

                //Service.Start();

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public static void Register(HttpConfiguration config)
        {
            config.EnableCors(new OpenCorrsPolicy());

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //config.Filters.Add(CreateBlobLoggingFilter());
            config.Filters.Add(new WebApiVersionHeaderFilter());
            //config.Formatters.Insert(0, new ByteArrayMediaTypeFormatter()); // Add before any of the default formatters

            //Enable attribute based routing for HTTP verbs.
            config.MapHttpAttributeRoutes();

            // Add additional convention-based routing for the default controller.
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        /// <summary> Creates the HTTP configuration. </summary>
        /// <returns> An <see cref="HttpConfiguration"/> to bootstrap the hosted API </returns>
        public static HttpConfiguration CreateHttpConfiguration()
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            
            return config;
        }

    }
}
