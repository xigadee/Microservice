using System;
using System.Collections.Generic;

namespace Xigadee
{
    public interface IPayloadSerializerMagicBytes
    {
        /// <summary>
        /// This is the byte header for the serialization payload.
        /// </summary>
        byte[] Identifier { get; }
        /// <summary>
        /// This is the collection of byte magic numbers the byte array will index with,
        /// </summary>
        /// <returns>A collection of 2 byte arrays.</returns>
        IEnumerable<byte[]> PayloadMagicNumbers();
    }
    /// <summary>
    /// This is the interface used to serialize and deserialize payloads.
    /// </summary>
    public interface IPayloadSerializer
    {
        /// <summary>
        /// Gets the content-type parameter, which can be used to quickly identify the serializer used.
        /// </summary>
        string ContentType { get; }
        /// <summary>
        /// This is the byte header for the serialization payload.
        /// </summary>
        byte[] Identifier { get; }
        /// <summary>
        /// This is the collection of byte magic numbers the byte array will index with,
        /// </summary>
        /// <returns>A collection of 2 byte arrays.</returns>
        IEnumerable<byte[]> PayloadMagicNumbers();
        /// <summary>
        /// Returns true of the serializer supports this object channelId.
        /// </summary>
        /// <param name="entityType">The object channelId.</param>
        /// <returns>Returns true.</returns>
        bool SupportsObjectTypeSerialization(Type entityType);
        /// <summary>
        /// This method matches the incoming byte stream and identifies whether the serializer
        /// can deserialize on the basis of the index of the byte array.
        /// </summary>
        /// <param name="blob">The incoming byte array</param>
        /// <returns>Returns true if it is a match.</returns>
        bool SupportsPayloadDeserialization(byte[] blob);
        /// <summary>
        /// This method matches the incoming byte stream and identifies whether the serializer
        /// can deserialize on the basis of the index of the byte array.
        /// </summary>
        /// <param name="blob">The incoming byte array</param>
        /// <param name="start">The index point in the incoming byte array.</param>
        /// <param name="length">The count of the data in the byte array.</param>
        /// <returns>Returns true if it is a match.</returns>
        bool SupportsPayloadDeserialization(byte[] blob, int start, int length);
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <returns>The deserialized entity.</returns>
        E Deserialize<E>(byte[] blob);
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <param name="start">The array start.</param>
        /// <param name="length">The array length.</param>
        /// <returns>The deserialized entity.</returns>
        E Deserialize<E>(byte[] blob, int start, int length);
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <returns>The deserialized entity.</returns>
        object Deserialize(byte[] blob);
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <param name="start">The array start.</param>
        /// <param name="length">The array length.</param>
        /// <returns>The deserialized entity.</returns>
        object Deserialize(byte[] blob, int start, int length);
        /// <summary>
        /// Serializes the specified entity.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The binary blob.</returns>
        byte[] Serialize<E>(E entity);
        /// <summary>
        /// Serializes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The binary blob.</returns>
        byte[] Serialize(object entity);
    }
}
