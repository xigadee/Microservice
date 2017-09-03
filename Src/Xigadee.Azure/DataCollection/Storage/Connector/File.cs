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
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.File;

namespace Xigadee
{
    /// <summary>
    /// This class is not currently supported.
    /// </summary>
    [Obsolete("This class is a placeholder and is not currently supported.")]
    public class AzureStorageConnectorFile: AzureStorageConnectorBase<FileRequestOptions, AzureStorageBinary>
    {
        /// <summary>
        /// This is the file client.
        /// </summary>
        public CloudFileClient Client { get; set; }
        /// <summary>
        /// This method writes to the incoming event to the underlying storage technology.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <param name="id">The microservice metadata.</param>
        /// <returns>
        /// This is an async task.
        /// </returns>
        /// <exception cref="NotImplementedException">Not currently supported.</exception>
        public override Task Write(EventHolder e, MicroserviceId id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// This method initializes the connector.
        /// </summary>
        /// <exception cref="NotImplementedException">Not currently supported.</exception>
        public override void Initialize()
        {
            if (ContainerId == null)
                ContainerId = AzureStorageHelper.GetEnum<DataCollectionSupport>(Support).StringValue;

            throw new NotImplementedException();
        }
        /// <summary>
        /// This method is used to check that the specific event should be written to the underlying storage.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <returns>
        /// Returns true if the event should be written. Currently always returns false;
        /// </returns>
        public override bool ShouldWrite(EventHolder e)
        {
            return false;
            //return Options.IsSupported(AzureStorageBehaviour.File, e);
        }
    }
}
