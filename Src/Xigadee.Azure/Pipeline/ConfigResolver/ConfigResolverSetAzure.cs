using System;

namespace Xigadee
{
    /// <summary>
    /// These extension methods connect the service bus listeners in to the pipeline.
    /// </summary>
    public static partial class AzureExtensionMethods
    {
        /// <summary>
        /// This extension method sets the service to use the CloudConfigurationManager at priority 20 by default.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline</param>
        /// <param name="priority">The optional priority, by default set to 20.</param>
        /// <param name="assign">An action to allow changes to be made to the resolver after it is created.</param>
        /// <returns>Returns the pipeline to continue the chain.</returns>
        public static P ConfigResolverSetAzure<P>(this P pipeline, int priority = 20, Action<ConfigResolverAzure> assign = null)
            where P : IPipeline
        {
            pipeline.ConfigResolverSet<P, ConfigResolverAzure>(priority, assign);

            return pipeline;
        }
    }
}
