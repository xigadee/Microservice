using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensionsCore
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
            pipeline.Service.ServiceHandlers.Serialization.Add(new JsonRawSerializer());

            return pipeline;
        }
    }
}
