using System;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// Adds the payload serializer.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="serializer">The serializer to add.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadSerializer<P>(this P pipeline, IPayloadSerializer serializer)
            where P : IPipeline
        {
            pipeline.Service.Serialization.RegisterPayloadSerializer(serializer);

            return pipeline;
        }
        /// <summary>
        /// Adds the payload serializer.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator">The serializer creator function.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadSerializer<P>(this P pipeline
            , Func<IEnvironmentConfiguration, IPayloadSerializer> creator)
            where P : IPipeline
        {
            pipeline.Service.Serialization.RegisterPayloadSerializer(creator(pipeline.Configuration));

            return pipeline;
        }
    }
}
