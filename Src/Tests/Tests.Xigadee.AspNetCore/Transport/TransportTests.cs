using System;
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
    public class TransportTests: TransportTestBase
    {
        /// <summary>
        /// Sets up this instance.
        /// </summary>
        [TestMethod]
        public async Task RegistrationCall()
        {
            var client = new TestConnector();

            client.ClientOverride = _client;

            var model = new RegistrationModel() { DisplayName = "Paul", Email = "paul@xigadee.org", Password = "NotARealPassword", PasswordConfirm = "NotARealPassword" };

            var rs = await client.Register(model);

            Assert.IsTrue(rs.IsSuccess);
        }
    }
}
