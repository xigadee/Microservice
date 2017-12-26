using System;
namespace Xigadee
{
    /// <summary>
    /// This interface is used to expose the serialization container to applications that require access to it.
    /// </summary>
    public interface IPayloadSerializationContainer
    {
        /// <summary>
        /// Attempts to extract the data transfer object from the serialization holder, or the object registry.
        /// </summary>
        /// <typeparam name="P">The DTO entity type.</typeparam>
        /// <param name="holder">The incoming serialization holder.</param>
        /// <param name="dto">The data transfer object.</param>
        /// <param name="throwExceptions">Throws a detailed exception if deserialization fails and this property is set to true.</param>
        /// <param name="useContentRegistryIfSupported">Specifies whether to use the object registry if supported.</param>
        /// <returns>Returns true if the object has been successfully deserialized.</returns>
        bool DtoTryExtraction<P>(SerializationHolder holder, out P dto, bool throwExceptions = false, bool useContentRegistryIfSupported = true);

        /// <summary>
        /// Attempts to extract the data transfer object from the serialization holder, or the object registry.
        /// </summary>
        /// <param name="holder">The incoming serialization holder.</param>
        /// <param name="dto">The data transfer object.</param>
        /// <param name="throwExceptions">Throws a detailed exception if deserialization fails and this property is set to true.</param>
        /// <param name="useContentRegistryIfSupported">Specifies whether to use the object registry if supported.</param>
        /// <returns>Returns true if the object has been successfully deserialized.</returns>
        bool DtoTryExtraction(SerializationHolder holder, out object dto, bool throwExceptions = false, bool useContentRegistryIfSupported = true);

        /// <summary>
        /// Attempts to insert the entity in to a serialization holder or the object registry.
        /// </summary>
        /// <param name="dto">The data transfer object.</param>
        /// <param name="holder">The outgoing serialization holder.</param>
        /// <param name="throwExceptions">Throws a detailed exception if serialization fails and this property is set to true.</param>
        /// <param name="useContentRegistryIfSupported">Specifies whether to use the object registry if supported.</param>
        /// <returns>Returns true if successful.</returns>
        bool DtoTryInsertion(object dto, out SerializationHolder holder, bool throwExceptions = false, bool useContentRegistryIfSupported = true);

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
        /// This method serializes the requestPayload object in to a binary blob using the 
        /// serializer collection.
        /// </summary>
        /// <param name="dto">The data transfer object to serialize.</param>
        /// <returns>Returns the binary blob object.</returns>
        [Obsolete("")]
        SerializationHolder PayloadSerialize(object dto);
    }
}
