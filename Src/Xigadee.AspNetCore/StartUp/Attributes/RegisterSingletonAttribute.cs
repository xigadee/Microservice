using System;

namespace Xigadee
{
    /// <summary>
    /// This attribute is used to specify a class that should be registered as a singleton for the application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RegisterAsSingletonAttribute:Attribute
    {
        /// <summary>
        /// You can specify a cast type that should be registered instead of the parameter or property return type.
        /// If this method cannot cast to this type, you will get an exception during runtime.
        /// </summary>
        /// <param name="registerType">The derived type to register for the singleton. Leave this blank if you do not wish to cast.</param>
        public RegisterAsSingletonAttribute(Type registerType = null)
        {
            SingletonRegistrationType = registerType;
        }

        /// <summary>
        /// This is the registration type for the Singelton.
        /// </summary>
        public Type SingletonRegistrationType { get; }
    }

    /// <summary>
    /// This attribute is used to specify a class that should be registered as a singleton for the application.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class DoNotRegisterAsSingletonAttribute : Attribute
    {
        /// <summary>
        /// You can specify a cast type that should be registered instead of the parameter or property return type.
        /// If this method cannot cast to this type, you will get an exception during runtime.
        /// </summary>
        /// <param name="registerType">The derived type to register for the singleton. Leave this blank if you do not wish to cast.</param>
        public DoNotRegisterAsSingletonAttribute(Type registerType = null)
        {
            SingletonRegistrationType = registerType;
        }

        /// <summary>
        /// This is the registration type for the Singelton.
        /// </summary>
        public Type SingletonRegistrationType { get; }
    }
}
