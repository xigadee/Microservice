using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xigadee
{
    public static partial class VisualStudioPipelineExtensions
    {
        /// <summary>
        /// Sets a configuration resolver for the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="priority">The resolver priority.</param>
        /// <param name="context">The TestContext object.</param>
        /// <param name="prefix">The settings key prefix. 'CI_' if left blank.</param>
        /// <param name="throwExceptionOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        /// <param name="assign">The pre-assignment action.</param>
        /// <returns>The pipeline.</returns>
        /// <exception cref="ArgumentNullException">The pipeline cannot be null</exception>
        public static P ConfigResolverSetTestContext<P>(this P pipeline, int priority, TestContext context
            , string prefix = "CI_"
            , bool throwExceptionOnNotFound = false
            , Action<ConfigResolverTestContext> assign = null)
            where P : IPipeline
        {
            if (pipeline == null)
                throw new ArgumentNullException($"{nameof(ConfigResolverSetTestContext)}: pipeline cannot be null");
            if (context == null)
                throw new ArgumentNullException($"{nameof(ConfigResolverSetTestContext)}: context cannot be null");

            var resolver = new ConfigResolverTestContext(context, prefix, throwExceptionOnNotFound);

            assign?.Invoke(resolver);

            pipeline.Configuration.ResolverSet(priority, resolver);

            return pipeline;
        }
    }
}
