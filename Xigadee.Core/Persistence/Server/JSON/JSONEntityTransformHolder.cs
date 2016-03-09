using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class is used to transform an entity within the persistence handler.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class JsonEntityTransformHolder<K, E>: EntityTransformHolder<K, E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// This is the metadata key set for the entity type.
        /// </summary>
        public static KeyValuePair<string, string> cnJsonMetadata_EntityType = new KeyValuePair<string, string>("$microservice.entitytype", "$microservice.entitytype");

        static JsonEntityTransformHolder()
        {
            SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        }

        public JsonEntityTransformHolder()
        {
            EntitySerializer = JsonSerialize;
            EntityDeserializer = JsonDeserialize;
        }

        /// <summary>
        /// This is the standard Json serialization settings.
        /// </summary>
        public static readonly JsonSerializerSettings SerializerSettings;

        /// <summary>
        /// This function can be set to make the key from the entity.
        /// </summary>
        public override Func<string, E> EntityDeserializer { get; set; }        
        /// <summary>
        /// This function can be set to make the key from the entity.
        /// </summary>
        public override Func<E, string> EntitySerializer { get; set; }

        #region JsonDeserialize(string json)
        /// <summary>
        /// This is a simple JSON deserialization method that returns an entity from the 
        /// JSON representation from the DocumentDB repository.
        /// </summary>
        /// <param name="json">The JSON to convert.</param>
        /// <returns>The object to return.</returns>
        private E JsonDeserialize(string json)
        {
            // Remove the document db id field prior to deserializing
            var jObj = JObject.Parse(json);
            jObj.Remove("id");

            //JObject jobj = JObject.
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
        private string JsonSerialize(E entity)
        {
            var jObj = JObject.Parse(JsonConvert.SerializeObject(entity, SerializerSettings));

            K key = KeyMaker(entity);
            string id = KeySerializer(key);
            jObj["id"] = id;

            jObj[cnJsonMetadata_EntityType.Key] = EntityName;

            //Check for version support.
            string version = null;
            if (Version.SupportsVersioning)
            {
                version = Version.EntityVersionAsString(entity);
                jObj[Version.VersionJsonMetadata.Key] = version;
            }

            return jObj.ToString();
        }
        #endregion
    }
}
