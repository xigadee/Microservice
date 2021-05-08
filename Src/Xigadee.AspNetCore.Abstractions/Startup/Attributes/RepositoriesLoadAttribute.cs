using System;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to specify a class that should be processed for repositories that need to be set on application startup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
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
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class StopRepositoriesProcessAttribute : Attribute
    {
        /// <summary>
        /// This is the constructor.
        /// </summary>
        public StopRepositoriesProcessAttribute()
        {
        }
    }

    /// <summary>
    /// This attribute is used to specify a module class that requires start stop support.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ModuleStartStopAttribute : Attribute
    {
        /// <summary>
        /// This is the constructor.
        /// </summary>
        public ModuleStartStopAttribute(bool autoConnect = false)
        {
            AutoConnect = autoConnect;
        }

        /// <summary>
        /// This property specifies that the container should autoconnect the module.
        /// </summary>
        public bool AutoConnect { get; }
    }
}
