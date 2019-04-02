using System;
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
        public async Task TestMethod1()
        {
            var prov = new UserSecurityModuleMemoryTest<TestUser>();

            var user = new TestUser() { Username = "pauls" };

            var rs = await prov.Users.Create(user);

            var rsr = await prov.Users.Read(user.Id);
        }
    }
}
