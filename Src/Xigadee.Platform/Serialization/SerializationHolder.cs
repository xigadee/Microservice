using System;

namespace Xigadee
{
    /// <summary>
    /// This class holds the metadata for the service message blob.
    /// </summary>
    public class SerializationHolder
    {
        /// <summary>
        /// Gets or sets the BLOB.
        /// </summary>
        public byte[] Blob { get; set; }
        /// <summary>
        /// Gets or sets the BLOB serializer content type identifier. If this is set, the specific serializer will be used without attempting to identify the magic bytes at the start of the blob stream.
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Gets or sets the BLOB serializer content type identifier. If this is set, the specific serializer will be used without attempting to identify the magic bytes at the start of the blob stream.
        /// </summary>
        public string ContentEncoding { get; set; }
        /// <summary>
        /// Gets or sets the BLOB serializer content type identifier. If this is set, the specific serializer will be used without attempting to identify the magic bytes at the start of the blob stream.
        /// </summary>
        public string ContentTypeEncoding { get; set; }
        /// <summary>
        /// This optional identifier is added by the serialization container and specifies the id of the deserialized object stored in the object registry.
        /// </summary>
        public Guid? ObjectRegistryId { get; set; }
        /// <summary>
        /// Performs an implicit conversion from <see cref="SerializationHolder"/> to a byte array./>.
        /// </summary>
        /// <param name="blob">The byte array.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator byte[] (SerializationHolder blob)
        {
            return blob?.Blob;
        }
        /// <summary>
        /// Performs an implicit conversion from a byte array to <see cref="SerializationHolder"/>.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator SerializationHolder(byte[] blob)
        {
            return new SerializationHolder() { Blob = blob };
        }
    }
}
