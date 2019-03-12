using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Xigadee
{
    /// <summary>
    /// This class is used to transform an entity within the persistence handler.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class EntityTransformHolder<K, E> where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This is the metadata key set for the entity type.
        /// </summary>
        public readonly KeyValuePair<string, string> JsonMetadata_EntityType = new KeyValuePair<string, string>("$microservice.entitytype", "$microservice.entitytype");
        /// <summary>
        /// This is the standard Json serialization settings.
        /// </summary>
        public static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTransformHolder{K, E}"/> class.
        /// </summary>
        /// <param name="jsonMode">if set to <c>true</c> [json mode].</param>
        public EntityTransformHolder(bool jsonMode = false)
        {
            if (jsonMode)
            {
                PersistenceEntitySerializer = new EntitySerializer<E>(JsonSerialize, JsonDeserialize);
            }

            // Set sensible defaults
            CacheEntitySerializer = new EntitySerializer<E>(JsonSerialize, JsonDeserialize);
            EntityName = typeof(E).Name.ToLowerInvariant();
            ReferenceHashMaker = t => $"{t.Item1.ToLowerInvariant()}.{t.Item2.ToLowerInvariant()}";

            // Handle the most common key types of string and guid for deserializing the key from string
            KeySerializer = k => k.ToString();
            if (typeof(K) == typeof(string))
                KeyDeserializer = s => (K)(object)(s);
            else if (typeof(K) == typeof(Guid))
                KeyDeserializer = s => (K)(object)Guid.Parse(s);

            SearchTranslator = new SearchExpressionHelper<E>();
        } 
        #endregion

        /// <summary>
        /// This is the search expression helper for search requests.
        /// </summary>
        public virtual SearchExpressionHelper<E> SearchTranslator { get; protected set; }
        /// <summary>
        /// This function is used by optimistic locking, it is used to define the version id for the entity.
        /// </summary>
        public virtual VersionPolicy<E> Version { get; set; }
        /// <summary>
        /// This is the entity name.
        /// </summary>
        public virtual string EntityName { get; set; }
        /// <summary>
        /// This function can be set to make the key from the entity.
        /// </summary>
        public virtual Func<E, K> KeyMaker { get; set; }
        /// <summary>
        /// This is the serializer used to store and retrieve the entity from persistence.
        /// </summary>
        public virtual EntitySerializer<E> PersistenceEntitySerializer { get; set; }
        /// <summary>
        /// This is the serializer used to store and retrieve the entity for caching.
        /// </summary>
        public virtual EntitySerializer<E> CacheEntitySerializer { get; set; }
        /// <summary>
        /// This function can be used to extract the references from an incoming entity to allow for caching.
        /// </summary>
        public virtual Func<E, IEnumerable<Tuple<string, string>>> ReferenceMaker { get; set; }
        /// <summary>
        /// This function can be used to make a hash from the References passed in.
        /// </summary>
        public virtual Func<Tuple<string, string>, string> ReferenceHashMaker { get; set; }
        /// <summary>
        /// This method makes a string reference from the key.
        /// </summary>
        public virtual Func<K, string> KeySerializer { get; set; }
        /// <summary>
        /// This method is used to create a unique string format for the persistence store.
        /// </summary>
        public virtual Func<K, string> KeyStringMaker => k => $"{EntityName}.{KeySerializer(k)}";
        /// <summary>
        /// This method deserializes the string version of the key to the object.
        /// </summary>
        public virtual Func<string, K> KeyDeserializer { get; set; }

        #region JsonDeserialize(string json)
        /// <summary>
        /// This is a simple JSON deserialization method that returns an entity from the 
        /// JSON representation from the DocumentDB repository.
        /// </summary>
        /// <param name="json">The JSON to convert.</param>
        /// <returns>The object to return.</returns>
        public virtual E JsonDeserialize(string json)
        {
            // Remove the documentDb id field prior to de-serializing
            var jObj = JObject.Parse(json);

            jObj.Remove("id");

            jObj.Remove(JsonMetadata_EntityType.Key);

            if (Version != null)
                jObj.Remove(Version.VersionJsonMetadata.Key);

            var entity = JsonConvert.DeserializeObject<E>(jObj.ToString(Formatting.None), SerializerSettings);

            return entity;
        }
        #endregion
        #region JsonSerialize(E entity)
        /// <summary>
        /// This method converts in the incoming entity in to a JSON object.
        /// </summary>
        /// <param name="entity">The request type.</param>
        /// <returns>Returns a JsonHolder object with the content id, version id and json body.</returns>
        public virtual string JsonSerialize(E entity)
        {
            return JsonMaker(entity).Json;
        }
        #endregion

        #region JsonMaker(E entity)
        /// <summary>
        /// This method converts in the incoming entity in to a JSON object.
        /// </summary>
        /// <param name="entity">The request type.</param>
        /// <returns>Returns a JsonHolder object with the content id, version id and json body.</returns>
        public JsonHolder<K> JsonMaker(E entity)
        {
            var jObj = JObject.Parse(JsonConvert.SerializeObject(entity, SerializerSettings));

            K key = KeyMaker(entity);
            string id = KeyStringMaker(key);
            jObj["id"] = id;

            jObj[JsonMetadata_EntityType.Key] = EntityName;

            //Check for version support.
            string version = null;
            if (Version != null && Version.SupportsVersioning)
            {
                version = Version.EntityVersionAsString(entity);
                jObj[Version.VersionJsonMetadata.Key] = version;
            }

            return new JsonHolder<K>(key, version, jObj.ToString(), id);
        }
        #endregion
    }
}
