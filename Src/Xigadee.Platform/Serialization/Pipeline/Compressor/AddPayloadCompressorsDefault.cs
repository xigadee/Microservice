namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// Adds the default payload compressors (Deflate and Gzip) to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadCompressorsDefault<P>(this P pipeline) 
            where P : IPipeline
        {
            pipeline.Service.Serialization.RegisterPayloadCompressor(new PayloadCompressorDeflate());
            pipeline.Service.Serialization.RegisterPayloadCompressor(new PayloadCompressorGzip());

            return pipeline;
        }
    }
}
