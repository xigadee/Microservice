using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This is the root serialization interface based around the SerializationHolder object.
    /// </summary>
    public interface IPayloadSerializer
    {
        /// <summary>
        /// Gets the content-type parameter, which can be used to quickly identify the serializer used.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// Returns true if the holder can be deserialized.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if it can be deserialized.</returns>
        bool SupportsDeserialization(SerializationHolder holder);
        /// <summary>
        /// Returns true if the Content in the holder can be serialized.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if it can be serialized.</returns>
        bool SupportsSerialization(SerializationHolder holder);

        /// <summary>
        /// Tries to deserialize the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the incoming binary payload is successfully deserialized to a content object.</returns>
        bool TryDeserialize(SerializationHolder holder);
        /// <summary>
        /// Tries to serialize the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the Content is serialized correctly to a binary blob.</returns>
        bool TrySerialize(SerializationHolder holder);

         /// <summary>
        /// Returns true if the serializer supports this content type for serialization.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if supported.</returns>
        bool SupportsContentTypeSerialization(SerializationHolder holder);
        /// <summary>
        /// Returns true if the serializer supports this entity type for serialization.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns true if supported.</returns>
        bool SupportsContentTypeSerialization(Type entityType);

    }

    /// <summary>
    /// This is the interface used to serialize and deserialize payloads.
    /// </summary>
    //public interface IPayloadSerializerMagicBytes: IPayloadSerializer
    //{



    //    /// <summary>
    //    /// This method matches the incoming byte stream and identifies whether the serializer
    //    /// can deserialize on the basis of the index of the byte array.
    //    /// </summary>
    //    /// <param name="blob">The incoming byte array</param>
    //    /// <param name="start">The index point in the incoming byte array.</param>
    //    /// <param name="length">The count of the data in the byte array.</param>
    //    /// <returns>Returns true if it is a match.</returns>
    //    bool SupportsPayloadDeserialization(byte[] blob, int start, int length);
    //    /// <summary>
    //    /// Deserializes the specified binary blob.
    //    /// </summary>
    //    /// <typeparam name="E">The entity type.</typeparam>
    //    /// <param name="blob">The binary blob.</param>
    //    /// <returns>The deserialized entity.</returns>
    //    E Deserialize<E>(byte[] blob);
    //    /// <summary>
    //    /// Deserializes the specified binary blob.
    //    /// </summary>
    //    /// <typeparam name="E">The entity type.</typeparam>
    //    /// <param name="blob">The binary blob.</param>
    //    /// <param name="start">The array start.</param>
    //    /// <param name="length">The array length.</param>
    //    /// <returns>The deserialized entity.</returns>
    //    E Deserialize<E>(byte[] blob, int start, int length);
    //    /// <summary>
    //    /// Deserializes the specified binary blob.
    //    /// </summary>
    //    /// <param name="blob">The binary blob.</param>
    //    /// <returns>The deserialized entity.</returns>
    //    object Deserialize(byte[] blob);
    //    /// <summary>
    //    /// Deserializes the specified binary blob.
    //    /// </summary>
    //    /// <param name="blob">The binary blob.</param>
    //    /// <param name="start">The array start.</param>
    //    /// <param name="length">The array length.</param>
    //    /// <returns>The deserialized entity.</returns>
    //    object Deserialize(byte[] blob, int start, int length);
    //    /// <summary>
    //    /// Serializes the specified entity.
    //    /// </summary>
    //    /// <typeparam name="E">The entity type.</typeparam>
    //    /// <param name="entity">The entity.</param>
    //    /// <returns>The binary blob.</returns>
    //    byte[] Serialize<E>(E entity);
    //    /// <summary>
    //    /// Serializes the specified entity.
    //    /// </summary>
    //    /// <param name="entity">The entity.</param>
    //    /// <returns>The binary blob.</returns>
    //    byte[] Serialize(object entity);
    //}
}
