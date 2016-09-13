#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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