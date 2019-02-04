using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace Xigadee
{
    public static partial class VisualStudioPipelineExtensions
    {
        /// <summary>
        /// The configuration resolver test context default prefix for keys
        /// </summary>
        public const string ConfigResolverTestContextDefault_Prefix = "CI_";
        /// <summary>
        /// The configuration resolver test context default priority setting of 10
        /// </summary>
        public const int ConfigResolverTestContextDefault_Priority = 10;
        /// <summary>
        /// The configuration resolver test context default throw exception policy on not found, which is false
        /// </summary>
        public const bool ConfigResolverTestContextDefault_ThrowExceptionOnNotFound = false;

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
        public static P ConfigResolverSetTestContext<P>(this P pipeline, TestContext context
            , int priority = ConfigResolverTestContextDefault_Priority
            , string prefix = ConfigResolverTestContextDefault_Prefix
            , bool throwExceptionOnNotFound = ConfigResolverTestContextDefault_ThrowExceptionOnNotFound
            , Action<ConfigResolverTestContext> assign = null)
            where P : IPipeline
        {
            ConfigResolverTestContext resolver;

            pipeline.ConfigResolverSetTestContext(context, out resolver, priority, prefix, throwExceptionOnNotFound, assign);

            return pipeline;
        }

        /// <summary>
        /// Sets a configuration resolver for the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="priority">The resolver priority.</param>
        /// <param name="context">The TestContext object.</param>
        /// <param name="resolver">The configuration resolver as an output parameter.</param>
        /// <param name="prefix">The settings key prefix. 'CI_' if left blank.</param>
        /// <param name="throwExceptionOnNotFound">if set to <c>true</c> [throw exception on not found].</param>
        /// <param name="assign">The pre-assignment action.</param>
        /// <returns>The pipeline.</returns>
        /// <exception cref="ArgumentNullException">The pipeline or context cannot be null</exception>
        public static P ConfigResolverSetTestContext<P>(this P pipeline, TestContext context, out ConfigResolverTestContext resolver
            , int priority = ConfigResolverTestContextDefault_Priority
            , string prefix = ConfigResolverTestContextDefault_Prefix
            , bool throwExceptionOnNotFound = ConfigResolverTestContextDefault_ThrowExceptionOnNotFound
            , Action<ConfigResolverTestContext> assign = null)
            where P : IPipeline
        {
            if (pipeline == null)
                throw new ArgumentNullException($"{nameof(ConfigResolverSetTestContext)}: pipeline cannot be null");
            if (context == null)
                throw new ArgumentNullException($"{nameof(ConfigResolverSetTestContext)}: context cannot be null");

            resolver = new ConfigResolverTestContext(context, prefix, throwExceptionOnNotFound);

            assign?.Invoke(resolver);

            pipeline.Configuration.ResolverSet(priority, resolver);

            return pipeline;
        }
    }
}
