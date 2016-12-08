using System;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Azure
{
    [TestClass]
    [Ignore] // Integration / debug testing only 
    public class ConfigResolverKeyVaultTest
    {
        private ConfigResolverKeyVault mConfigResolver;

        [TestInitialize]
        public void Initialize()
        {
            mConfigResolver = new ConfigResolverKeyVault(new ClientCredential("", ""), "");

        }


        [TestMethod]
        public void ResolveKey()
        {
            Assert.IsFalse(mConfigResolver.CanResolve("Blah"));
            Assert.IsNull(mConfigResolver.Resolve("Blah"));

            Assert.IsTrue(mConfigResolver.CanResolve("TestSecret"));
            Assert.IsNotNull(mConfigResolver.Resolve("TestSecret"));

            Assert.IsFalse(mConfigResolver.CanResolve("A:Weird:Key"));
        }
    }
}
