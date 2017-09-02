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
        #region TestContext
        /// <summary>
        /// All hail the Microsoft test magic man!
        /// This class can be populated with values through Visual Studio menus -> [Test>Test Settings>Select Test Settings File] and then selecting a file with the extension .runsettings
        /// See here for details: https://msdn.microsoft.com/en-us/library/jj635153.aspx
        /// There is a default file default.runsettings that has a set of empty CI injection parameters specified for testing in this project.
        /// </summary>
        public TestContext TestContext
        {
            get; set;
        } 
        #endregion

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
