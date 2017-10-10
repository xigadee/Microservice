using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Xigadee
{
    /// <summary>
    /// This class is used to connect the TestContext class to the Xigadee configuration system.
    /// </summary>
    /// <seealso cref="Xigadee.ConfigResolver" />
    public class ConfigResolverTestContext: ConfigResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigResolverTestContext"/> class.
        /// </summary>
        /// <param name="context">The TestContext object.</param>
        /// <param name="prefix">The settings key prefix. 'CI_' if left blank.</param>
        /// <param name="throwExceptionOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        public ConfigResolverTestContext(TestContext context
            , string prefix = VisualStudioPipelineExtensions.ConfigResolverTestContextDefault_Prefix
            , bool throwExceptionOnNotFound = VisualStudioPipelineExtensions.ConfigResolverTestContextDefault_ThrowExceptionOnNotFound
            )
        {
            Context = context ?? throw new ArgumentNullException("context", $"{nameof(ConfigResolverTestContext)}: context cannot be null");
            Prefix = prefix?? "";
            ThrowException = throwExceptionOnNotFound;
        }

        private TestContext Context { get; set; }

        private string Prefix { get; set; }

        private bool ThrowException { get; set; }

        /// <summary>
        /// Use this method to get the value from the TestContext resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>
        /// Returns true if it can resolve.
        /// </returns>
        public override bool CanResolve(string key)
        {
            return Context.HasCISetting(key, Prefix);
        }
        /// <summary>
        /// Use this method to get the value from the TestContext resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>
        /// This is the settings value, null if not set.
        /// </returns>
        public override string Resolve(string key)
        {
            return Context.GetCISettingAsString(key, Prefix, ThrowException);
        }
    }
}
