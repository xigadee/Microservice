namespace Xigadee
{
    /// <summary>
    /// This class is used for configuration that extends the traditional pipeline.
    /// </summary>
    public abstract class MicroservicePipelineExtension<P>: IPipelineExtension<P>
        where P: IPipeline
    {
        /// <summary>
        /// This is the default constructor that sets the underlying pipeline.
        /// </summary>
        /// <param name="pipeline">The base pipeline.</param>
        protected MicroservicePipelineExtension(P pipeline)
        {
            Pipeline = pipeline;
        }

        /// <summary>
        /// This is the configuration pipeline.
        /// </summary>
        public P Pipeline { get; }
    }
}
