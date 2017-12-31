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
        /// The serializers that support magic bytes.
        /// </summary>
        protected Dictionary<string, IPayloadSerializerMagicBytes> mPayloadSerializersMagicBytes;

        /// <summary>
        /// This contains the supported serializers.
        /// </summary>
        protected Dictionary<string, IPayloadSerializer> mPayloadSerializers;

        /// <summary>
        /// This contains the supported serializers.
        /// </summary>
        protected Dictionary<string, IPayloadCompressor> mPayloadCompression;

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
            mPayloadSerializers = new Dictionary<string, IPayloadSerializer>();
            mPayloadSerializersMagicBytes = new Dictionary<string, IPayloadSerializerMagicBytes>();
        }
        #endregion

        /// <summary>
        /// Gets or sets the default type of the content type. This is based on the first serializer added to the collection.
        /// </summary>
        public string DefaultContentType { get; protected set; }

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

                statistics.Serialization = mPayloadSerializers?.Select((c) => $"{c.Key}: {c.Value.GetType().Name}").ToArray();
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

        #region Add/Clear/Count
        /// <summary>
        /// This method adds the serializer to the collection.
        /// </summary>
        /// <param name="serializer">The serializer to add.</param>
        public IPayloadSerializer Add(IPayloadSerializer serializer)
        {
            if (string.IsNullOrEmpty(serializer.ContentType))
                throw new ArgumentOutOfRangeException("ContentType cannot be null or empty.");

            var contentType = serializer.ContentType.ToLowerInvariant();
            mPayloadSerializers.Add(contentType, serializer);
            if (serializer is IPayloadSerializerMagicBytes)
            {
                var mb = (IPayloadSerializerMagicBytes)serializer;
                mPayloadSerializersMagicBytes.Add(mb.ToIdentifier(), mb);
            }

            mLookUpCache?.Clear();

            DefaultContentType = DefaultContentType ?? contentType;

            return serializer;
        }
        /// <summary>
        /// This method clears all the serializers currently registered.
        /// </summary>
        public void Clear()
        {
            mPayloadSerializers.Clear();
            mLookUpCache?.Clear();
            DefaultContentType = null;
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
        /// <param name="holder">The binary holder.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public P PayloadDeserialize<P>(SerializationHolder holder)
        {
            if (holder?.Blob == null || mPayloadSerializers.Count == 0)
                return default(P);

            var mimeType = PrepareMimeType(holder.ContentType);

            var serializer = mPayloadSerializers[mimeType];

            if (serializer != null 
                && serializer.TryDeserialize(holder) 
                && holder.ObjectType == typeof(P))
                return (P)holder.Object;

            return default(P);
        }

        /// <summary>
        /// This method deserializes the binary blob and returns the object.
        /// </summary>
        /// <param name="holder">The binary holder.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        public object PayloadDeserialize(SerializationHolder holder)
        {
            if (holder?.Blob == null || mPayloadSerializers.Count == 0)
                return null;

            var mimeType = PrepareMimeType(holder.ContentType);

            var serializer = mPayloadSerializers[mimeType];

            if (serializer != null
                && serializer.TryDeserialize(holder))
                return holder.Object;

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
            //if (payload == null)
            //    return null;

            //var serializer = mPayloadSerializers.Values.FirstOrDefault(s => s.SupportsContentTypeSerialization(payload.GetType()));
            //if (serializer != null)
            //    return serializer.Serialize(payload);

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
                            ser = mPayloadSerializers.Values.FirstOrDefault((s) => s.SupportsContentTypeSerialization(t));
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



        private string PrepareMimeType(string mimetype)
        {
            if (mimetype == null)
                throw new ArgumentNullException(nameof(mimetype));

            var items = mimetype.Split(';');

            return items[0].Trim().ToLowerInvariant();
        }

        public bool SupportsSerializer(string mimetype)
        {
            return mPayloadSerializers.ContainsKey(PrepareMimeType(mimetype));
        }

        public bool TryPayloadSerialize(SerializationHolder holder)
        {
            return false;
        }

        public bool TryPayloadDeserialize(SerializationHolder holder)
        {
            return false;
        }

        #region Collector
        /// <summary>
        /// This is the data collector.
        /// </summary>
        public IDataCollection Collector { get; set; }
        #endregion

    }
}
