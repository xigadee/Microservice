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
    /// This is the content holder.
    /// </summary>
    public class ContentRegistryHolder
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public object Content { get; set; }
    }

    /// <summary>
    /// This container holds the system serialization/de-serialization components that are used when transmitting data outside of the system.
    /// </summary>
    public partial class SerializationContainer : ServiceContainerBase<SerializationStatistics, SerializationPolicy>
        , IPayloadSerializationContainer, IRequireDataCollector
    {
        #region Declarations        
        /// <summary>
        /// This is a list of serializers in the order they were added to the collection. The first serializer added will get priority when serializing objects.
        /// </summary>
        protected List<IPayloadSerializer> mSerializers;

        /// <summary>
        /// This contains the supported serializers.
        /// </summary>
        protected Dictionary<byte[], IPayloadSerializer> mPayloadSerializers;

        /// <summary>
        /// This is the look up cache for the specific type.
        /// </summary>
        protected ConcurrentDictionary<Type, IPayloadSerializer> mLookUpCache;

        /// <summary>
        /// This is the object registry cache.
        /// </summary>
        protected ConcurrentDictionary<Guid, ContentRegistryHolder> mContentRegistry;
        #endregion
        #region Constructor
        /// <summary>
        /// This default constructor takes the list of registered serializers.
        /// </summary>
        /// <param name="policy">The collection of serializers</param>
        public SerializationContainer(SerializationPolicy policy = null)
            : base(policy)
        {
            mPayloadSerializers = new Dictionary<byte[], IPayloadSerializer>();
        }
        #endregion

        #region Collector
        /// <summary>
        /// This is the data collector.
        /// </summary>
        public IDataCollection Collector { get; set; } 
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
            mContentRegistry = new ConcurrentDictionary<Guid, ContentRegistryHolder>();
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
            mContentRegistry.Clear();
            mContentRegistry = null;
        }
        #endregion

        #region Add/Clear/Count
        /// <summary>
        /// This method adds the serializer to the collection.
        /// </summary>
        /// <param name="fnSerializer">The function to create the serializer.</param>
        public IPayloadSerializer Add(Func<IPayloadSerializer> fnSerializer)
        {
            return Add(fnSerializer());
        }
        /// <summary>
        /// This method adds the serializer to the collection.
        /// </summary>
        /// <param name="serializer">The serializer to add.</param>
        public IPayloadSerializer Add(IPayloadSerializer serializer)
        {
            mPayloadSerializers.Add(serializer.Identifier, serializer);
            mLookUpCache?.Clear();

            return serializer;
        }
        /// <summary>
        /// This method clears all the serializers currently registered.
        /// </summary>
        public void Clear()
        {
            mPayloadSerializers.Clear();
            mLookUpCache?.Clear();
        }
        /// <summary>
        /// Gets the count on the number of registered serializers.
        /// </summary>
        public int Count => mPayloadSerializers?.Count ?? 0; 
        #endregion

        #region PayloadDeserialize...

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

        #region SerializerResolve(Type objectType, out IPayloadSerializer serializer)
        /// <summary>
        /// This method resolves the relevant serializer for the object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="serializer">The output serializer.</param>
        /// <returns>Returns true if a serializer can be resolved.</returns>
        /// <exception cref="ArgumentNullException">objectType cannot be null.</exception>
        protected bool SerializerResolve(Type objectType, out IPayloadSerializer serializer)
        {
            if (objectType == null)
                throw new ArgumentNullException("objectType", "objectType cannot be null.");

            serializer = null;

            serializer = mLookUpCache.GetOrAdd(objectType
                , (t) =>
                    {
                        IPayloadSerializer ser = null;
                        try
                        {
                            ser = mPayloadSerializers.Values.FirstOrDefault((s) => s.SupportsObjectTypeSerialization(t));
                        }
                        catch (Exception ex)
                        {
                            Collector?.LogException("", ex);
                            ser = null;
                        }

                        return ser;
                    }
                );

            return serializer != null;
        }
        #endregion

        #region RegistryExtract<P>(SerializationHolder holder, out P dto)
        /// <summary>
        /// This method extracts an item from the object registry.
        /// </summary>
        /// <typeparam name="P">The object type.</typeparam>
        /// <param name="holder">The serialization holder.</param>
        /// <param name="dto">The data transfer object.</param>
        /// <returns>Returns true if the object has been found in the object registry.</returns>
        public bool RegistryExtract<P>(SerializationHolder holder, out P dto)
        {
            dto = default(P);

            if (!holder.ObjectRegistryId.HasValue)
                return false;

            ContentRegistryHolder item;
            if (!mContentRegistry.TryGetValue(holder.ObjectRegistryId.Value, out item))
                return false;

            if (item.Content.GetType().IsAssignableFrom(typeof(P)))
                return false;

            dto = (P)item.Content;
            return true;
        } 
        #endregion

        public bool DtoTryExtraction(SerializationHolder holder, out object dto, bool throwExceptions = false, bool useObjectRegistryIfSupported = true)
        {
            return DtoTryExtraction<object>(holder, out dto, throwExceptions, useObjectRegistryIfSupported);
        }

        public bool DtoTryExtraction<P>(SerializationHolder holder, out P dto, bool throwExceptions = false, bool useObjectRegistryIfSupported = true)
        {
            if (useObjectRegistryIfSupported && RegistryExtract(holder, out dto))
                return true;

            throw new NotImplementedException();
        }

        public bool DtoTryInsertion(object dto, out SerializationHolder holder, bool throwExceptions = false, bool useObjectRegistryIfSupported = true)
        {
            holder = new SerializationHolder();

            if (dto == null)
                return false;

            IPayloadSerializer serializer;
            if (!SerializerResolve(dto.GetType(), out serializer))
                return false;

            holder.Blob = serializer.Serialize(dto);

            return true;
        }
    }
}
