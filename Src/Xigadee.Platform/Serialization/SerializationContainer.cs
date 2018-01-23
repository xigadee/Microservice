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
    public partial class SerializationContainer : ServiceContainerBase<SerializationStatistics, SerializationContainer.Policy>
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
        protected Dictionary<string, IPayloadCompressor> mPayloadCompressors;
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
        public SerializationContainer(Policy policy = null)
            : base(policy)
        {
            mPayloadSerializers = new Dictionary<string, IPayloadSerializer>();
            mPayloadSerializersMagicBytes = new Dictionary<string, IPayloadSerializerMagicBytes>();
            mPayloadCompressors = new Dictionary<string, IPayloadCompressor>();
        }
        #endregion

        #region DefaultContentType
        /// <summary>
        /// Gets or sets the default type of the content type. This is based on the first serializer added to the collection.
        /// </summary>
        public string DefaultContentType { get; protected set; } 
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

                statistics.Serialization = mPayloadSerializers?.Select((c) => $"{c.Key}: {c.Value.GetType().Name}").ToArray();
                statistics.Compression = mPayloadCompressors?.Select((c) => $"{c.Key}: {c.Value.GetType().Name}").ToArray();
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
            ClearSerializers();
            ClearCompressors();
        }
        #endregion

        #region Add/Clear/Count - Serializer
        /// <summary>
        /// This method adds the serializer to the collection.
        /// </summary>
        /// <param name="component">The serializer to add.</param>
        public IPayloadSerializer Add(IPayloadSerializer component)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            string id;
            if (!ExtractContentType(component.ContentType, out id))
                throw new ArgumentOutOfRangeException($"ContentType '{component.ContentType}' is not valid.");

            mPayloadSerializers.Add(id, component);
            if (component is IPayloadSerializerMagicBytes)
            {
                var mb = (IPayloadSerializerMagicBytes)component;
                mPayloadSerializersMagicBytes.Add(mb.ToIdentifier(), mb);
            }

            mLookUpCache?.Clear();

            DefaultContentType = DefaultContentType ?? id;

            return component;
        }
        /// <summary>
        /// This method clears all the serializers currently registered.
        /// </summary>
        public void ClearSerializers()
        {
            mPayloadSerializers.Clear();
            mPayloadSerializersMagicBytes.Clear();
            mLookUpCache?.Clear();
            DefaultContentType = null;
        }
        /// <summary>
        /// Gets the count on the number of registered serializers.
        /// </summary>
        public int CountSerializers => mPayloadSerializers?.Count ?? 0;

        #endregion
        #region Add/Clear/Count - Compressor
        /// <summary>
        /// This method adds the compressor to the collection.
        /// </summary>
        /// <param name="component">The compressor to add.</param>
        public IPayloadCompressor Add(IPayloadCompressor component)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            string id;
            if (!ExtractContentEncoding(component.ContentEncoding, out id))
                throw new ArgumentOutOfRangeException($"ContentEncoding '{component.ContentEncoding}' is not valid.");

            mPayloadCompressors.Add(id, component);

            return component;
        }
        /// <summary>
        /// This method clears all the compressors currently registered.
        /// </summary>
        public void ClearCompressors()
        {
            mPayloadCompressors.Clear();
        }
        /// <summary>
        /// Gets the count on the number of registered compressors.
        /// </summary>
        public int CountCompressors => mPayloadCompressors?.Count ?? 0; 
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
            return (P)PayloadDeserialize(holder);
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

            if (TryPayloadDeserialize(holder))
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
        /// <returns>Returns the binary blob holder.</returns>
        public SerializationHolder PayloadSerialize(object payload)
        {
            var holder = SerializationHolder.CreateWithObject(payload);
            holder.ContentType = DefaultContentType;

            if (TryPayloadSerialize(holder))
                return holder;

            return null;
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

        #region ExtractContentType(string contentType, out string value)        
        /// <summary>
        /// Extracts the type of the content in the format type/subtype.
        /// </summary>
        /// <param name="headerField">Type of the content.</param>
        /// <param name="value">The value.</param>
        /// <remarks>See: https://www.w3.org/Protocols/rfc1341/4_Content-Type.html </remarks>
        /// <returns>Returns true if the content type can be extracted from the header field.</returns>
        public static bool ExtractContentType(string headerField, out string value)
        {
            value = null;

            if (string.IsNullOrEmpty(headerField))
                return false;

            var items = headerField.Split(';');

            value = items[0].Trim().ToLowerInvariant();
            return true;
        }
        #endregion
        #region ExtractContentEncoding(string contentEncoding, out string value)        
        /// <summary>
        /// Extracts the content encoding in to a matchable format.
        /// </summary>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <param name="value">The value.</param>
        /// <returns>Returns true if it can be extracted.</returns>
        public static bool ExtractContentEncoding(string contentEncoding, out string value)
        {
            value = null;

            if (string.IsNullOrEmpty(contentEncoding))
                return false;

            value = contentEncoding.Trim().ToLowerInvariant();
            return true;
        }
        #endregion

        //Serializer
        #region SupportsSerializer...
        /// <summary>
        /// Checks that a specific serializer is supported.
        /// </summary>
        /// <param name="contentType">The content type identifier for the serializer.</param>
        /// <returns>
        /// Returns true if the serializer is supported.
        /// </returns>
        public bool SupportsSerializer(string contentType)
        {
            IPayloadSerializer serializer;
            return TryGetSerializer(contentType, out serializer);
        }
        /// <summary>
        /// Checks that a specific serializer is supported.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true when the holder ContentType is supported.
        /// </returns>
        public bool SupportsSerializer(SerializationHolder holder)
        {
            IPayloadSerializer serializer;
            return TryGetSerializer(holder.ContentType, out serializer);
        }
        #endregion
        #region TryDeserialize(SerializationHolder holder)
        /// <summary>
        /// Tries to deserialize the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the incoming binary payload is successfully deserialized.
        /// </returns>
        public bool TryDeserialize(SerializationHolder holder)
        {
            IPayloadSerializer sr = null;
            if (!TryGetSerializer(holder.ContentType, out sr))
                return false;

            return sr.TryDeserialize(holder);
        } 
        #endregion
        #region TrySerialize(SerializationHolder holder)
        /// <summary>
        /// Tries to compress the outgoing holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the Content is serialized correctly to a binary blob.
        /// </returns>
        public bool TrySerialize(SerializationHolder holder)
        {
            IPayloadSerializer sr = null;
            if (!TryGetSerializer(holder.ContentType, out sr))
                return false;

            return sr.TrySerialize(holder);
        } 
        #endregion
        #region TryGetSerializer(string contentType, out IPayloadSerializer serializer)
        /// <summary>
        /// Tries to get the serializer.
        /// </summary>
        /// <param name="contentType">The serializer mime type.</param>
        /// <param name="serializer">The output serializer.</param>
        /// <returns>Returns true if successfully matched.</returns>
        protected bool TryGetSerializer(string contentType, out IPayloadSerializer serializer)
        {
            serializer = null;
            if (string.IsNullOrEmpty(contentType))
                return false;

            string sType;
            if (!ExtractContentType(contentType, out sType))
                return false;

            return mPayloadSerializers.TryGetValue(sType, out serializer);
        }
        #endregion
        //Compressor
        #region SupportsCompressor...
        /// <summary>
        /// A boolean function that returns true if the compression type is supported.
        /// </summary>
        /// <param name="contentEncodingType">The content encoding type, i.e. GZIP/DEFLATE etc..</param>
        /// <returns>
        /// Returns true when the ContentEncoding type is supported.
        /// </returns>
        public bool SupportsCompressor(string contentEncodingType)
        {
            IPayloadCompressor compressor;
            return TryGetCompressor(contentEncodingType, out compressor);
        }
        /// <summary>
        /// A boolean function that returns true if the compression type is supported.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true when the holder ContentEncoding is supported.
        /// </returns>
        public bool SupportsCompressor(SerializationHolder holder)
        {
            return SupportsCompressor(holder.ContentEncoding);
        }
        #endregion
        #region TryDecompress(SerializationHolder holder)
        /// <summary>
        /// Tries to decompress the incoming holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the incoming binary payload is successfully decompressed.
        /// </returns>
        public bool TryDecompress(SerializationHolder holder)
        {
            IPayloadCompressor comp;
            if (!TryGetCompressor(holder.ContentEncoding, out comp))
                return false;

            return comp.TryDecompression(holder);
        }
        #endregion
        #region TryCompress(SerializationHolder holder)
        /// <summary>
        /// Tries to compress the outgoing payload.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <returns>
        /// Returns true if the Content is compressed correctly to a binary blob.
        /// </returns>
        public bool TryCompress(SerializationHolder holder)
        {
            IPayloadCompressor comp;
            if (!TryGetCompressor(holder.ContentEncoding, out comp))
                return false;

            return comp.TryCompression(holder);
        } 
        #endregion
        #region TryGetCompressor(string contentEncodingType, out IPayloadCompressor compressor)
        /// <summary>
        /// Tries to get the compressor.
        /// </summary>
        /// <param name="contentEncodingType">The content encoding type.</param>
        /// <param name="compressor">The compressor.</param>
        /// <returns>Returns true if successful.</returns>
        protected bool TryGetCompressor(string contentEncodingType, out IPayloadCompressor compressor)
        {
            compressor = null;
            string ceType;

            if (!ExtractContentEncoding(contentEncodingType, out ceType))
                return false;

            return mPayloadCompressors.TryGetValue(ceType, out compressor);
        }
        #endregion

        #region TryPayloadSerialize(SerializationHolder holder, bool throwExceptions = false)
        /// <summary>
        /// This method attempts to Serialize the object and sets the blob and headers in the holder.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <param name="throwExceptions">Directs the container to throw detailed exceptions on failure. The default is false.</param>
        /// <returns>
        /// Returns true if the operation is successful.
        /// </returns>
        public bool TryPayloadSerialize(SerializationHolder holder, bool throwExceptions = false)
        {
            return TrySerialize(holder) && holder.HasContentEncoding && TryCompress(holder);
        }
        #endregion
        #region TryPayloadDeserialize(SerializationHolder holder, bool throwExceptions = false)
        /// <summary>
        /// This method attempts to deserialize the binary blob and sets the object in the holder.
        /// </summary>
        /// <param name="holder">The serialization holder.</param>
        /// <param name="throwExceptions">Directs the container to throw detailed exceptions on failure. The default is false.</param>
        /// <returns>
        /// Returns true if the operation is successful.
        /// </returns>
        public bool TryPayloadDeserialize(SerializationHolder holder, bool throwExceptions = false)
        {
            if (holder.HasContentEncoding && !TryDecompress(holder))
                return false;

            return TryDeserialize(holder);
        }
        #endregion

        #region Collector
        /// <summary>
        /// This is the data collector.
        /// </summary>
        public IDataCollection Collector { get; set; }
        #endregion

        /// <summary>
        /// This policy contains the settings for the Serialization Container.
        /// </summary>
        /// <seealso cref="Xigadee.PolicyBase" />
        public class Policy: PolicyBase
        {
            /// <summary>
            /// Specifies whether the serialization container supports the object registry. 
            /// By default this is false to preserve legacy behaviour.
            /// </summary>
            public bool ContentRegistrySupported { get; set; } = false;
            /// <summary>
            /// This is the maximum number of objects that the registry will support. Leave this null to not define a limit.
            /// </summary>
            public int? ContentRegistryLimit { get; set; } = 10000;

            /// <summary>
            /// This is the maximum time that an object can survive in the object registry,
            /// </summary>
            public TimeSpan ContentRegistryTimeToLive { get; set; } = TimeSpan.FromSeconds(15);
            /// <summary>
            /// Specifies that a warning should be issued to the data collector when an object times out.  
            /// </summary>
            public bool ContentRegistryWarningOnTimeout { get; set; } = true;
        }
    }
}
