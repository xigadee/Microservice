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
        public static P AddPayloadCompressor<P>(this P pipeline, ISerializationCompressor compressor)
            where P : IPipeline
        {
            pipeline.Service.Serialization.RegisterPayloadCompressor(compressor);

            return pipeline;
        }


        /// <summary>
        /// Adds the payload compressor.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator">The compressor creator function.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadCompressor<P>(this P pipeline
            , Func<IEnvironmentConfiguration, ISerializationCompressor> creator)
            where P : IPipeline
        {
            var serializer = creator(pipeline.Configuration);
            pipeline.AddPayloadCompressor(serializer);

            return pipeline;
        }
    }
}
