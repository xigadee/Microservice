#region using

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the abstract base class for persistence services that use JSON and the serialization mechanism.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="E"></typeparam>
    public abstract class PersistenceManagerHandlerJsonBase<K, E, S>: PersistenceMessageHandlerBase<K, E, S>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
    {
        #region Declarations
        /// <summary>
        /// This is the standard Json serialization settings.
        /// </summary>
        protected readonly JsonSerializerSettings mJsonSerializerSettings;
        /// <summary>
        /// This is the json maker function. If this is null, the default method is used.
        /// </summary>
        protected readonly Func<RepositoryHolder<K, E>, JsonHolder<K>> mJsonMaker;
        /// <summary>
        /// This is the metadata key set for the entity type.
        /// </summary>
        protected readonly KeyValuePair<string, string> cnJsonMetadata_EntityType
            = new KeyValuePair<string, string>("$microservice.entitytype", "$microservice.entitytype");
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="entityName">The entity name, derived from E if left null.</param>
        /// <param name="versionPolicy">The optional version and locking policy.</param>
        /// <param name="defaultTimeout">The default timeout when making requests.</param>
        /// <param name="retryPolicy">The retry policy</param>
        protected PersistenceManagerHandlerJsonBase(
              string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, K> keyMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<RepositoryHolder<K, E>, JsonHolder<K>> jsonMaker = null
            , Func<string, E> entityDeserializer = null
            , Func<E, string> entitySerializer = null
            , Func<K, string> keySerializer = null
            , Func<string, K> keyDeserializer = null
            ) : 
            base( persistenceRetryPolicy: persistenceRetryPolicy
                , resourceProfile:resourceProfile
                , cacheManager: cacheManager
                , entityName: entityName
                , versionPolicy: versionPolicy
                , defaultTimeout: defaultTimeout
                , keyMaker:keyMaker
                , referenceMaker:referenceMaker
                , entityDeserializer: entityDeserializer
                , entitySerializer: entitySerializer
                , keySerializer: keySerializer
                , keyDeserializer: keyDeserializer
                )
        {
            mJsonMaker = jsonMaker;
            mJsonSerializerSettings=new JsonSerializerSettings { TypeNameHandling=TypeNameHandling.Auto };
            mTransform.EntitySerializer = (e) => JsonMaker(e).Json;

        }
        #endregion

        #region EntityMaker(string jsonHolder)
        /// <summary>
        /// This is a simple JSON deserialization method that returns an entity from the 
        /// JSON representation from the DocumentDB repository.
        /// </summary>
        /// <param name="json">The JSON to convert.</param>
        /// <returns>The object to return.</returns>
        protected override E EntityDeserialize(string json)
        {
            // Remove the document db id field prior to deserializing
            var jObj = JObject.Parse(json);
            jObj.Remove("id");

            //JObject jobj = JObject.
            var entity = JsonConvert.DeserializeObject<E>(jObj.ToString(Formatting.None), mJsonSerializerSettings);

            return entity;
        }
        #endregion

        #region JsonMaker(PersistenceRepositoryHolder<K, E> rq)
        /// <summary>
        /// This method converts in the incoming entity in to a JSON object.
        /// </summary>
        /// <param name="rq">The request type.</param>
        /// <returns>Returns a JsonHolder object with the contentid, versionid and json body.</returns>
        protected virtual JsonHolder<K> JsonMaker(PersistenceRepositoryHolder<K, E> rq)
        {
            if (mJsonMaker != null)
                return mJsonMaker(rq);

            return JsonMaker(rq.Entity);
        }
        #endregion
        #region JsonMaker(E entity)
        /// <summary>
        /// This method converts in the incoming entity in to a JSON object.
        /// </summary>
        /// <param name="rq">The request type.</param>
        /// <returns>Returns a JsonHolder object with the contentid, versionid and json body.</returns>
        protected virtual JsonHolder<K> JsonMaker(E entity)
        {
            var jObj = JObject.Parse(JsonConvert.SerializeObject(entity, mJsonSerializerSettings));

            K key = KeyMaker(entity);
            string id = KeyStringMaker(key);
            jObj["id"] = id;

            jObj[cnJsonMetadata_EntityType.Key] = mTransform.EntityName;

            //Check for version support.
            string version = null;
            if (mTransform.Version.SupportsVersioning)
            {
                version = mTransform.Version.EntityVersionAsString(entity);
                jObj[mTransform.Version.VersionJsonMetadata.Key] = version;
            }

            return new JsonHolder<K>(key, version, jObj.ToString(), id);
        }
        #endregion
    }

    public static class JsonParser<E>
    {
        /// <summary>
        /// This is the standard Json serialization settings.
        /// </summary>
        static readonly JsonSerializerSettings mJsonSerializerSettings;

        static JsonParser()
        {
            mJsonSerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        }

        public static string Serialize(E entity)
        {
            var jObj = JObject.Parse(JsonConvert.SerializeObject(entity, mJsonSerializerSettings));

            K key = KeyMaker(entity);
            string id = KeyStringMaker(key);
            jObj["id"] = id;

            jObj[cnJsonMetadata_EntityType.Key] = mTransform.EntityName;

            //Check for version support.
            string version = null;
            if (mTransform.Version.SupportsVersioning)
            {
                version = mTransform.Version.EntityVersionAsString(entity);
                jObj[mTransform.Version.VersionJsonMetadata.Key] = version;
            }

            return new JsonHolder<K>(key, version, jObj.ToString(), id);
        }

        public static E Deserialize(string json)
        {
            // Remove the document db id field prior to deserializing
            var jObj = JObject.Parse(json);
            jObj.Remove("id");

            //JObject jobj = JObject.
            var entity = JsonConvert.DeserializeObject<E>(jObj.ToString(Formatting.None), mJsonSerializerSettings);

            return entity;
        }
    }

}
