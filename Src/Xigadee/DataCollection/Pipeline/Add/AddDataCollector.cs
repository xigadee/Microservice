using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method adds a DataCollector to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="collector">The collector to add.</param>
        /// <returns>The pipeline.</returns>
        public static P AddDataCollector<P>(this P pipeline
            , IDataCollectorComponent collector)
            where P:IPipeline
        {
            pipeline.Service.DataCollection.Register(collector);

            return pipeline;
        }
        /// <summary>
        /// This extension method adds a DataCollector to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <typeparam name="L">The collector type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator">The creator function.</param>
        /// <param name="action">The creation action.</param>
        /// <returns>The pipeline.</returns>
        public static P AddDataCollector<P,L>(this P pipeline
            , Func<IEnvironmentConfiguration, L> creator
            , Action<L> action = null)
            where P : IPipeline
            where L : IDataCollectorComponent
        {
            var collector = creator(pipeline.Configuration);

            action?.Invoke(collector);

            pipeline.Service.DataCollection.Register(collector);

            return pipeline;
        }
    }
}
