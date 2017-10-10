#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
