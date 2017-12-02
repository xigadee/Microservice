using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// Adds the default JSON payload serializer to the Microservice.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadSerializerDefaultJson<P>(this P pipeline)
            where P : IPipeline
        {
            var component = pipeline.Service.Serialization.RegisterPayloadSerializer(new JsonContractSerializer());

            return pipeline;
        }
    }
}
