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
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Xigadee
{
    /// <summary>
    /// This class implements a config resolver using Table storage as the settings mechanism.
    /// </summary>
    public class ConfigResolverTableStorage : ConfigResolver
    {
        /// <summary>
        /// This is the cloud storage account used for all connectivity.
        /// </summary>
        public CloudStorageAccount StorageAccount { get; set; }

        public CloudTableClient Client { get; set; }

        public CloudTable Table { get; set; }

        public ConfigResolverTableStorage(string sasKey)
        {

        }

        public override bool CanResolve(string key)
        {
            return false;
        }

        public override string Resolve(string key)
        {
            throw new NotImplementedException();
        }
    }
}
