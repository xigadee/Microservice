using System;
using Xigadee;

namespace Xigadee
{
    public static partial class AzureExtensionMethods
    {
        /// <summary>
        /// This extension method sets the service to use the KeyVaultResolver at priority 30 by default.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="resolver">The new resolver as an out parameter.</param>
        /// <param name="priority">The optional priority, by default set to 10.</param>
        /// <param name="assign">The pre-assignment method.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The table partition key for the configuration settings.</param>
        /// <returns>Returns the pipeline to continue the chain.</returns>
        public static P ConfigResolverSetTableStorage<P>(this P pipeline, out ConfigResolverTableStorage resolver
            , int priority = AzureTableStorageConfigDefaultPriority
            , Action<ConfigResolverTableStorage> assign = null
            , string tableName = AzureTableStorageConfigDefaultTableName
            , string partitionKey = AzureTableStorageConfigDefaultPartitionKey
            )
            where P : IPipeline
        {
            var sasKey = pipeline.Configuration.AzureTableStorageConfigSASKey();

            resolver = new ConfigResolverTableStorage(sasKey, tableName, partitionKey);

            assign?.Invoke(resolver);

            pipeline.ConfigResolverSet(priority, resolver);

            return pipeline;
        }

        /// <summary>
        /// This extension method sets the service to use the KeyVaultResolver at priority 30 by default.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="priority">The optional priority, by default set to 10.</param>
        /// <param name="assign">The pre-assignment method.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="partitionKey">The table partition key for the configuration settings.</param>
        /// <returns>Returns the pipeline to continue the chain.</returns>
        public static P ConfigResolverSetTableStorage<P>(this P pipeline
            , int priority = AzureTableStorageConfigDefaultPriority
            , Action<ConfigResolverTableStorage> assign = null
            , string tableName = AzureTableStorageConfigDefaultTableName
            , string partitionKey = AzureTableStorageConfigDefaultPartitionKey
            )
            where P : IPipeline
        {
            ConfigResolverTableStorage resolver;

            pipeline.ConfigResolverSetTableStorage(out resolver, priority, assign, tableName, partitionKey);

            return pipeline;
        }

    }
}
