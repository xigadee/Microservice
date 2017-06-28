using System;

namespace Xigadee
{
    /// <summary>
    /// Indicates that http traffic should be allowed - allows certain actions to allow through
    /// http traffic when the requires https filter has been added globally
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AllowHttpTrafficAttribute : Attribute
    {
        /// <summary>
        /// Applies to only local calls
        /// </summary>
        public bool LocalOnly { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="localOnly">This applies to only local traffic i.e. localhost calls</param>
        public AllowHttpTrafficAttribute(bool localOnly = true)
        {
            LocalOnly = localOnly;
        }
    }
}
