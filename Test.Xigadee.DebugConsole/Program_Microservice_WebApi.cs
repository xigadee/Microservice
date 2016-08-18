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
            sContext.ApiServer.StatusChanged += StatusChanged;
            sContext.ApiServer.Start();
        }

        static void MicroserviceWebAPIStop()
        {
            sContext.ApiServer.Stop();
        }

    }
}
