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
        public static P AddPayloadSerializer<P>(this P pipeline, ISerializationSerializer serializer)
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
            , Func<IEnvironmentConfiguration, ISerializationSerializer> creator)
            where P : IPipeline
        {
            var serializer = creator(pipeline.Configuration);
            pipeline.AddPayloadSerializer(serializer);

            return pipeline;
        }

        /// <summary>
        /// Adds the payload serializer.
        /// </summary>
        /// <typeparam name="P">The pipeline type.</typeparam>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="mimeContentType">The MIME content type for the serializer.</param>
        /// <param name="serialize">The serialize action.</param>
        /// <param name="canSerialize">The serialize check function.</param>
        /// <param name="deserialize">The deserialize action.</param>
        /// <param name="canDeserialize">The deserialize check function.</param>
        /// <param name="supportsContentTypeSerialization">The function that checks the ContentType for serialization.</param>
        /// <param name="friendlyName">This is the friendly name of the serializer.</param>
        /// <returns>The pipeline.</returns>
        public static P AddPayloadSerializer<P>(this P pipeline
            , string mimeContentType
            , Action<ServiceHandlerContext> serialize = null
            , Func<ServiceHandlerContext, bool> canSerialize = null
            , Action<ServiceHandlerContext> deserialize = null
            , Func<ServiceHandlerContext, bool> canDeserialize = null
            , Func<Type, bool> supportsContentTypeSerialization = null
            , string friendlyName = null
            )
            where P : IPipeline
        {
            if (string.IsNullOrEmpty(mimeContentType))
                throw new ArgumentNullException("mimeContentType", "mimeContentType cannot be null or empty.");

            var serializer = new DynamicSerializer(mimeContentType, friendlyName, serialize, canSerialize, deserialize, canDeserialize, supportsContentTypeSerialization);

            pipeline.AddPayloadSerializer(serializer);

            return pipeline;
        }
    }
}
