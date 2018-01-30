using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is used by serializers that use a magic byte marker at the start of the serialized byte array.
    /// </summary>
    public interface ISystemHandlerSerializationMagicBytes
    {
        /// <summary>
        /// Converts the identifier to a hex string.
        /// </summary>
        /// <returns>A hex string.</returns>
        string ToIdentifier();
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
        /// This method matches the incoming byte stream and identifies whether the serializer
        /// can deserialize on the basis of the index of the byte array.
        /// </summary>
        /// <param name="blob">The incoming byte array</param>
        /// <returns>Returns true if it is a match.</returns>
        bool SupportsPayloadDeserialization(byte[] blob);
    }
}
