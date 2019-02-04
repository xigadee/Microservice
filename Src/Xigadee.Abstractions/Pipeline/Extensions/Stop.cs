namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This method stops the microservice defined in the pipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        public static void Stop(this IPipelineBase pipeline)
        {
            pipeline.ToMicroservice().Stop();
        }

    }
}
