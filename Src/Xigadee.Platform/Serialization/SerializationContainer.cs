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
    /// This container holds the system serialization/de-serialization components that are used when transmitting data outside of the system.
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
        /// This default constructor takes the list of registered serializers.
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
                statistics.Cache = mLookUpCache?.Select((c) => $"{c.Key.Name}: {c.Value.GetType().Name}").ToArray();
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

            mLookUpCache = new ConcurrentDictionary<Type, IPayloadSerializer>();
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
        /// This method clears all the serializers currently registered.
        /// </summary>
        public void Clear()
        {
            mPayloadSerializers.Clear();
        }
        #endregion

        #region PayloadDeserialize...

        #region ServiceMessage ...


        #endregion

        #region byte[]...
        /// <summary>
        /// This method deserializes the binary blob and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public P PayloadDeserialize<P>(SerializationHolder blob)
        {
            if (blob?.Blob == null || mPayloadSerializers.Count == 0)
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
        public object PayloadDeserialize(SerializationHolder blob)
        {
            if (blob?.Blob == null || mPayloadSerializers.Count == 0)
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
        public SerializationHolder PayloadSerialize(object payload)
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
