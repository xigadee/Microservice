#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region using
using System;
using System.Collections.Concurrent;
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
    public class SerializationContainer : ServiceContainerBase<SerializationStatistics, SerializationPolicy>
        , IPayloadSerializationContainer
    {
        #region Declarations
        /// <summary>
        /// This contains the supported serializers.
        /// </summary>
        protected Dictionary<byte[], IPayloadSerializer> mPayloadSerializers;
        /// <summary>
        /// This is the look up cache for the specific type.
        /// </summary>
        protected ConcurrentDictionary<Type, IPayloadSerializer> mLookUpCache;
        #endregion
        #region Constructor
        /// <summary>
        /// This default constrcutor takes the list of registered serializers.
        /// </summary>
        /// <param name="policy">The collection of serializers</param>
        public SerializationContainer(SerializationPolicy policy = null)
            : base(policy)
        {
            mPayloadSerializers = new Dictionary<byte[], Xigadee.IPayloadSerializer>();
        }
        #endregion

        #region StatisticsRecalculate(SerializationStatistics statistics)
        /// <summary>
        /// This method is used to update any calculated fields for the specific service statistics.
        /// </summary>
        /// <param name="statistics">The current statistics.</param>
        protected override void StatisticsRecalculate(SerializationStatistics statistics)
        {
            base.StatisticsRecalculate(statistics);

            try
            {
                statistics.ItemCount = mPayloadSerializers?.Count ?? 0;
                statistics.CacheCount = mLookUpCache?.Count ?? 0;

                statistics.Serialization = mPayloadSerializers?.Select((c) => $"{BitConverter.ToString(c.Key)}: {c.Value.GetType().Name}").ToArray();
            }
            catch (Exception)
            {
            }
        } 
        #endregion

        #region StartInternal()
        /// <summary>
        /// This override checks whether there is a default serialization container set.
        /// </summary>
        protected override void StartInternal()
        {
            if (mPayloadSerializers.Count == 0)
                throw new PayloadSerializerCollectionIsEmptyException();

            mLookUpCache = new ConcurrentDictionary<Type, Xigadee.IPayloadSerializer>();
        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This method clears the container.
        /// </summary>
        protected override void StopInternal()
        {
            mLookUpCache.Clear();
            mPayloadSerializers.Clear();
        }
        #endregion

        #region Add/Clear
        /// <summary>
        /// This method adds the serializer to the collection.
        /// </summary>
        /// <param name="serializer">The serializer to add.</param>
        public void Add(IPayloadSerializer serializer)
        {
            mPayloadSerializers.Add(serializer.Identifier, serializer);
        }
        /// <summary>
        /// This method clears all the serliazers currently registered.
        /// </summary>
        public void Clear()
        {
            mPayloadSerializers.Clear();
        }
        #endregion

        #region PayloadDeserialize...
        #region TransmissionPayload ...
        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <param name="payload">The transmission payload.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public object PayloadDeserialize(TransmissionPayload payload)
        {
            return PayloadDeserialize(payload.Message);
        }
        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="payload">The transmission payload.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public P PayloadDeserialize<P>(TransmissionPayload payload)
        {
            return PayloadDeserialize<P>(payload.Message);
        }
        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <param name="payload">The transmission payload.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        public bool PayloadTryDeserialize(TransmissionPayload payload, out object entity)
        {
            entity = null;
            if (payload?.Message?.Blob == null)
                return false;

            entity = PayloadDeserialize(payload);

            return true;
        }
        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="payload">The transmission payload.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        public bool PayloadTryDeserialize<P>(TransmissionPayload payload, out P entity)
        {
            entity = default(P);
            if (payload?.Message?.Blob == null)
                return false;

            entity = PayloadDeserialize<P>(payload);

            return true;
        }

        #endregion

        #region ServiceMessage ...
        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public object PayloadDeserialize(ServiceMessage message)
        {
            try
            {
                if (message.Blob == null || mPayloadSerializers.Count == 0)
                    return null;

                var serializer = mPayloadSerializers.Values.FirstOrDefault(s => s.SupportsPayloadDeserialization(message.Blob));

                if (serializer != null)
                    return serializer.Deserialize(message.Blob);

                return null;
            }
            catch (Exception ex)
            {
                throw new PayloadDeserializationException(message.OriginatorKey, ex);
            }
        }

        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public P PayloadDeserialize<P>(ServiceMessage message)
        {
            try
            {
                return PayloadDeserialize<P>(message.Blob);
            }
            catch (Exception ex)
            {
                throw new PayloadSerializationException(message.OriginatorKey, ex);
            }
        }

        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        public bool PayloadTryDeserialize(ServiceMessage message, out object entity)
        {
            entity = null;
            if (message?.Blob == null)
                return false;

            entity = PayloadDeserialize(message);

            return true;
        }
        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="message">The service message.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        public bool PayloadTryDeserialize<P>(ServiceMessage message, out P entity)
        {
            entity = default(P);
            if (message?.Blob == null)
                return false;

            entity = PayloadDeserialize<P>(message);

            return true;
        }
        #endregion

        #region byte[]...
        /// <summary>
        /// This method deserializes the binary blob and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public P PayloadDeserialize<P>(byte[] blob)
        {
            if (blob == null || mPayloadSerializers.Count == 0)
                return default(P);

            var serializer = mPayloadSerializers.Values.FirstOrDefault(s => s.SupportsPayloadDeserialization(blob));

            if (serializer != null)
                return serializer.Deserialize<P>(blob);

            return default(P);
        }

        /// <summary>
        /// This method deserializes the binary blob and returns the object.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public object PayloadDeserialize(byte[] blob)
        {
            if (blob == null || mPayloadSerializers.Count == 0)
                return null;

            var serializer = mPayloadSerializers.Values.FirstOrDefault(s => s.SupportsPayloadDeserialization(blob));

            if (serializer != null)
                return serializer.Deserialize(blob);

            return null;
        } 
        #endregion
        #endregion

        #region PayloadSerialize(object requestPayload)
        /// <summary>
        /// This method serializes the requestPayload object in to a binary blob using the 
        /// serializer collection.
        /// </summary>
        /// <param name="payload">The requestPayload to serialize.</param>
        /// <returns>Returns the binary blob object.</returns>
        public byte[] PayloadSerialize(object payload)
        {
            if (payload == null)
                return null;

            var serializer = mPayloadSerializers.Values.FirstOrDefault(s => s.SupportsObjectTypeSerialization(payload.GetType()));
            if (serializer != null)
                return serializer.Serialize(payload);

            throw new PayloadTypeSerializationNotSupportedException(payload.GetType().AssemblyQualifiedName);
        }
        #endregion

    }
}
