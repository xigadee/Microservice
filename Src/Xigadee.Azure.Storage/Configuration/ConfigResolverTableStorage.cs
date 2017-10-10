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
using Microsoft.WindowsAzure.Storage.RetryPolicies;
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
        #region PropertyKey
        /// <summary>
        /// Gets the property key for the Table Storage Value. The default value key is 'Value'.
        /// </summary>
        public string PropertyKey { get; } 
        #endregion

        #region TableName
        /// <summary>
        /// Gets the name of the table that the configuration settings will be retrieved from.
        /// </summary>
        public string TableName { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigResolverTableStorage"/> class.
        /// </summary>
        /// <param name="sasKey">The SAS key to access the table storage repository.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The table partition key for the configuration settings.</param>
        /// <param name="propertyKey">The Azure table storage configuration default property key, currently "Value"</param>
        /// <param name="RequestOptionsDefault">You can set this to enable a more specific retry policy.</param>
        public ConfigResolverTableStorage(string sasKey
            , string tableName = AzureStorageExtensionMethods.AzureTableStorageConfigDefaultTableName
            , string partitionKey = AzureStorageExtensionMethods.AzureTableStorageConfigDefaultPartitionKey
            , string propertyKey = AzureStorageExtensionMethods.AzureTableStorageConfigDefaultPropertyKey
            , TableRequestOptions RequestOptionsDefault = null
            )
            : this(CloudStorageAccount.Parse(sasKey), tableName, partitionKey, propertyKey, RequestOptionsDefault)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigResolverTableStorage"/> class.
        /// </summary>
        /// <param name="storageAccount">The storage account.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The table partition key for the configuration settings.</param>
        /// <param name="propertyKey">The Azure table storage configuration default property key, currently "Value"</param>
        /// <param name="RequestOptionsDefault">You can set this to enable a more specific retry policy.</param>
        public ConfigResolverTableStorage(CloudStorageAccount storageAccount
            , string tableName = AzureStorageExtensionMethods.AzureTableStorageConfigDefaultTableName
            , string partitionKey = AzureStorageExtensionMethods.AzureTableStorageConfigDefaultPartitionKey
            , string propertyKey = AzureStorageExtensionMethods.AzureTableStorageConfigDefaultPropertyKey
            , TableRequestOptions RequestOptionsDefault = null
            )
        {
            PartitionKey = partitionKey ?? "";

            PropertyKey = propertyKey ?? throw new ArgumentNullException("propertyKey", $"{nameof(ConfigResolverTableStorage)} propertyKey cannot be null");
            TableName = tableName ?? throw new ArgumentNullException("tableName", $"{nameof(ConfigResolverTableStorage)} tableName cannot be null");
            StorageAccount = storageAccount ?? throw new ArgumentNullException("storageAccount", $"{nameof(ConfigResolverTableStorage)} storageAccount cannot be null");

            Client = StorageAccount.CreateCloudTableClient();

            Client.DefaultRequestOptions = RequestOptionsDefault ?? DefaultTableRequestOptions();

            Table = Client.GetTableReference(TableName);
        }
        #endregion

        #region DefaultTableRequestOptions()
        /// <summary>
        /// This method returns the default table request options.
        /// </summary>
        /// <returns>A TableRequestOptions object.</returns>
        protected virtual TableRequestOptions DefaultTableRequestOptions()
        {
            return new TableRequestOptions()
            {
                  RetryPolicy = new LinearRetry(TimeSpan.FromMilliseconds(200), 5)
                , ServerTimeout = TimeSpan.FromSeconds(1)
            };
        } 
        #endregion

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
            //Not the most efficient, but bear in mind these are cached after being read.
            string value;
            return ResolveInternal(key, out value);
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
            value = null;

            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve(PartitionKey, key);

            // Execute the retrieve operation.
            TableResult retrievedResult = Table.ExecuteAsync(retrieveOperation).Result;

            var result = retrievedResult?.Result as DynamicTableEntity;

            if (result == null)
                return false;

            value = result.Properties[PropertyKey].StringValue;

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
            string value = null;

            ResolveInternal(key, out value);

            return value;
        } 
        #endregion
    }
}
