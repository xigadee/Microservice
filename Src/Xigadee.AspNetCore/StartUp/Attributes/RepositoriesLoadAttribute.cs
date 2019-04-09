using System;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to specify a class that should be processed for repositories that need to be set on application startup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RepositoriesProcessAttribute : Attribute
    {
        /// <summary>
        /// This is the constructor.
        /// </summary>
        public RepositoriesProcessAttribute()
        {
        }
    }

    /// <summary>
    /// This attribute is used to specify a class that should not be processed for repositories that need to be set on application startup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class StopRepositoriesProcessAttribute : Attribute
    {
        /// <summary>
        /// This is the constructor.
        /// </summary>
        public StopRepositoriesProcessAttribute()
        {
        }
    }


}
