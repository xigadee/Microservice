using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static IDisposable mWebApp;

        static void MicroserviceWebAPIStart()
        {
            string baseAddress = "http://localhost:29001/";
            var settings = Microsoft.Owin.Hosting.Utilities.SettingsLoader.LoadFromConfig();
            StartOptions opts = new StartOptions(baseAddress);
            
            opts.Settings.Add("paul", "cool");
            opts.ServerFactory = "";
            try
            {
                mWebApp = WebApp.Start<Test.Xigadee.Api.Server.Startup>(opts);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //opts.Settings.

        }
        static void MicroserviceWebAPIStop()
        {
            mWebApp.Dispose();
            mWebApp = null;
        }
    }
}
