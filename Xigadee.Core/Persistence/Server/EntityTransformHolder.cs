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
    public class EntityTransformHolder<K, E>
        where K : IEquatable<K>
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
        /// This function can be set to make the key from the entity.
        /// </summary>
        public virtual Func<string, E> EntityDeserializer { get; set; }        
        /// <summary>
        /// This function can be set to make the key from the entity.
        /// </summary>
        public virtual Func<E, string> EntitySerializer { get; set; }
        /// <summary>
        /// This function can be used to extract the references from an incoming entity to allow for caching.
        /// </summary>
        public virtual Func<E, IEnumerable<Tuple<string, string>>> ReferenceMaker { get; set; }
        /// <summary>
        /// This method makes a string reference from the key.
        /// </summary>
        public virtual Func<K, string> KeySerializer { get; set; }
        /// <summary>
        /// This method is used to create a unique string format for the persistence store.
        /// </summary>
        public virtual Func<K, string> KeyStringMaker => (k) => string.Format("{0}.{1}", EntityName, KeySerializer(k));
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
            // Remove the document db id field prior to deserializing
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
        /// <param name="rq">The request type.</param>
        /// <returns>Returns a JsonHolder object with the contentid, versionid and json body.</returns>
        public virtual string JsonSerialize(E entity)
        {
            return JsonMaker(entity).Json;
        }
        #endregion

        #region JsonMaker(E entity)
        /// <summary>
        /// This method converts in the incoming entity in to a JSON object.
        /// </summary>
        /// <param name="rq">The request type.</param>
        /// <returns>Returns a JsonHolder object with the contentid, versionid and json body.</returns>
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
