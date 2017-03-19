using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Owin;
using Xigadee;

[assembly: OwinStartup(typeof(Test.Xigadee.Api.Web2.StartUp))]
namespace Test.Xigadee.Api.Web2
{
    public partial class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            //try
            //{
            //    app.UseApplicationInsights(new OperationIdContextMiddlewareConfiguration { ShouldTryGetIdFromHeader = true, OperationIdHeaderName = "X-CorrelationId" });

            //    var pipeline = new UnityWebApiMicroservicePipeline("API");

            //    pipeline.Configure();

            //    pipeline.StartWebApi(app);
            //}
            //catch (Exception ex)
            //{
            //    sLogger.Log(LogLevel.Error, ex);
            //    throw;
            //}
        }
    }
}
