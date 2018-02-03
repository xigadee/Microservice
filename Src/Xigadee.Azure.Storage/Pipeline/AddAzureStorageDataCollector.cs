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
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static partial class AzureStorageExtensionMethods
    {
        /// <summary>
        /// Adds the azure storage data collector.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creds">The Azure storage credentials.</param>
        /// <param name="adjustPolicy">The adjust policy.</param>
        /// <param name="resourceProfile">The resource profile.</param>
        /// <param name="handler">The encryption handler id.</param>
        /// <param name="onCreate">The optional on-create intercept action.</param>
        /// <param name="context">The operation context.</param>
        /// <returns>Returns the pipeline</returns>
        /// <exception cref="EncryptionHandlerNotResolvedException">This error is thrown when the encryption handler id is not found.</exception>
        public static P AddAzureStorageDataCollector<P>(this P pipeline
            , StorageCredentials creds = null
            , Action<AzureStorageDataCollectorPolicy> adjustPolicy = null
            , ResourceProfile resourceProfile = null
            , EncryptionHandlerId handler = null
            , Action<AzureStorageDataCollector> onCreate = null
            , OperationContext context = null
            )
            where P : IPipeline
        {
            AzureStorageDataCollectorPolicy policy = new AzureStorageDataCollectorPolicy();

            if (handler != null)
            {
                if (!pipeline.Service.ServiceHandlers.Encryption.Contains(handler.Id))
                    throw new EncryptionHandlerNotResolvedException(handler.Id);
            }

            adjustPolicy?.Invoke(policy);

            if (creds == null)
                creds = pipeline.Configuration.AzureStorageCredentials(true);

            var component = new AzureStorageDataCollector(creds, policy
                , context: context
                , encryptionId:handler);

            onCreate?.Invoke(component);

            pipeline.AddDataCollector(component);

            return pipeline;
        }
    }
}
