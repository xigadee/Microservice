namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method adds an override setting and clears the cache.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="key">The key.</param>
        /// <param name="value"></param>
        /// <returns>Returns the pipeline.</returns>
        public static P ConfigurationOverrideSet<P>(this P pipeline, string key, string value)
            where P : IPipeline
        {
            pipeline.Configuration.OverrideSettings.Add(key, value);
            pipeline.Configuration.CacheFlush();
            return pipeline;
        }
    }
}
