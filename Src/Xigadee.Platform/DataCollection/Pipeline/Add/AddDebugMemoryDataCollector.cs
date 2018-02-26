using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension method can be used to quickly add a debug memory based data collector.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="collector">The collector as an output parameter.</param>
        /// <param name="supportMap">The support map can be used to filter the types of events that you wish to filter. Leave this null to support all types.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddDebugMemoryDataCollector<P>(this P pipeline
            , out DebugMemoryDataCollector collector
            , DataCollectionSupport? supportMap = null)
            where P : IPipeline
        {
            DebugMemoryDataCollector collectorInt = null;
            pipeline.AddDataCollector((c) => collectorInt = new DebugMemoryDataCollector(supportMap));
            collector = collectorInt;
            return pipeline;
        }
        /// <summary>
        /// This extension method can be used to quickly add a debug memory based data collector.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="action">This action is called to allow the collector to be assigned externally.</param>
        /// <param name="supportMap">The support map can be used to filter the types of events that you wish to filter. Leave this null to support all types.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P AddDebugMemoryDataCollector<P>(this P pipeline
            , Action<DebugMemoryDataCollector> action
            , DataCollectionSupport? supportMap = null)
            where P : IPipeline
        {
            DebugMemoryDataCollector collector = new DebugMemoryDataCollector(supportMap);
            pipeline.AddDataCollector(collector);
            action?.Invoke(collector);
            return pipeline;
        }
    }
}
