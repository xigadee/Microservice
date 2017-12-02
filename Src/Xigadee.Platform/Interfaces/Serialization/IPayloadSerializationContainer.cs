using System;
namespace Xigadee
{
    /// <summary>
    /// This interface is used to expose the serialization container to applications 
    /// that require access to it.
    /// </summary>
    public interface IPayloadSerializationContainer
    {
        ///// <summary>
        ///// This method extracts the binary blob from the message and deserializes and returns the object.
        ///// </summary>
        ///// <param name="payload">The transmission payload.</param>
        ///// <returns>Returns the object deserialized from the binary blob.</returns>
        //object PayloadDeserialize(TransmissionPayload payload);

        ///// <summary>
        ///// This method tries to extract an entity from the message if present.
        ///// </summary>
        ///// <param name="payload">The transmission payload.</param>
        ///// <param name="entity">The deserialized entity</param>
        ///// <returns>Returns true if the message is present.</returns>
        //bool PayloadTryDeserialize(TransmissionPayload payload, out object entity);

        ///// <summary>
        ///// This method extracts the binary blob from the message and deserializes and returns the object.
        ///// </summary>
        ///// <typeparam name="P">The payload message type.</typeparam>
        ///// <param name="payload">The transmission payload.</param>
        ///// <returns>Returns the object deserialized from the binary blob.</returns>
        //P PayloadDeserialize<P>(TransmissionPayload payload);

        ///// <summary>
        ///// This method tries to extract an entity from the message if present.
        ///// </summary>
        ///// <typeparam name="P">The payload message type.</typeparam>
        ///// <param name="payload">The transmission payload.</param>
        ///// <param name="entity">The deserialized entity</param>
        ///// <returns>Returns true if the message is present.</returns>
        //bool PayloadTryDeserialize<P>(TransmissionPayload payload, out P entity);

        ///// <summary>
        ///// This method extracts the binary blob from the message and deserializes and returns the object.
        ///// </summary>
        ///// <typeparam name="P">The payload message type.</typeparam>
        ///// <param name="message">The service message.</param>
        ///// <returns>Returns the object deserialized from the binary blob.</returns>
        //P PayloadDeserialize<P>(ServiceMessage message);

        ///// <summary>
        ///// This method tries to extract an entity from the message if present.
        ///// </summary>
        ///// <typeparam name="P">The payload message type.</typeparam>
        ///// <param name="message">The service message.</param>
        ///// <param name="entity">The deserialized entity</param>
        ///// <returns>Returns true if the message is present.</returns>
        //bool PayloadTryDeserialize<P>(ServiceMessage message, out P entity);

        ///// <summary>
        ///// This method extracts the binary blob from the message and deserializes and returns the object.
        ///// </summary>
        ///// <param name="message">The service message.</param>
        ///// <returns>Returns the object deserialized from the binary blob.</returns>
        //object PayloadDeserialize(ServiceMessage message);

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

        ///// <summary>
        ///// This method tries to extract an entity from the message if present.
        ///// </summary>
        ///// <param name="message">The service message.</param>
        ///// <param name="entity">The deserialized entity</param>
        ///// <returns>Returns true if the message is present.</returns>
        //bool PayloadTryDeserialize(ServiceMessage message, out object entity);




        /// <summary>
        /// This method serializes the requestPayload object in to a binary blob using the 
        /// serializer collection.
        /// </summary>
        /// <param name="dto">The data transfer object to serialize.</param>
        /// <returns>Returns the binary blob object.</returns>
        [Obsolete("")]
        SerializationHolder PayloadSerialize(object dto);

        ///// <summary>
        ///// Payloads the register.
        ///// </summary>
        ///// <param name="message">The service message object to register the data transfer object.</param>
        ///// <param name="dto">The data transfer object to serialize.</param>
        ///// <param name="useObjectRegistry">if set to <c>true</c> the serializer will store a reference to the DTO in the object registry instead of explicitly serializing the object.</param>
        ///// <returns>Returns true if successful.</returns>
        //bool PayloadRegister(ServiceMessage message, object dto, bool useObjectRegistry = false);
    }
}
