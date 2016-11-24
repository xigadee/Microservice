using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Xigadee
{
    /// <summary>
    /// These extension methods configure the configuration management to use Key Vault in Azure.
    /// </summary>
    public static partial class AzureExtensionMethods
    {
        /// <summary>
        /// This extension method sets the service to use the KeyVaultResolver at priority 30 by default.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="priority">The optional priority, by default set to 30.</param>
        /// <param name="assign">An action to allow changes to be made to the resolver after it is created.</param>
        /// <returns>Returns the pipeline to continue the chain.</returns>
        public static P ConfigResolverSetKeyVault<P>(this P pipeline, int priority = 30, Action<ConfigResolver> assign = null) where P : MicroservicePipeline
        {
            return pipeline.ConfigResolverSetKeyVault(pipeline.Configuration.KeyVaultClientCredential(), pipeline.Configuration.KeyVaultSecretBaseUri(), priority, assign);
        }

        /// <summary>
        /// This extension method sets the service to use the KeyVaultResolver at priority 30 by default.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="clientCredential">Key Vault Client Credentials</param>
        /// <param name="secretBaseUri">Key Vault Secret Base Uri</param>
        /// <param name="priority">The optional priority, by default set to 30.</param>
        /// <param name="assign">An action to allow changes to be made to the resolver after it is created.</param>
        /// <returns>Returns the pipeline to continue the chain.</returns>
        public static P ConfigResolverSetKeyVault<P>(this P pipeline, ClientCredential clientCredential, string secretBaseUri, int priority = 30, Action<ConfigResolver> assign = null) where P : MicroservicePipeline
        {
            pipeline.ConfigResolverSet(priority, new ConfigResolverKeyVault(clientCredential, secretBaseUri), assign);
            return pipeline;
        }
    }
}
