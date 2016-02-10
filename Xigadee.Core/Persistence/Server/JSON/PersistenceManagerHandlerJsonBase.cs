#region using

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
#endregion
namespace Xigadee
{
    public abstract class PersistenceManagerHandlerJsonBase<K, E>: PersistenceManagerHandlerJsonBase<K, E, PersistenceStatistics>
        where K : IEquatable<K>
    {
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
            , ResourceProfile resourceProfile = null) 
            : base(entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy: persistenceRetryPolicy, resourceProfile: resourceProfile)
        {
        }
        #endregion
    }

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

        protected readonly JsonSerializerSettings mJsonSerializerSettings;

        protected string mEntityName;

        /// <summary>
        /// This is the metadata key set for the entity type.
        /// </summary>
        protected readonly KeyValuePair<string, string> cnJsonMetadata_EntityType
            = new KeyValuePair<string, string>("$microservice.entitytype", "$microservice.entitytype");

        /// <summary>
        /// This function is used by optimistic locking, it is used to define the version id for the entity.
        /// </summary>
        protected readonly VersionPolicy<E> mVersion;
        /// <summary>
        /// 
        /// </summary>
        protected readonly TimeSpan? mDefaultTimeout;
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
            , ResourceProfile resourceProfile = null) : base(persistenceRetryPolicy: persistenceRetryPolicy, resourceProfile:resourceProfile)
        {
            mJsonSerializerSettings=new JsonSerializerSettings { TypeNameHandling=TypeNameHandling.Auto };

            mVersion=versionPolicy??new VersionPolicy<E>();

            mEntityName=entityName??typeof(E).Name.ToLowerInvariant();

            mDefaultTimeout=defaultTimeout;
        } 
        #endregion

        #region EntityMaker(string jsonHolder)
        /// <summary>
        /// This is a simple JSON deserialization method that returns an entity from the 
        /// JSON representation from the DocumentDB repository.
        /// </summary>
        /// <param name="json">The JSON to convert.</param>
        /// <returns>The object to return.</returns>
        protected virtual E EntityMaker(string json)
        {
            // Remove the document db id field prior to deserializing
            var jObj = JObject.Parse(json);
            jObj.Remove("id");

            //JObject jobj = JObject.
            var entity = JsonConvert.DeserializeObject<E>(jObj.ToString(Formatting.None), mJsonSerializerSettings);

            return entity;
        }
        #endregion
        #region KeyStringMaker(K key)
        /// <summary>
        /// This is a very simple key serializer to a string representation.
        /// </summary>
        /// <param name="key">The incoming key object.</param>
        /// <returns>The output string.</returns>
        protected virtual string KeyStringMaker(K key)
        {
            return string.Format("{0}.{1}", mEntityName, key.ToString());
        }
        #endregion

        #region KeyMaker(E entity)
        /// <summary>
        /// This method intercepts and replaces the keymaker in the function has been set in the constructor.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>Returns the key from the entity.</returns>
        protected virtual K KeyMaker(E entity)
        {
            if (mKeyMaker==null)
                throw new NotImplementedException();

            return mKeyMaker(entity);
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

            jObj[cnJsonMetadata_EntityType.Key] = mEntityName;

            //Check for version support.
            string version = null;
            if (mVersion.SupportsVersioning)
            {
                version = mVersion.EntityVersionAsString(entity);
                jObj[mVersion.VersionJsonMetadata.Key] = version;
            }

            return new JsonHolder<K>(key, version, jObj.ToString(), id);
        }
        #endregion
    }
}
