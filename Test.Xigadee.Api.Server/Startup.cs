using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Owin;
using Owin;
using Unity.WebApi;

[assembly: OwinStartup(typeof(Test.Xigadee.Api.Server.Startup))]

namespace Test.Xigadee.Api.Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            try
            {
                //AreaRegistration.RegisterAllAreas();
                var config = new HttpConfiguration();
                app.UseWebApi(config);

                Service.Initialise();

                GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(Service.Unity);
                GlobalConfiguration.Configure((c) => WebApiConfig.Register(c));

                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);

                Service.Start();

                ConfigureAuth(app);
            }
            catch (Exception ex)
            {

                throw ex;
            }

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
