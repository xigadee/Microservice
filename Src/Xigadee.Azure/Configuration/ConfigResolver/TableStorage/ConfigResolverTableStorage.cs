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
    /// This class implements a config resolver using Table storage as the persistence mechanism.
    /// </summary>
    public class ConfigResolverTableStorage : ConfigResolver
    {
        /// <summary>
        /// This is the cloud storage account used for all connectivity.
        /// </summary>
        protected CloudStorageAccount StorageAccount { get; set; }
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        protected CloudTableClient Client { get; set; }
        /// <summary>
        /// Gets or sets the table that contains the settings.
        /// </summary>
        protected CloudTable Table { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigResolverTableStorage"/> class.
        /// </summary>
        /// <param name="sasKey">The SAS key to access the table storage repository.</param>
        /// <param name="tableName">The optional name of the table. If unset or null, the value 'config' will be used instead.</param>
        public ConfigResolverTableStorage(string sasKey, string tableName = "config")
        {
            StorageAccount = CloudStorageAccount.Parse(sasKey);
            Client = StorageAccount.CreateCloudTableClient();
            Table = Client.GetTableReference(tableName);

            //if (!Table.Exists())
            //    return false;

        }

        public override bool CanResolve(string key)
        {

            //TableOperation insert = TableOperation.
            return false;
        }

        protected bool ResolveInternal(string key, out string value)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve("Settings", key);

            // Execute the retrieve operation.
            TableResult retrievedResult = Table.Execute(retrieveOperation);

            if (retrievedResult.Result == null)
            {
                value = null;
                return false;
            }

            value = (string)retrievedResult.Result;

            return true;
        }

        public override string Resolve(string key)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve("Settings", key);

            // Execute the retrieve operation.
            TableResult retrievedResult = Table.Execute(retrieveOperation);

            return null;
        }
    }
}
