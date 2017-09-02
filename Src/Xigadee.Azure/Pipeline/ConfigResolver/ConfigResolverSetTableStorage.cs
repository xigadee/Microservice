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
        /// <returns>Returns the pipeline to continue the chain.</returns>
        public static P ConfigResolverSetTableStorage<P>(this P pipeline, out ConfigResolverTableStorage resolver, int priority = 10)
            where P : IPipeline
        {
            //var credentials = pipeline.Configuration.KeyVaultClientCredential();
            //var credsecret = pipeline.Configuration.KeyVaultSecretBaseUri();

            ConfigResolverTableStorage newResolver = null;
            //Action<ConfigResolverKeyVault> assign = (r) => newResolver = r;

            //var pipe = pipeline.ConfigResolverSetKeyVault(credentials, credsecret, priority, assign);

            resolver = newResolver;

            return pipeline;
        }

        /// <summary>
        /// This extension method sets the service to use the KeyVaultResolver at priority 30 by default.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="priority">The optional priority, by default set to 10.</param>
        /// <param name="assign">An action to allow changes to be made to the resolver after it is created.</param>
        /// <returns>Returns the pipeline to continue the chain.</returns>
        public static P ConfigResolverSetTableStorage<P>(this P pipeline, int priority = 10, Action<ConfigResolverTableStorage> assign = null)
            where P : IPipeline
        {
            //var credentials = pipeline.Configuration.KeyVaultClientCredential();
            //var credsecret = pipeline.Configuration.KeyVaultSecretBaseUri();
            //pipeline.ConfigResolverSetKeyVault(credentials, credsecret, priority, assign);

            return pipeline;
        }

        ///// <summary>
        ///// This extension method sets the service to use the KeyVaultResolver at priority 30 by default.
        ///// </summary>
        ///// <typeparam name="P">The pipeline type.</typeparam>
        ///// <param name="pipeline">The pipeline</param>
        ///// <param name="clientCredential">Key Vault Client Credentials</param>
        ///// <param name="secretBaseUri">Key Vault Secret Base Uri</param>
        ///// <param name="priority">The optional priority, by default set to 10.</param>
        ///// <param name="assign">An action to allow changes to be made to the resolver after it is created.</param>
        ///// <returns>Returns the pipeline to continue the chain.</returns>
        //public static P ConfigResolverSetTableStorage<P>(this P pipeline, ClientCredential clientCredential, string secretBaseUri, int priority = 10, Action<ConfigResolverTableStorage> assign = null)
        //    where P : IPipeline
        //{
        //    var resolver = new ConfigResolverKeyVault(clientCredential, secretBaseUri);
        //    assign?.Invoke(resolver);
        //    pipeline.ConfigResolverSet(priority, resolver);
        //    return pipeline;
        //}
    }
}
