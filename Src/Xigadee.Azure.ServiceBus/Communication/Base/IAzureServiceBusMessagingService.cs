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

namespace Xigadee
{
    /// <summary>
    /// This interface is used for Azure service bus messaging.
    /// </summary>
    /// <typeparam name="P">The partition config type.</typeparam>
    public interface IAzureServiceBusMessagingService<P>: IMessagingService<P>, IAzureServiceBusMessagingService
        where P : PartitionConfig
    {

    }

    /// <summary>
    /// This is the default Azure Service Bus properties.
    /// </summary>
    /// <seealso cref="Xigadee.IMessagingService{P}" />
    /// <seealso cref="Xigadee.IAzureServiceBusMessagingService" />
    public interface IAzureServiceBusMessagingService
    {
        /// <summary>
        /// This is the Azure Service Bus connection information.
        /// </summary>
        AzureServiceBusConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the Azure Service Bus entity name.
        /// </summary>
        string EntityName { get; set; }
    }
}
