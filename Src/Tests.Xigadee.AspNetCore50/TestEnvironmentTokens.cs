using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Tests.Xigadee
{
    /// <summary>
    /// This class holds the set of environment tokens defined in the test context.
    /// </summary>
    public abstract class TestEnvironmentTokens
    {
        /// <summary>
        /// This method is used to populate the token.
        /// </summary>
        /// <param name="testContext"></param>
        /// <param name="type"></param>
        /// <param name="tokenPrefix"></param>
        /// <param name="uri"></param>
        protected virtual void Populate(TestContext testContext, TestHostType type, string tokenPrefix, Uri uri = null)
        {
            TestContext = testContext;
            Uri = uri;

            switch (type)
            {
                case TestHostType.Local:
                    ContextTokenPrefix = $"{tokenPrefix}Local";
                    break;
                case TestHostType.Remote:
                    ContextTokenPrefix = $"{tokenPrefix}Remote";
                    break;
            }
        }

        /// <summary>
        /// This is the token prefix.
        /// </summary>
        public string ContextTokenPrefix { get; protected set; }
        /// <summary>
        /// This is the test context.
        /// </summary>
        public TestContext TestContext { get; protected set; }

        /// <summary>
        /// This is the Api Uri.
        /// </summary>
        public Uri Uri { get; protected set; }


    }
}
