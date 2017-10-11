using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    public static partial class AzureServiceBusExtensionMethods
    {
        /// <summary>
        /// Gets the namespace manager.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        public static NamespaceManagerREST GetNamespaceManager(this ServiceBusConnectionStringBuilder connection)
        {
            return new NamespaceManagerREST(connection);
        }
    }
}
