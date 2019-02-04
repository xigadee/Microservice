using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xigadee
{
    /// <summary>
    /// This static class provides shortcuts when using the TestContext class.
    /// </summary>
    public static partial class VisualStudioPipelineExtensions
    {
        /// <summary>
        /// This method creates a configuration container .
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Returns and environment configuration.</returns>
        public static IEnvironmentConfiguration ToConfiguration(this TestContext context)
        {
            var config = new ConfigBase();

            config.ResolverSet(20, new ConfigResolverTestContext(context));

            return config;
        }

        /// <summary>
        /// Gets the CI setting from the test context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <param name="prefix">The prefix. By default this is CI_</param>
        /// <param name="throwExceptionOnNotFound">This specifies whether the method should throw an exception if the key/value pair cannot be found.</param>
        /// <returns>The setting, if it can be found.</returns>
        public static object GetCISetting(this TestContext context, string key, string prefix = "CI_", bool throwExceptionOnNotFound = false)
        {
            string ciKey = CIKey(key, prefix);

            if (throwExceptionOnNotFound && !context.HasCISetting(key, prefix))
                throw new TestContextSettingException($"{ciKey} could not be found in the TestContext supplied.");

            var result = context.Properties[ciKey];

            return result;
        }

        /// <summary>
        /// Gets the CI setting from the test context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <param name="prefix">The prefix. By default this is CI_</param>
        /// <param name="throwExceptionOnNotFound">This specifies whether the method should throw an exception if the key/value pair cannot be found.</param>
        /// <param name="convert">This function can be used to convert an object value in the context properties to a string.</param>
        /// <returns>The string setting, if it can be found.</returns>
        public static string GetCISettingAsString(this TestContext context, string key, string prefix = "CI_", bool throwExceptionOnNotFound = false, Func<object,string> convert = null)
        {
            var result = context.GetCISetting(key, prefix, throwExceptionOnNotFound);

            if (result == null)
                return null;

            if (result is string)
                return result as string;

            if (convert == null)
                throw new TestContextSettingException($"{CIKey(key, prefix)} was found in the TestContext but was not a string value, and no conversion function was supplied.");

            return convert(result);
        }
        /// <summary>
        /// Determines whether the specified setting exists in the TestContext.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>
        /// Returns true if the key exists.
        /// </returns>
        public static bool HasCISetting(this TestContext context, string key, string prefix = "CI_")
        {
#if NET47
            return context.Properties.Contains(CIKey(key, prefix));
#else
            return context.Properties.ContainsKey(CIKey(key, prefix));
#endif
        }

        private static string CIKey(string key, string prefix)
        {
            return $"{prefix??""}{key}";
        }
    }
}
