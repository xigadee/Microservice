﻿using System;
using System.Net;
using System.Net.Http;
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
        [TestMethod]
        public async Task ReadEntity()
        {
            try
            {
                var prov = new ApiProviderAsyncV2<Guid, MondayMorningBlues>(
                    new Uri("http://localhost/api")
                    );

                prov.ClientOverride = _client;

                var rs1 = await prov.Read(new Guid("9A2E3F6D-3B98-4C2C-BD45-74F819B5EDFC"));

                var e2 = new MondayMorningBlues() { NotEnoughCoffee = true, NotEnoughSleep = false };

                var rs2 = await prov.Create(e2);

                var rs3 = await prov.Update(rs2.Entity);


                // Arrange
                //var customBearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiJhNGY4ZTc2MDQxZWY0NWJlYWEzOGIyOTA0OThiM2YyMSIsImV4cCI6MTU1OTk4NDY2NywiaXNzIjoiY29yaW50LndvcmxkcmVtaXQuY29tIiwiYXVkIjoiRGV2ZWxvcG1lbnQifQ.JXGB7b2Lb3uu_wi_H-HcDdZoKgPYfcYExNjbKeSxXPs";

                // Act
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get
                //    , "api/values");
                , $"api/mondaymorningblues/{e2.Id.ToString("N")}");

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