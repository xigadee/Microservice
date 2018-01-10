using System;
namespace Xigadee
{
    /// <summary>
    /// This interface is used to manage payload compression.
    /// </summary>
    public interface IPayloadCompressionContainer
    {
        /// <summary>
        /// A boolean function that returns true if the compression type is supported.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <returns>Returns true when supported.</returns>
        bool SupportsCompression(SerializationHolder holder);

        /// <summary>
        /// Tries to decompress the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the incoming binary payload is successfully decompressed.</returns>
        bool TryDecompression(SerializationHolder holder);
        /// <summary>
        /// Tries to compress the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the Content is compressed correctly to a binary blob.</returns>
        bool TryCompression(SerializationHolder holder);
    }

    /// <summary>
    /// This interface is used to expose the serialization container to applications that require access to it.
    /// </summary>
    public interface IPayloadSerializationContainer
    {
        /// <summary>
        /// Checks that a specific serializer is supported.
        /// </summary>
        /// <param name="mimetype">The mime type identifier for the serializer.</param>
        /// <returns>Returns true if the serializer is supported.</returns>
        bool SupportsSerializer(string mimetype);

        /// <summary>
        /// Gets or sets the default type of the content type. This is based on the first serializer added to the collection.
        /// </summary>
        string DefaultContentType{get;}

        /// <summary>
        /// This method deserializes the binary blob and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        P PayloadDeserialize<P>(SerializationHolder blob);

        /// <summary>
        /// This method deserializes the binary blob and returns the object.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        object PayloadDeserialize(SerializationHolder blob);

        /// <summary>
        /// This method attempts to Serialize the object and sets the blob and headers in the holder.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <returns>Returns true if the operation is successful.</returns>
        bool TryPayloadSerialize(SerializationHolder holder);

        /// <summary>
        /// This method attempts to deserialize the binary blob and sets the object in the holder.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <returns>Returns true if the operation is successful.</returns>
        bool TryPayloadDeserialize(SerializationHolder holder);

        /// <summary>
        /// This method serializes the requestPayload object in to a binary blob using the 
        /// serializer collection.
        /// </summary>
        /// <param name="dto">The data transfer object to serialize.</param>
        /// <returns>Returns the binary blob object.</returns>
        [Obsolete("")]
        SerializationHolder PayloadSerialize(object dto);
    }
}
