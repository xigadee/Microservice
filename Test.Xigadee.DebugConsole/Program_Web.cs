using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using Unity.WebApi;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static IDisposable mWebApp;

        static void MicroserviceWebAPIStart()
        {
            StartOptions options = new StartOptions();
            options.Urls.Add("http://localhost:29001");

            //options.Urls.Add("http://127.0.0.1:29001");
            //options.Urls.Add(string.Format("http://{0}:29001", Environment.MachineName));

            //var settings = Microsoft.Owin.Hosting.Utilities.SettingsLoader.LoadFromConfig();
            //StartOptions opts = new StartOptions(baseAddress);

            //opts.Settings.Add("paul", "cool");
            //opts.ServerFactory = "";
            try
            {
                mWebApp = WebApp.Start<Test.Xigadee.Api.Server.Startup>(options);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        static void MicroserviceWebAPIStop()
        {
            mWebApp.Dispose();
            mWebApp = null;
        }

        public class Startup
        {

            public void Configuration(IAppBuilder app)
            {
                try
                {
                    //AreaRegistration.RegisterAllAreas();
                    var config = new HttpConfiguration();
                    //app.UseWebApi(config);
                    
                    //Service.Initialise();

                    //GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(Service.Unity);
                    //GlobalConfiguration.Configure((c) => WebApiConfig.Register(c));

                    //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                    //RouteConfig.RegisterRoutes(RouteTable.Routes);
                    //BundleConfig.RegisterBundles(BundleTable.Bundles);

                    //Service.Start();

                    //ConfigureAuth(app);
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }

        }
    }
}
