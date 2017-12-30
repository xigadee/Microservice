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
        /// <returns>The pipeline.</returns>
        public static P AddPayloadSerializer<P>(this P pipeline
            , string mimeContentType
            , Action<SerializationHolder> serialize = null
            , Func<SerializationHolder, bool> canSerialize = null
            , Action<SerializationHolder> deserialize = null
            , Func<SerializationHolder, bool> canDeserialize = null
            , Func<Type, bool> supportsContentTypeSerialization = null)
            where P : IPipeline
        {
            if (string.IsNullOrEmpty(mimeContentType))
                throw new ArgumentNullException("mimeContentType", "mimeContentType cannot be null or empty.");

            var serializer = new DynamicSerializer(mimeContentType, serialize, canSerialize, deserialize, canDeserialize, supportsContentTypeSerialization);

            pipeline.AddPayloadSerializer(serializer, mimeContentType);

            return pipeline;
        }
    }
}
