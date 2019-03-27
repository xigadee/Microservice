using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Xigadee
{
    [TestClass]
    [TestCategory("Unit")]
    [TestCategory("ExcludeFromCI")]
    public class ASPNETCoreTests
    {
        private HttpClient _client;

        [TestInitialize]
        public void Setup()
        {
            var testServer = new TestServer(new WebHostBuilder().UseStartup<TestStartup>());
            _client = testServer.CreateClient();
        }

        [TestMethod]
        public async Task ReadEntity()
        {
            // Arrange
            //var customBearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiJhNGY4ZTc2MDQxZWY0NWJlYWEzOGIyOTA0OThiM2YyMSIsImV4cCI6MTU1OTk4NDY2NywiaXNzIjoiY29yaW50LndvcmxkcmVtaXQuY29tIiwiYXVkIjoiRGV2ZWxvcG1lbnQifQ.JXGB7b2Lb3uu_wi_H-HcDdZoKgPYfcYExNjbKeSxXPs";

            // Act
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get
            //    , "api/values");
            , "api/mondaymorningblues/9A2E3F6D-3B98-4C2C-BD45-74F819B5EDFC");

            //httpRequestMessage.Headers.Add("Authorization", $"bearer {customBearerToken}");

            var result = await _client.SendAsync(httpRequestMessage);

        }
    }
}
