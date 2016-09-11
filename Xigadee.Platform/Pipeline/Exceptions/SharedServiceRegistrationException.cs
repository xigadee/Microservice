using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when the registration of a shared service fails.
    /// </summary>
    public class SharedServiceRegistrationException: Exception
    {
        public SharedServiceRegistrationException(string typeName, string serviceName)
        {
            TypeName = typeName;
            ServiceName = serviceName;
        }

        public string TypeName { get; }

        public string ServiceName { get; }
    }
}
