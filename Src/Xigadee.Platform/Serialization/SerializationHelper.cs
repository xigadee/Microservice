using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This helper class provides extended functionality for the serialization interface.
    /// </summary>
    public static class SerializationHelper
    {
        #region TransmissionPayload ...
        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <param name="srz">The serialization container.</param>
        /// <param name="payload">The transmission payload.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public static object PayloadDeserialize(this IPayloadSerializationContainer srz, TransmissionPayload payload)
        {
            return srz.PayloadDeserialize(payload.Message);
        }
        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="srz">The serialization container.</param>
        /// <param name="payload">The transmission payload.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public static P PayloadDeserialize<P>(this IPayloadSerializationContainer srz, TransmissionPayload payload)
        {
            return srz.PayloadDeserialize<P>(payload.Message);
        }
        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <param name="srz">The serialization container.</param>
        /// <param name="payload">The transmission payload.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        public static bool PayloadTryDeserialize(this IPayloadSerializationContainer srz, TransmissionPayload payload, out object entity)
        {
            entity = null;
            if (payload?.Message?.Blob == null)
                return false;

            entity = srz.PayloadDeserialize(payload);

            return true;
        }
        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="srz">The serialization container.</param>
        /// <param name="payload">The transmission payload.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        public static bool PayloadTryDeserialize<P>(this IPayloadSerializationContainer srz, TransmissionPayload payload, out P entity)
        {
            entity = default(P);
            if (payload?.Message?.Blob == null)
                return false;

            entity = srz.PayloadDeserialize<P>(payload);

            return true;
        }

        #endregion


        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="srz">The serialization container.</param>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public static P PayloadDeserialize<P>(this IPayloadSerializationContainer srz, ServiceMessage message)
        {
            try
            {
                return srz.PayloadDeserialize<P>(message.Blob);
            }
            catch (Exception ex)
            {
                throw new PayloadSerializationException(message.OriginatorKey, ex);
            }
        }

        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <param name="srz">The serialization container.</param>
        /// <param name="message">The service message.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        public static bool PayloadTryDeserialize(this IPayloadSerializationContainer srz, ServiceMessage message, out object entity)
        {
            entity = null;
            if (message?.Blob == null)
                return false;

            entity = srz.PayloadDeserialize(message);

            return true;
        }
        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="srz">The serialization container.</param>
        /// <param name="message">The service message.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        public static bool PayloadTryDeserialize<P>(this IPayloadSerializationContainer srz, ServiceMessage message, out P entity)
        {
            entity = default(P);
            if (message?.Blob == null)
                return false;

            entity = srz.PayloadDeserialize<P>(message);

            return true;
        }

        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <param name="srz">The serialization container.</param>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public static object PayloadDeserialize(this IPayloadSerializationContainer srz, ServiceMessage message)
        {
            try
            {
                if (message.Blob?.Blob == null)
                    return null;

                return srz.PayloadDeserialize(message.Blob);
            }
            catch (Exception ex)
            {
                throw new PayloadDeserializationException(message.OriginatorKey, ex);
            }
        }
    }
}
