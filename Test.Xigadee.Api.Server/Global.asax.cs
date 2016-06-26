using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity.WebApi;

namespace Test.Xigadee.Api.Server
{
    public class WebApiApplication: System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            Service.Initialise();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(Service.Unity);
            GlobalConfiguration.Configure((c) => WebApiConfig.Register(c));

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Service.Start();
        }
    }
}
