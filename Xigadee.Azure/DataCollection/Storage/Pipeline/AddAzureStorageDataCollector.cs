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
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static partial class AzureExtensionMethods
    {

        public static P AddAzureStorageDataCollector<P>(this P pipeline
            , StorageCredentials creds = null
            , Action<AzureStorageDataCollectorPolicy> adjustPolicy = null
            , ResourceProfile resourceProfile = null
            , IEncryptionHandler encryption = null
            , Action<AzureStorageDataCollector> onCreate = null)
            where P : IPipeline
        {
            AzureStorageDataCollectorPolicy policy = new AzureStorageDataCollectorPolicy();
            adjustPolicy?.Invoke(policy);

            if (creds == null)
                creds = pipeline.Configuration.AzureStorageCredentials(true);

            var component = new AzureStorageDataCollector(creds, policy);//, serviceName ?? service.Name, containerName, resourceProfile, encryption);

            onCreate?.Invoke(component);

            pipeline.AddDataCollector(component);

            return pipeline;
        }
    }
}
