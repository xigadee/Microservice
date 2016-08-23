using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Xigadee;

namespace Test.Xigadee.Api.Server
{
    public static class RouteConfig
    {
        public static void Register(PopulatorWebApi Service)
        {
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
                defaults: new { id = RouteParameter.Optional, controller = "OData4", action = "Metadata" }
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
        }
    }
}