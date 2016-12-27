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
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Xigadee
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="O"></typeparam>
    /// <typeparam name="C"></typeparam>
    public abstract class AzureStorageConnectorBase<O,C,S>
        where O: Microsoft.WindowsAzure.Storage.IRequestOptions
        where C: AzureStorageContainerBase
    {
        public DataCollectionSupport Support { get; set; }

        public AzureStorageDataCollectorOptions Options { get; set; }

        public Func<EventBase, S> Serializer { get; set; }

        /// <summary>
        /// This is the root id for the storage container.
        /// </summary>
        public string RootId { get; set; }

        /// <summary>
        /// This method returns the default request options if set.
        /// </summary>
        public virtual O RequestOptionsDefault { get; set; }
        /// <summary>
        /// This abstract method is used to convert the incoming entity to a serializable format
        /// and associated metadata that is ready to be written to the underlying storage.
        /// </summary>
        /// <param name="e">The event entity.</param>
        /// <param name="id">The microservice properties.</param>
        /// <returns>Returns a container containing the serialized entity and associated metadata.</returns>
        public abstract C Convert(EventBase e, MicroserviceId id);

    }

    public abstract class AzureStorageConnectorBinary<O, C>: AzureStorageConnectorBase<O, C, byte[]>
        where O : Microsoft.WindowsAzure.Storage.IRequestOptions
        where C : AzureStorageContainerBase
    {
        public AzureStorageConnectorBinary()
        {
             Serializer = AzureStorageDCExtensions.DefaultJsonBinarySerializer;
        }

        public override C Convert(EventBase e, MicroserviceId id)
        {
            throw new NotImplementedException();
        }
    }

}
