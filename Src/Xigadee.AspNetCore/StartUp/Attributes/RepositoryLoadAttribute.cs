using System;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to specify a class that should be registered as a singleton for the application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RepositoryLoadAttribute : Attribute
    {
        /// <summary>
        /// You can specify a cast type that should be registered instead of the parameter or property return type.
        /// If this method cannot cast to this type, you will get an exception during runtime.
        /// </summary>
        public RepositoryLoadAttribute()
        {
        }
    }
}
