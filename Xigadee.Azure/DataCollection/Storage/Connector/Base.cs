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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Xigadee
{
    /// <summary>
    /// This is the common interface for storage connectors.
    /// </summary>
    public interface IAzureStorageConnectorBase
    {
        /// <summary>
        /// This is the default timeout.
        /// </summary>
        TimeSpan? DefaultTimeout { get; set; }
        /// <summary>
        /// This is the specific EventBase type supported for the connector.
        /// </summary>
        DataCollectionSupport Support { get; set; }
        /// <summary>
        /// This is the cloud storage account used for all connectivity.
        /// </summary>
        CloudStorageAccount StorageAccount { get; set; }
        /// <summary>
        /// This is the specific storage options.
        /// </summary>
        AzureStorageDataCollectorOptions Options { get; set; }
        /// <summary>
        /// This is the Azure storage operation context.
        /// </summary>
        OperationContext Context { get; set; }
        /// <summary>
        /// This method initializes the connector.
        /// </summary>
        void Initialize();
    }
    /// <summary>
    /// This is the base class shared by all connectors.
    /// </summary>
    /// <typeparam name="O">The request options that determines the retry policy.</typeparam>
    /// <typeparam name="C">The container type.</typeparam>
    /// <typeparam name="S">The serialization type.</typeparam>
    public abstract class AzureStorageConnectorBase<O,C,S>: IAzureStorageConnectorBase
        where O: Microsoft.WindowsAzure.Storage.IRequestOptions
        where C: AzureStorageContainerBase
    {
        /// <summary>
        /// This is the specific EventBase type supported for the connector.
        /// </summary>
        public DataCollectionSupport Support { get; set; }
        /// <summary>
        /// This is the specific storage options.
        /// </summary>
        public AzureStorageDataCollectorOptions Options { get; set; }
        /// <summary>
        /// This is the cloud storage account used for all connectivity.
        /// </summary>
        public CloudStorageAccount StorageAccount { get; set; }
        /// <summary>
        /// This is the Azure storage operation context.
        /// </summary>
        public OperationContext Context { get; set; }
        /// <summary>
        /// This function is used to create the specific ids for the entity;
        /// </summary>
        public Func<EventBase, MicroserviceId, Tuple<string, string>> IdMaker { get; set; }
        /// <summary>
        /// This function serializes the event entity.
        /// </summary>
        public Func<EventBase, S> Serializer { get; set; }
        /// <summary>
        /// This is the default timeout.
        /// </summary>
        public TimeSpan? DefaultTimeout { get; set; } 
        /// <summary>
        /// This is the root id for the storage container.
        /// </summary>
        public string RootId { get; set; }
        /// <summary>
        /// This method returns the default request options if set.
        /// </summary>
        public virtual O RequestOptionsDefault { get; set; }
        /// <summary>
        /// This method writes to the incoming event to the underlying storage technology.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <param name="id">The microservice metadata.</param>
        /// <returns>This is an async task.</returns>
        public abstract Task Write(EventBase e, MicroserviceId id);
        /// <summary>
        /// This method initializes the connector.
        /// </summary>
        public abstract void Initialize();
    }
}
