using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Swashbuckle.Application;
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

                //config.Formatters.Insert(0, new ByteArrayMediaTypeFormatter()); // Add before any of the default formatters

                //Enable attribute based routing for HTTP verbs.
                Service.ApiConfig.MapHttpAttributeRoutes();

                // Add additional convention-based routing for the default controller.
                Service.ApiConfig.Routes.MapHttpRoute(
                    name: "Security",
                    routeTemplate: "v1/account/{action}/{id}",
                    defaults: new { id = RouteParameter.Optional, controller = "Security" }
                );

                Service.ApiConfig.Routes.MapHttpRoute(
                    name: "DefaultPersistence",
                    routeTemplate: "v1/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                // Add additional convention-based routing for the default controller.
                Service.ApiConfig.Routes.MapHttpRoute(
                    name: "ODataMetadata",
                    routeTemplate: "v1/OData/OData.svc/$metadata",
                    defaults: new { id = RouteParameter.Optional, controller = "OData4", action="Metadata" }
                );

                // Add additional convention-based routing for the default controller.
                Service.ApiConfig.Routes.MapHttpRoute(
                    name: "ODataBatch",
                    routeTemplate: "v1/OData/OData.svc/$batch",
                    defaults: new { id = RouteParameter.Optional, controller = "OData4", action = "Batch" }
                );


                Service.ApiConfig.Routes.MapHttpRoute(
                    name: "OData",
                    routeTemplate: "v1/OData/OData.svc/{controller}",
                    defaults: new { action = "Search" }, constraints: null, 
                    handler: new HttpMethodChangeHandler(Service.ApiConfig, "SEARCH"));

                // /swagger/ui/index
                Service.ApiConfig.EnableSwagger(c =>
                {
                    c.IncludeXmlComments("docs.XML");
                    c.Schemes(new[] { "http", "https" });
                    c.SingleApiVersion("1.0", "Xigadee Test API");
                })
                .EnableSwaggerUi();

                Service.Start(app, AzureHelper.Resolver);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
     }
}
