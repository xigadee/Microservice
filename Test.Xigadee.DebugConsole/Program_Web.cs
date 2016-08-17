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
        static string sApiHost = "http://localhost:29001";
        static void MicroserviceWebAPIStart()
        {
            try
            {
                StartOptions options = new StartOptions();
                options.Urls.Add(sApiHost);
                sMainMenu.Value.AddInfoMessage($"Api Starting @ {sApiHost}", true);

                sContext.ApiServer = WebApp.Start<Test.Xigadee.Api.Server.Startup>(options);

                sContext.ApiPersistence = new Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>(() => new ApiProviderAsyncV2<Guid, MondayMorningBlues>(new Uri(sApiHost)));
                sMainMenu.Value.AddInfoMessage("Api Started", true);

            }
            catch (Exception ex)
            {
                sMainMenu.Value.AddInfoMessage($"Api Failed: {ex.Message}", true);

                sContext.ApiPersistence = null;
                sContext.ApiServer = null;
            }
        }

        static void MicroserviceWebAPIStop()
        {
            sMainMenu.Value.AddInfoMessage("Api Stopping", true);
            sContext.ApiPersistence = null;
            sContext.ApiServer.Dispose();
            sContext.ApiServer = null;
            sMainMenu.Value.AddInfoMessage("Api Stopped", true);
        }

    }
}
