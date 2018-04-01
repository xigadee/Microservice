namespace Xigadee
{
    /// <summary>
    /// This is the abstract base config resolver class. Override this class to sort this out.
    /// </summary>
    public abstract class ConfigResolver
    {
        /// <summary>
        /// Use this method to get the value from the specific resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>Returns true if it can resolve.</returns>
        public abstract bool CanResolve(string key);
        /// <summary>
        /// Use this method to get the value from the specific resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>This is the settings value, null if not set.</returns>
        public abstract string Resolve(string key);
    }
}
