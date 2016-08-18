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
        static void MicroserviceWebAPIStart()
        {
            try
            {
                StartOptions options = new StartOptions();
                options.Urls.Add(sContext.ApiUri.ToString());
                sMenuMain.Value.AddInfoMessage($"Api Starting @ {sContext.ApiUri.ToString()}", true);
                sContext.ApiPersistence.Status = 1;

                sContext.ApiServer = WebApp.Start<Test.Xigadee.Api.Server.Startup>(options);

                sMenuMain.Value.AddInfoMessage("Api Started", true);
                sContext.ApiPersistence.Status = 2;

            }
            catch (Exception ex)
            {
                sMenuMain.Value.AddInfoMessage($"Api Failed: {ex.Message}", true);

                sContext.ApiServer = null;
                sContext.ApiPersistence.Status = 0;
            }
        }

        static void MicroserviceWebAPIStop()
        {
            sMenuMain.Value.AddInfoMessage("Api Stopping", true);
            sContext.ApiPersistence.Status = 0;
            sContext.ApiServer.Dispose();
            sContext.ApiServer = null;
            sMenuMain.Value.AddInfoMessage("Api Stopped", true);
        }

    }
}
