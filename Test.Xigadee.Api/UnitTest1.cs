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
    [TestClass]
    public class UnitTest1
    {
        private TestServer mServer;
        private byte[] mSecret = Encoding.ASCII.GetBytes("Jwt is cool");
        private byte[] mSecretFalse = Encoding.ASCII.GetBytes("Jwt is wack");

        [TestInitialize]
        public void FixtureInit()
        {
            mServer = TestServer.Create<StartUp>();
        }

        [TestCleanup]
        public void FixtureCleanUp()
        {
            mServer.Dispose();
        }

        [TestMethod]
        public void TestMethodSuccess()
        {
            var token =  new JwtToken();

            token.Claims.Audience="api";
            token.Claims.NotBefore = DateTime.UtcNow.AddHours(-1);
            token.Claims.ExpirationTime = DateTime.UtcNow.AddHours(1);
            token.Claims.IssuedAt = DateTime.UtcNow;
            

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "/api/test");
            message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.ToString(mSecret));

            var response = mServer.HttpClient
                .SendAsync(message)
                .Result;

            var result = response.Content.ReadAsAsync<IEnumerable<string>>().Result;

            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("hello", result.First());
            Assert.AreEqual("world", result.Last());
        }


        [TestMethod]
        public void TestMethodFail401()
        {
            var token = new JwtToken();

            token.Claims.Audience = "api";
            token.Claims.NotBefore = DateTime.UtcNow.AddHours(-1);
            token.Claims.ExpirationTime = DateTime.UtcNow.AddHours(1);
            token.Claims.IssuedAt = DateTime.UtcNow;

            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "/api/test");
            message.Headers.Authorization = new AuthenticationHeaderValue("bearer", token.ToString(mSecretFalse));

            var response = mServer.HttpClient
                .SendAsync(message)
                .Result;

            Assert.IsTrue(response.StatusCode == System.Net.HttpStatusCode.Forbidden);
        }

    }

    public class TestController: ApiController
    {
        public IEnumerable<string> Get()
        {
            var principal = Thread.CurrentPrincipal;
            return new[] { "hello", "world" };
        }

        public string Get(int id)
        {
            return "hello world";
        }
    }

    public class StartUp
    {
        private byte[] mSecret = Encoding.ASCII.GetBytes("Jwt is cool");

        public void Configuration(IAppBuilder app)
        {
            var webpipe = new WebApiMicroservicePipeline();

            webpipe.HttpConfig.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            webpipe
                
                .ApiAddJwtTokenAuthentication(JwtHashAlgorithm.HS256, mSecret)
                ;

            webpipe.StartWebApi(app);

        }
    }
}
