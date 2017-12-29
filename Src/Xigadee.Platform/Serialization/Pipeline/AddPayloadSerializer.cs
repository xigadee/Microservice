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
        /// <param name="mimeContentType">The override for the mime content type.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadSerializer<P>(this P pipeline, IPayloadSerializer serializer
            , string mimeContentType = null)
            where P : IPipeline
        {
            if (mimeContentType != null)
                serializer.ContentType = mimeContentType;

            pipeline.Service.Serialization.RegisterPayloadSerializer(serializer);

            return pipeline;
        }


        /// <summary>
        /// Adds the payload serializer.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="creator">The serializer creator function.</param>
        /// <param name="mimeContentType">The override for the mime content type.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadSerializer<P>(this P pipeline
            , Func<IEnvironmentConfiguration, IPayloadSerializer> creator
            , string mimeContentType = null)
            where P : IPipeline
        {
            var serializer = creator(pipeline.Configuration);
            pipeline.AddPayloadSerializer(serializer, mimeContentType);

            return pipeline;
        }
    }
}
