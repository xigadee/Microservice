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
        /// <param name="supportLegacy">This parameter specifies whether the JsonContractSerializer should be added to the collection. The default is true.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadSerializerDefaultJson<P>(this P pipeline, bool supportLegacy = true)
            where P : IPipeline
        {
            pipeline.Service.Serialization.RegisterPayloadSerializer(new JsonRawSerializer());
            if (supportLegacy)
                pipeline.Service.Serialization.RegisterPayloadSerializer(new JsonContractSerializer());

            return pipeline;
        }

    }
}
