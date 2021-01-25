using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Xigadee;

namespace Tests.Xigadee
{
    /// <summary>
    /// This class forms the base of unit tests for an Api in the MsTest environment.
    /// </summary>
    /// <typeparam name="S">The start-up class.</typeparam>
    /// <typeparam name="C">The connector type.</typeparam>
    public abstract class MsTestHostBase<C,S,T>: TestHostBase<C, S>
        where S : class, IStartup
        where C : ApiProviderBase, new()
        where T : TestEnvironmentTokens, new()
    {
        /// <summary>
        /// This dictionary contains the environment tokens.
        /// </summary>
        protected Dictionary<string, T> _tokens = new Dictionary<string, T>();

        #region ServerCleanUp()
        /// <summary>
        /// Cleans up and disposes of the instance.
        /// </summary>
        [TestCleanup]
        public override void ServerCleanUp() => base.ServerCleanUp(); 
        #endregion

        #region TestContext
        /// <summary>
        /// This is the incoming test context.
        /// </summary>
        protected TestContext _testContext;
        /// <summary>
        /// This is the incoming test context.
        /// </summary>
        public TestContext TestContext
        {
            get => _testContext;

            set
            {
                _testContext = value;
                TestContextPopulate(value);
            }
        }
        /// <summary>
        /// This method can be overriden to set specific tokens or data from the test context.
        /// </summary>
        protected abstract void TestContextPopulate(TestContext tCtx);
        #endregion
    }
}
