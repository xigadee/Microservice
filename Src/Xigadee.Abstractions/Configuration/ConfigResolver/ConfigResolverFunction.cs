using System;

namespace Xigadee
{
    /// <summary>
    /// This is the function resolver.
    /// </summary>
    public class ConfigResolverFunction: ConfigResolver
    {
        private Func<string, string, string> mResolver;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigResolverFunction"/> class.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <exception cref="System.ArgumentNullException">resolver cannot be null</exception>
        public ConfigResolverFunction(Func<string, string, string> resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException("resolver cannot be null");
            mResolver = resolver;
        }
        /// <summary>
        /// Use this method to get the value from the resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>
        /// Returns true if it can resolve.
        /// </returns>
        public override bool CanResolve(string key)
        {
            return mResolver(key, null) != null;
        }
        /// <summary>
        /// Use this method to get the value from the resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>
        /// This is the settings value, null if not set.
        /// </returns>
        public override string Resolve(string key)
        {
            return mResolver(key, null);
        }
    }
}
