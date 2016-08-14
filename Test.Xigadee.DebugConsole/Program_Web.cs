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
            try
            {
                StartOptions options = new StartOptions();
                options.Urls.Add("http://localhost:29001");
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

    }
}
