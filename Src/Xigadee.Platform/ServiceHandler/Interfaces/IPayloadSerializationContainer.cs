using System;
namespace Xigadee
{
    /// <summary>
    /// This interface is used to expose the serialization container to applications that require access to it.
    /// </summary>
    public interface IPayloadSerializationContainer
    {
        #region Compressor
        /// <summary>
        /// A boolean function that returns true if the compression type is supported.
        /// </summary>
        /// <param name="contentEncodingType">The content encoding type, i.e. GZIP/DEFLATE etc..</param>
        /// <returns>Returns true when the ContentEncoding type is supported.</returns>
        bool SupportsCompressor(string contentEncodingType);
        /// <summary>
        /// A boolean function that returns true if the compression type is supported.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true when the holder ContentEncoding is supported.</returns>
        bool SupportsCompressor(ServiceHandlerContext holder);
        /// <summary>
        /// Tries to decompress the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the incoming binary payload is successfully decompressed.</returns>
        bool TryDecompress(ServiceHandlerContext holder);
        /// <summary>
        /// Tries to compress the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the Content is compressed correctly to a binary blob.</returns>
        bool TryCompress(ServiceHandlerContext holder); 
        #endregion

        /// <summary>
        /// Checks that a specific serializer is supported.
        /// </summary>
        /// <param name="mimetype">The mime type identifier for the serializer.</param>
        /// <returns>Returns true if the serializer is supported.</returns>
        bool SupportsSerializer(string mimetype);
        /// <summary>
        /// Checks that a specific serializer is supported.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true when the holder ContentType is supported.</returns>
        bool SupportsSerializer(ServiceHandlerContext holder);
        /// <summary>
        /// Tries to deserialize the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the incoming binary payload is successfully deserialized.</returns>
        bool TryDeserialize(ServiceHandlerContext holder);
        /// <summary>
        /// Tries to compress the outgoing holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>Returns true if the Content is serialized correctly to a binary blob.</returns>
        bool TrySerialize(ServiceHandlerContext holder);


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
        P PayloadDeserialize<P>(ServiceHandlerContext blob);

        /// <summary>
        /// This method deserializes the binary blob and returns the object.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        object PayloadDeserialize(ServiceHandlerContext blob);

        /// <summary>
        /// This method attempts to Serialize the object and sets the blob and headers in the holder.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <param name="throwExceptions">Directs the container to throw detailed exceptions on failure. The default is false.</param>
        /// <returns>Returns true if the operation is successful.</returns>
        bool TryPayloadSerialize(ServiceHandlerContext holder, bool throwExceptions = false);

        /// <summary>
        /// This method attempts to deserialize the binary blob and sets the object in the holder.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <param name="throwExceptions">Directs the container to throw detailed exceptions on failure. The default is false.</param>
        /// <returns>Returns true if the operation is successful.</returns>
        bool TryPayloadDeserialize(ServiceHandlerContext holder, bool throwExceptions = false);

        /// <summary>
        /// This method serializes the requestPayload object in to a binary blob using the 
        /// serializer collection.
        /// </summary>
        /// <param name="dto">The data transfer object to serialize.</param>
        /// <returns>Returns the binary blob object.</returns>
        [Obsolete("")]
        ServiceHandlerContext PayloadSerialize(object dto);
    }
}
