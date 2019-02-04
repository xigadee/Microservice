namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
    {
        /// <summary>
        /// This method starts the microservice defined in the pipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        public static void Start(this IPipelineBase pipeline)
        {
            pipeline.ToMicroservice().Start();
        }

    }
}
