using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.Xigadee;
using Xigadee;

namespace Tests.Xigadee
{
    [TestClass]
    [TestCategory("Unit")]
    [TestCategory("ExcludeFromCI")]
    public class ASPNETCoreTests: TransportTestBase
    {
        readonly Guid keyId = new Guid("{811EAFE5-D7E2-4FA8-8264-6BEAF5D63B8D}");

        public async Task<string> CreateSessionToken()
        {

            // Act
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "api/security/createsession");

            var result = await _client.SendAsync(httpRequestMessage);

            var tokenB = await result.Content.ReadAsByteArrayAsync();
            
            return Encoding.UTF8.GetString(tokenB);

        }

        [TestMethod]
        public async Task ReadEntity()
        {
            try
            {
                var token = await CreateSessionToken();

                var b64 = Convert.ToBase64String(keyId.ToByteArray());

                var prov = new ApiProviderAsyncV2<Guid, MondayMorningBlues>(
                    new Uri("http://localhost/api"), authHandler: (JwtAuthProvider)token
                    );



                prov.ClientOverride = _client;

                var rs1 = await prov.Read(new Guid("9A2E3F6D-3B98-4C2C-BD45-74F819B5EDFC"));

                var e = new MondayMorningBlues() { NotEnoughCoffee = true, NotEnoughSleep = false };

                var rs2 = await prov.Create(e);
                Assert.IsTrue(rs2.IsSuccess);

                var rs2v = await prov.Version(rs2.Entity.Id);
                Assert.IsTrue(rs2v.IsSuccess);

                var s2a = await prov.Search("$select=obiwan, coffee, sleep&$filter=coffee eq True");
                rs2.Entity.NotEnoughCoffee = false;
                var rs3 = await prov.Update(rs2.Entity);

                var rs3a = await prov.Read(rs3.Entity.Id);

                var rs4 = await prov.Version(rs2.Entity.Id);

                var s1 = await prov.SearchEntity("$top=10&$skip=1");
                var s2b = await prov.Search("$select=obiwan, coffee, sleep&$filter=coffee eq True");

                var d1 = await prov.Delete(rs2.Entity.Id);
                var d1b = await prov.Read(rs2.Entity.Id);

                var er1 = await prov.Read(Guid.NewGuid());

                var ev1 = await prov.Version(Guid.NewGuid());

                // Arrange
                //var customBearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiJhNGY4ZTc2MDQxZWY0NWJlYWEzOGIyOTA0OThiM2YyMSIsImV4cCI6MTU1OTk4NDY2NywiaXNzIjoiY29yaW50LndvcmxkcmVtaXQuY29tIiwiYXVkIjoiRGV2ZWxvcG1lbnQifQ.JXGB7b2Lb3uu_wi_H-HcDdZoKgPYfcYExNjbKeSxXPs";

                // Act
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get
                //    , "api/values");
                , $"api/mondaymorningblues/{e.Id.ToString("N")}");

                //httpRequestMessage.Headers.Add("Authorization", $"bearer {customBearerToken}");

                var result = await _client.SendAsync(httpRequestMessage);
            }
            catch (Exception ex)
            {

                Assert.Fail(ex.Message);
            }


        }
    }
}
