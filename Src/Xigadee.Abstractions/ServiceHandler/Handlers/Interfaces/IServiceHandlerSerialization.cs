using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the root serialization interface based around the SerializationHolder object.
    /// </summary>
    public interface IServiceHandlerSerialization: IServiceHandler
    {
        /// <summary>
        /// Returns true if the holder can be deserialized.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if it can be deserialized.</returns>
        bool SupportsDeserialization(ServiceHandlerContext holder);
        /// <summary>
        /// Returns true if the Content in the holder can be serialized.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if it can be serialized.</returns>
        bool SupportsSerialization(ServiceHandlerContext holder);

        /// <summary>
        /// Tries to deserialize the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the incoming binary payload is successfully deserialized to a content object.</returns>
        bool TryDeserialize(ServiceHandlerContext holder);
        /// <summary>
        /// Tries to serialize the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the Content is serialized correctly to a binary blob.</returns>
        bool TrySerialize(ServiceHandlerContext holder);

         /// <summary>
        /// Returns true if the serializer supports this content type for serialization.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if supported.</returns>
        bool SupportsContentTypeSerialization(ServiceHandlerContext holder);
        /// <summary>
        /// Returns true if the serializer supports this entity type for serialization.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns true if supported.</returns>
        bool SupportsContentTypeSerialization(Type entityType);

    }
}
