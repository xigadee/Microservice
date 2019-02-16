using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// Adds the payload compressor.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="compressor">The compressor to add.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadCompressor<P>(this P pipeline, IServiceHandlerCompression compressor)
            where P : IPipeline
        {
            pipeline.Service.ServiceHandlers.Compression.Add(compressor);

            return pipeline;
        }


        /// <summary>
        /// Adds the payload compressor.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator">The compressor creator function.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadCompressor<P>(this P pipeline, Func<IEnvironmentConfiguration, IServiceHandlerCompression> creator)
            where P : IPipeline
        {
            var compressor = creator(pipeline.Configuration);
            pipeline.Service.ServiceHandlers.Compression.Add(compressor);

            return pipeline;
        }
    }
}
