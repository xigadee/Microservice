#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This container holds the system serialization/deserialization components that are used when transmitting data outside of the system.
    /// </summary>
    public class SerializationContainer : CollectionContainerBase<IPayloadSerializer>, IPayloadSerializationContainer
    {
        #region Constructor
        /// <summary>
        /// This default constrcutor takes the list of registered serializers.
        /// </summary>
        /// <param name="collection">The collection of serializers</param>
        public SerializationContainer(IEnumerable<IPayloadSerializer> collection)
            : base(collection)
        {
        } 
        #endregion

        #region PayloadDeserialize<P>(ServiceMessage message)
        /// <summary>
        /// This method extracts the binary blob from the message and deserializes it in to the 
        /// specific message channelId.
        /// </summary>
        /// <typeparam name="P">The requestPayload message channelId.</typeparam>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public P PayloadDeserialize<P>(TransmissionPayload payload)
        {
            try
            {
                if (payload.Message.Blob == null || Count == 0)
                    return default(P);

                var serializer = Items.FirstOrDefault(s => s.SupportsPayloadDeserialization(payload.Message.Blob));
                if (serializer != null)
                    return serializer.Deserialize<P>(payload.Message.Blob);

                return default(P);
            }
            catch (Exception ex)
            {
                throw new PayloadSerializationException(payload.Message.OriginatorKey, ex);
            }
        }
        #endregion
        #region PayloadDeserialize(ServiceMessage message)
        /// <summary>
        /// This method extracts the binary blob from the message and deserializes it.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public object PayloadDeserialize(ServiceMessage message)
        {
            try
            {
                if (message.Blob == null || Count == 0)
                    return null;

                var serializer = Items.FirstOrDefault(s => s.SupportsPayloadDeserialization(message.Blob));
                if (serializer != null)
                    return serializer.Deserialize(message.Blob);

                return null;
            }
            catch (Exception ex)
            {
                throw new PayloadDeserializationException(message.OriginatorKey, ex);
            }
        }
        #endregion

        #region PayloadSerialize(object requestPayload)
        /// <summary>
        /// This method serializes the requestPayload object in to a binary blob using the 
        /// serializer collection.
        /// </summary>
        /// <param name="requestPayload">The requestPayload to serialize.</param>
        /// <returns>Returns the binary blob object.</returns>
        public byte[] PayloadSerialize(object payload)
        {
            if (payload == null)
                return null;

            var serializer = Items.FirstOrDefault(s => s.SupportsObjectTypeSerialization(payload.GetType()));
            if (serializer != null)
                return serializer.Serialize(payload);

            throw new PayloadTypeSerializationNotSupportedException(payload.GetType().AssemblyQualifiedName);
        }
        #endregion


        protected override void StartInternal()
        {
            if (Count == 0)
                throw new PayloadSerializerCollectionIsEmptyException();
        }
    }
}
