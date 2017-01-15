using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web.Http;
using Microsoft.Owin.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using Xigadee;

namespace Test.Xigadee.Api
{

    public class StartUp
    {
        private byte[] mSecret = Encoding.ASCII.GetBytes(JwtTests.SecretPass);

        public void Configuration(IAppBuilder app)
        {
            var webpipe = new WebApiMicroservicePipeline();

            webpipe.HttpConfig.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            webpipe
                .ApiAddMicroserviceUnavailableFilter()
                .ApiAddJwtTokenAuthentication(JwtHashAlgorithm.HS256, mSecret)
                ;

            webpipe.StartWebApi(app);

        }
    }
}
