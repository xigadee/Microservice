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
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for Azure Queue communication.
    /// </summary>
    public class AzureQueueFabricBridge : FabricBridgeBase<ICommunicationBridge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureQueueFabricBridge"/> class.
        /// Which is used to provides connectivity using the Azure Storage Queue.
        /// </summary>
        public AzureQueueFabricBridge(StorageCredentials credentials)
        {
            Credentials = credentials;
        }

        /// <summary>
        /// Gets the Azure storage credentials credentials.
        /// </summary>
        protected StorageCredentials Credentials { get; }

        /// <summary>
        /// Gets the <see cref="ICommunicationBridge"/> with the specified mode.
        /// </summary>
        /// <value>
        /// The <see cref="ICommunicationBridge"/>.
        /// </value>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">Only queue is supported. Broadcast is not available on Azure Storage.</exception>
        public override ICommunicationBridge this[FabricMode mode]
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}
