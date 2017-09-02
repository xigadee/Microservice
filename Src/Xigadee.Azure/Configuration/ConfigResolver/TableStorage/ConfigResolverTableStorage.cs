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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Xigadee
{
    /// <summary>
    /// This class implements a config resolver using Table storage as the persistence mechanism.
    /// </summary>
    public class ConfigResolverTableStorage : ConfigResolver
    {
        #region StorageAccount
        /// <summary>
        /// This is the cloud storage account used for all connectivity.
        /// </summary>
        protected CloudStorageAccount StorageAccount { get; set; }
        #endregion
        #region Client
        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        protected CloudTableClient Client { get; set; }
        #endregion
        #region Table
        /// <summary>
        /// Gets or sets the table that contains the settings.
        /// </summary>
        protected CloudTable Table { get; set; }
        #endregion
        #region PartitionKey
        /// <summary>
        /// Gets the partition key.
        /// </summary>
        public string PartitionKey { get; }
        #endregion
        #region TableName
        /// <summary>
        /// Gets the name of the table that the configuration settings will be retrieved from.
        /// </summary>
        public string TableName { get; } 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigResolverTableStorage"/> class.
        /// </summary>
        /// <param name="sasKey">The SAS key to access the table storage repository.</param>
        /// <param name="tableName">The optional name of the table. If unset or null, the value 'config' will be used instead.</param>
        public ConfigResolverTableStorage(string sasKey, string tableName = "Configuration", string partitionKey = "config")
            :this(CloudStorageAccount.Parse(sasKey), tableName, partitionKey)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigResolverTableStorage"/> class.
        /// </summary>
        /// <param name="storageAccount">The storage account.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The table partition key for the configuration settings.</param>
        public ConfigResolverTableStorage(CloudStorageAccount storageAccount, string tableName = "Configuration", string partitionKey = "config")
        {
            PartitionKey = partitionKey ?? "";
            TableName = tableName ?? throw new ArgumentNullException("tableName", $"{nameof(ConfigResolverTableStorage)} tableName cannot be null");

            StorageAccount = storageAccount ?? throw new ArgumentNullException("storageAccount", $"{nameof(ConfigResolverTableStorage)} storageAccount cannot be null"); 

            Client = StorageAccount.CreateCloudTableClient();
            Table = Client.GetTableReference(TableName);


        }

        #region CanResolve(string key)
        /// <summary>
        /// Use this method to check that the key exists doe the specific resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>
        /// Returns true if it can be resolved.
        /// </returns>
        public override bool CanResolve(string key)
        {

            //TableOperation insert = TableOperation.
            return false;
        }
        #endregion

        #region ResolveInternal(string key, out string value)
        /// <summary>
        /// Returns the key value from the resolver.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value as an out parameter.</param>
        /// <returns>Returns true if the value is resolved.</returns>
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
        #endregion

        #region Resolve(string key)
        /// <summary>
        /// Use this method to get the value from the specific resolver.
        /// </summary>
        /// <param name="key">The key to resolve</param>
        /// <returns>
        /// This is the settings value, null if not set.
        /// </returns>
        public override string Resolve(string key)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve("Settings", key);

            // Execute the retrieve operation.
            TableResult retrievedResult = Table.Execute(retrieveOperation);

            return null;
        } 
        #endregion
    }
}
