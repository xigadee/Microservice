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
                var Service = new PopulatorWebApi();

                //config.DependencyResolver = 
                Service.ApiConfig.EnableCors(new OpenCorrsPolicy());
                // Web API configuration and services
                // Configure Web API to use only bearer token authentication.
                //config.SuppressDefaultHostAuthentication();
                //Service.ApiConfig.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

                //config.Filters.Add(CreateBlobLoggingFilter());

                Service.ApiConfig.Filters.Add(new WebApiVersionHeaderFilter());
                //config.Formatters.Insert(0, new ByteArrayMediaTypeFormatter()); // Add before any of the default formatters

                //Enable attribute based routing for HTTP verbs.
                Service.ApiConfig.MapHttpAttributeRoutes();

                // Add additional convention-based routing for the default controller.
                Service.ApiConfig.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "v1/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                // Add additional convention-based routing for the default controller.
                Service.ApiConfig.Routes.MapHttpRoute(
                    name: "ODataMetadata",
                    routeTemplate: "v1/OData/OData.svc/$metadata",
                    defaults: new { id = RouteParameter.Optional }
                );

                // Add additional convention-based routing for the default controller.
                Service.ApiConfig.Routes.MapHttpRoute(
                    name: "ODataBatch",
                    routeTemplate: "v1/OData/OData.svc/$batch",
                    defaults: new { id = RouteParameter.Optional }
                );

                // Add additional convention-based routing for the default controller.
                Service.ApiConfig.Routes.MapHttpRoute(
                    name: "OData",
                    routeTemplate: "v1/OData/OData.svc/{controller}",
                    defaults: new { id = RouteParameter.Optional }
                );


                //app.UseJwtBearerAuthentication(
                //Service.Initialise();
                //config.DependencyResolver =  new UnityDependencyResolver(Service.Unity);
                //GlobalConfiguration.Configuration.DependencyResolver = config.DependencyResolver;
                //GlobalConfiguration.Configure((c) => Register(c));

                //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

                Service.Start(app, AzureHelper.Resolver);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}
