#region using
using Microsoft.Azure.ServiceBus;
using System;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the Azure Service Bus connection information.
    /// </summary>
    public class AzureServiceBusConnection
    {
        /// <summary>
        /// This is the Azure Service Bus connection information.
        /// </summary>
        /// <param name="connection">The Azure Service Bus connection string.</param>
        /// <param name="defaultReceiveMode">The default receive mode.</param>
        /// <param name="defaultRetryPolicy">The retry policy;</param>
        public AzureServiceBusConnection(ServiceBusConnectionStringBuilder connection
            , ReceiveMode defaultReceiveMode
            , RetryPolicy defaultRetryPolicy
            )
        {
            if (connection == null)
                throw new ArgumentNullException("connection", "connection cannot be null or empty for an Azure Service Bus Connection");

            Connection = connection;
            DefaultReceiveMode = defaultReceiveMode;
            DefaultRetryPolicy = defaultRetryPolicy;
        }

        /// <summary>
        /// This is the Service Bus connection.
        /// </summary>
        public ServiceBusConnectionStringBuilder Connection { get; }

        /// <summary>
        /// The default receive mode.
        /// </summary>
        public ReceiveMode DefaultReceiveMode { get; set; }

        /// <summary>
        /// The default retry policy.
        /// </summary>
        public RetryPolicy DefaultRetryPolicy { get; set; }

    }
}
