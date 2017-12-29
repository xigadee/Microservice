using System;

namespace Xigadee
{
    /// <summary>
    /// This class can be used to create a dynamic serializer through the Fluent configuration.
    /// </summary>
    /// <seealso cref="Xigadee.SerializerBase" />
    public class DynamicSerializer: SerializerBase
    {
        /// <summary>
        /// Gets or sets the serializer.
        /// </summary>
        public Action<SerializationHolder> Serializer { get; set; }
        /// <summary>
        /// Gets or sets the function that specifies whether the holder can be serialized.
        /// </summary>
        public Func<SerializationHolder, bool> FnCanSerialize { get; set; }
        /// <summary>
        /// Gets or sets the deserializer.
        /// </summary>
        public Action<SerializationHolder> Deserializer { get; set; }
        /// <summary>
        /// Gets or sets the function that specifies whether a holder can be deserialized.
        /// </summary>
        public Func<SerializationHolder, bool> FnCanDeserialize { get; set; }
        /// <summary>
        /// Gets or sets the function supports content type serialization.
        /// </summary>
        public Func<Type, bool> FnSupportsContentTypeSerialization { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSerializer"/> class.
        /// </summary>
        /// <param name="mimeContentType">Type of the MIME content.</param>
        /// <param name="serialize">The serialize.</param>
        /// <param name="canSerialize">The can serialize.</param>
        /// <param name="deserialize">The deserialize.</param>
        /// <param name="canDeserialize">The can deserialize.</param>
        /// <param name="supportsContentTypeSerialization">The supports content type serialization.</param>
        public DynamicSerializer(string mimeContentType = null
            , Action<SerializationHolder> serialize = null
            , Func<SerializationHolder, bool> canSerialize = null
            , Action<SerializationHolder> deserialize = null
            , Func<SerializationHolder, bool> canDeserialize = null
            , Func<Type, bool> supportsContentTypeSerialization = null
            )
        {
            ContentType = mimeContentType;
            Serializer = serialize;
            FnCanSerialize = canSerialize;
            Deserializer = deserialize;
            FnCanDeserialize = canDeserialize;
            FnSupportsContentTypeSerialization = supportsContentTypeSerialization;
        }

        /// <summary>
        /// Returns true if the holder can be deserialized.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if it can be deserialized.
        /// </returns>
        public override bool SupportsDeserialization(SerializationHolder holder)
        {
            return Deserializer != null 
                && (FnCanDeserialize?.Invoke(holder) ?? true) 
                && base.SupportsDeserialization(holder);
        }

        /// <summary>
        /// Returns true if the Content in the holder can be serialized.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if it can be serialized.
        /// </returns>
        public override bool SupportsSerialization(SerializationHolder holder)
        {
            return Serializer!=null
                && (FnCanSerialize?.Invoke(holder) ?? true)
                && base.SupportsSerialization(holder);
        }

        /// <summary>
        /// Deserializes the specified holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <exception cref="NotSupportedException">The deserialize action is not set.</exception>
        public override void Deserialize(SerializationHolder holder)
        {
            if (Deserializer == null)
                throw new NotSupportedException("The deserialize action is not set.");

            Deserializer(holder);
        }

        /// <summary>
        /// Serializes the specified holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <exception cref="NotSupportedException">The serialize action is not set.</exception>
        public override void Serialize(SerializationHolder holder)
        {
            if (Serializer == null)
                throw new NotSupportedException("The serialize action is not set.");

            Serializer(holder);
        }

        /// <summary>
        /// Returns true if the serializer supports this entity type for serialization.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>
        /// Returns true if supported.
        /// </returns>
        public override bool SupportsContentTypeSerialization(Type entityType)
        {
            return Serializer != null 
                && (FnSupportsContentTypeSerialization?.Invoke(entityType)?? false);
        }
    }
}
