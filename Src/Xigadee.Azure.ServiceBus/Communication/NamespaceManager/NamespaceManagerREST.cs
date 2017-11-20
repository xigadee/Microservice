#region using
using Microsoft.Azure.ServiceBus;
using System;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class provides a temporary replacement for the missing SAS-based management functionality 
    /// in the new .NET Standard libraries.
    /// Thanks to Ruppert Koch for code examples at https://code.msdn.microsoft.com/Service-Bus-HTTP-client-fe7da74a
    /// </summary>
    public class NamespaceManagerREST
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceManagerREST"/> class.
        /// </summary>
        /// <param name="connection">The Service Bus SAS connection builder object.</param>
        public NamespaceManagerREST(ServiceBusConnectionStringBuilder connection)
        {
            Connection = connection;
        }

        #region Connection
        /// <summary>
        /// This is the Azure connection class.
        /// </summary>
        public ServiceBusConnectionStringBuilder Connection { get; }
        #endregion
    }
}
