namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method clears all the registered ConfigResolvers in the configuration.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>Returns the pipeline.</returns>
        public static P ConfigResolversClear<P>(this P pipeline)
            where P : IPipeline
        {
            pipeline.Configuration.ResolversClear();

            return pipeline;
        }
    }
}
