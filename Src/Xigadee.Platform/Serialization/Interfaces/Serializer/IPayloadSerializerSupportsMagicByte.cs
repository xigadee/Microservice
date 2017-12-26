using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by legacy serializers that do not pass a content-type but prepend two "magic" bytes to the serialization byte array 
    /// to identify the serialized content.
    /// </summary>
    public interface IPayloadSerializerSupportsMagicByte
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
}
