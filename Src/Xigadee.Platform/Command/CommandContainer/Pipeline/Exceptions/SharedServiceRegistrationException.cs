using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when the registration of a shared service fails.
    /// </summary>
    public class SharedServiceRegistrationException: Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SharedServiceRegistrationException"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="serviceName">Name of the service.</param>
        public SharedServiceRegistrationException(string typeName, string serviceName)
        {
            TypeName = typeName;
            ServiceName = serviceName;
        }
        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        public string TypeName { get; }
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public string ServiceName { get; }
    }
}
