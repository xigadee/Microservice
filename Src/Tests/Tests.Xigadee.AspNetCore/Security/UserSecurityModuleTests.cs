using System;
using System.Linq;
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
    public class UserSecurityModuleTests
    {
        [TestMethod]
        public async Task TestMethodUSession()
        {
            var uSess = new UserSession();

            uSess.AddCustomClaim("One", "two");

            Assert.IsTrue(uSess.HasCustomClaims);
            var claim = uSess.CustomClaims().First();
            Assert.IsTrue(claim.type == "One");
        }

        [TestMethod]
        public async Task TestMethod1()
        {
            var prov = new UserSecurityModule<TestUser>().SetAsMemoryBased();

            var user = new TestUser() { Username = "pauls" };

            var rs = await prov.Users.Create(user);

            var rsr = await prov.Users.Read(user.Id);
        }

        [TestMethod]
        public async Task TestMethodHmm()
        {
            var prov = new UserSecurityModule<TestUser>().SetAsMemoryBased();

            var user = new User();

            try
            {
                var rs = await prov.Users.Create(user);
            }
            catch (Exception ex)
            {

                //Assert.ThrowsException<
            }

            //var rsr = await prov.Users.Read(user.Id);
        }
    }
}
