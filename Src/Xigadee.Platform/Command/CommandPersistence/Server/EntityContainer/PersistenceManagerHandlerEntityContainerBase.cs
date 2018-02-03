#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json.Linq;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This persistence class is used to hold entities in memory during the lifetime of the
    /// Microservice and does not persist to any backing store.
    /// This class is used extensively by the Unit test projects. The class inherits from Json base
    /// and so employs the same logic as that used by the Azure Storage and DocumentDb persistence classes.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="S">The persistence statistics type.</typeparam>
    /// <typeparam name="P">The persistence command policy type.</typeparam>
    /// <typeparam name="C">The entity container type.</typeparam>
    public abstract class PersistenceManagerHandlerEntityContainerBase<K, E, S, P, C>: PersistenceManagerHandlerJsonBase<K, E, S, P>, IRequireSecurityService
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
        where P : PersistenceCommandPolicy, new()
        where C : class, IPersistenceEntityContainer<K, E>, new()
    {
        #region Declarations
        /// <summary>
        /// Gets the pre-populate collection.
        /// </summary>
        protected IEnumerable<KeyValuePair<K, E>> mPrePopulate;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the memory persistence manager. 
        /// This persistence manager is used to hold an in-memory JSON representation of the entity.
        /// It is primarily used for test purposes, but can be used in a production context.
        /// Please note that all data will be lost when the service is restarted.
        /// </summary>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="keyDeserializer">The entity key deserializer.</param>
        /// <param name="entityName">The entity name to be used in the collection. By default this will be set through reflection.</param>
        /// <param name="versionPolicy">The version policy. This is needed if you wish to support optimistic locking for updates.</param>
        /// <param name="defaultTimeout">The default timeout. This is used for testing to simulate timeouts.</param>
        /// <param name="persistenceRetryPolicy">The retry policy. This is used for testing purposes.</param>
        /// <param name="resourceProfile">The resource profile.</param>
        /// <param name="cacheManager">The cache manager.</param>
        /// <param name="referenceMaker">The reference maker. This is used for entities that support read by reference.</param>
        /// <param name="referenceHashMaker">The reference hash maker. This is used for fast lookup.</param>
        /// <param name="keySerializer">The key serializer function.</param>
        /// <param name="policy">The policy.</param>
        /// <param name="prePopulate">The optional pre-population collection.</param>
        public PersistenceManagerHandlerEntityContainerBase(Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = default(TimeSpan?)
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null
            , P policy = null
            , IEnumerable<KeyValuePair<K, E>> prePopulate = null
            )
            : base(keyMaker, keyDeserializer, entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy, resourceProfile, cacheManager, referenceMaker, referenceHashMaker, keySerializer, policy)
        {
            mPrePopulate = prePopulate;
        }
        #endregion

        #region Start/Stop        
        /// <summary>
        /// This method starts the persistence command.
        /// </summary>
        protected override void StartInternal()
        {
            try
            {
                Container = new C();
                ContainerConfigure();
                Container.ServiceHandlers = ServiceHandlers;
                Container.Security = Security;
                Container.Start();
                PrePopulate();
            }
            catch (Exception ex)
            {
                Collector?.LogException($"{GetType().Name}/StartInternal", ex);
                throw;
            }

            base.StartInternal();
        }
        /// <summary>
        /// This method stops the persistence command.
        /// </summary>
        protected override void StopInternal()
        {
            base.StopInternal();

            Container.Stop();
            Container = null;
        }
        #endregion
        #region PrePopulate()
        /// <summary>
        /// This method is used to pre-populate entities in to the memory cache.
        /// </summary>
        public virtual void PrePopulate()
        {
            mPrePopulate?.ForEach((k) => Container.Add(k.Key, k.Value, mTransform?.ReferenceMaker?.Invoke(k.Value)));
            mPrePopulate = null;
        }
        #endregion

        #region Container
        /// <summary>
        /// This container holds the entity collection in storage.
        /// </summary>
        protected C Container { get; set; }
        #endregion
        #region ContainerConfigure()
        /// <summary>
        /// This method is called to configure the container.
        /// </summary>
        protected virtual void ContainerConfigure()
        {
            Container.Configure(mTransform);
        } 
        #endregion

        #region EntityPopulate(K key, E entity)
        /// <summary>
        /// This method allows items to be added to the collection. This method is broken out to allow
        /// entities to be loaded in the pre-populate stage.
        /// </summary>
        /// <param name="key">The entity key.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>The response status</returns>
        protected virtual int EntityPopulate(K key, E entity)
        {
            return Container.Add(key, entity, mTransform.ReferenceMaker(entity)); ;
        }
        #endregion

        #region InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
        /// <summary>
        /// This is the create override for the command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="holder">The entity holder.</param>
        /// <returns>Returns the response encapsulated in a PersistenceResponseHolder object.</returns>
        protected override async Task<IResponseHolder<E>> InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
        {
            E entity = holder.Rq.Entity;
            int response = EntityPopulate(key, entity);

            if (response == 201)
            {
                JsonHolder<K> jsonHolder = mTransform.JsonMaker(entity);
                return new PersistenceResponseHolder<E>(PersistenceResponse.Created201, jsonHolder.Json, mTransform.PersistenceEntitySerializer.Deserializer(jsonHolder.Json));
            }
            else
                return new PersistenceResponseHolder<E>(PersistenceResponse.Conflict409);

        }
        #endregion
        #region InternalRead(K key, PersistenceRequestHolder<K, E> holder)
        /// <summary>
        /// This is the read implementation.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="holder">The request holder.</param>
        /// <returns></returns>
        protected override async Task<IResponseHolder<E>> InternalRead(K key, PersistenceRequestHolder<K, E> holder)
        {

            E entity;
            bool success = Container.TryGetValue(key, out entity);

            if (success)
            {
                JsonHolder<K> jsonHolder = mTransform.JsonMaker(entity);
                return new PersistenceResponseHolder<E>(PersistenceResponse.Ok200, jsonHolder.Json, mTransform.PersistenceEntitySerializer.Deserializer(jsonHolder.Json));
            }
            else
                return new PersistenceResponseHolder<E>(PersistenceResponse.NotFound404);

        }
        #endregion
        #region InternalReadByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, E> holder)
        /// <summary>
        /// The internal read by reference command.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="holder">The request holder.</param>
        /// <returns></returns>
        protected override async Task<IResponseHolder<E>> InternalReadByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, E> holder)
        {
            E entity;
            bool success = Container.TryGetValue(reference, out entity);

            if (success)
            {
                JsonHolder<K> jsonHolder = mTransform.JsonMaker(entity);
                return new PersistenceResponseHolder<E>(PersistenceResponse.Ok200, jsonHolder.Json, mTransform.PersistenceEntitySerializer.Deserializer(jsonHolder.Json));
            }
            else
                return new PersistenceResponseHolder<E>(PersistenceResponse.NotFound404);

        }
        #endregion
        #region InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
        /// <summary>
        /// The internal update command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="holder">The request holder.</param>
        /// <returns></returns>
        protected override async Task<IResponseHolder<E>> InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
        {
            E oldEntity;
            bool successExisting = Container.TryGetValue(key, out oldEntity);
            if (!successExisting)
                return new PersistenceResponseHolder<E>(PersistenceResponse.NotFound404);

            E newEntity = holder.Rq.Entity;


            var ver = mTransform.Version;
            if (ver.SupportsOptimisticLocking)
            {
                if (ver.EntityVersionAsString(oldEntity) != ver.EntityVersionAsString(newEntity))
                    return new PersistenceResponseHolder<E>(PersistenceResponse.PreconditionFailed412);
            }

            if (ver.SupportsVersioning)
                ver.EntityVersionUpdate(newEntity);


            Container.Update(key, newEntity, mTransform.ReferenceMaker(newEntity));

            var jsonHolder = mTransform.JsonMaker(newEntity);
            return new PersistenceResponseHolder<E>(PersistenceResponse.Ok200)
            {
                Content = jsonHolder.Json
                , IsSuccess = true
                , Entity = newEntity
            };
        }
        #endregion
        #region InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        /// <summary>
        /// The internal delete command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="holder">The request holder.</param>
        /// <returns></returns>
        protected override async Task<IResponseHolder> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            if (!Container.Remove(key))
                return new PersistenceResponseHolder(PersistenceResponse.NotFound404);

            return new PersistenceResponseHolder(PersistenceResponse.Ok200);
        }
        #endregion
        #region InternalDeleteByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        /// <summary>
        /// The internal delete by reference command.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="holder">The request holder.</param>
        /// <returns></returns>
        protected override async Task<IResponseHolder> InternalDeleteByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            if (!Container.Remove(reference))
                return new PersistenceResponseHolder(PersistenceResponse.NotFound404);

            return new PersistenceResponseHolder(PersistenceResponse.Ok200);

        }
        #endregion
        #region InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        /// <summary>
        /// The internal version command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="holder">The request holder.</param>
        /// <returns></returns>
        protected override async Task<IResponseHolder> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            E entity;
            bool success = Container.TryGetValue(key, out entity);

            if (success)
            {
                JsonHolder<K> jsonHolder = mTransform.JsonMaker(entity);
                return new PersistenceResponseHolder(PersistenceResponse.Ok200, jsonHolder.Json);
            }
            else
                return new PersistenceResponseHolder(PersistenceResponse.NotFound404);
        }
        #endregion
        #region InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        /// <summary>
        /// The internal version by reference command.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="holder">The request holder.</param>
        /// <returns></returns>
        protected override async Task<IResponseHolder> InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            E entity;
            bool success = Container.TryGetValue(reference, out entity);

            if (success)
            {
                JsonHolder<K> jsonHolder = mTransform.JsonMaker(entity);
                return new PersistenceResponseHolder(PersistenceResponse.Ok200, jsonHolder.Json);
            }
            else
                return new PersistenceResponseHolder(PersistenceResponse.NotFound404);

        }
        #endregion

        #region InternalSearch(SearchRequest key, PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
        /// <summary>
        /// The internal search command.
        /// </summary>
        /// <param name="key">The search request.</param>
        /// <param name="holder">The request holder.</param>
        /// <returns></returns>
        protected async override Task<IResponseHolder<SearchResponse>> InternalSearch(SearchRequest key, PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
        {
            var query = Container.Values.AsQueryable<E>();

            Expression expression = mTransform.SearchTranslator.Build(key);

            //Execute Expression on Query
            bool success = true; //for the time being since we are not executing any queries
            holder.Rs.Entity = new SearchResponse();
            List<JObject> jObjects = new List<JObject>();
            foreach (var source in query.ToList())
            {
                jObjects.Add(JObject.FromObject(source));
            }
            if (success)
            {
                var resultEntities = query.ToList();
                holder.Rs.Entity.Data = jObjects;
                return new PersistenceResponseHolder<SearchResponse>(PersistenceResponse.Ok200, null, holder.Rs.Entity);
            }



            //holder.Rs.
            //key.Select
            ////Create the expression parameters
            //ParameterExpression num1 = Expression.Parameter(typeof(E), "num1");
            //ParameterExpression num2 = Expression.Parameter(typeof(E), "num2");

            ////Create the expression parameters
            //ParameterExpression[] parameters = new ParameterExpression[] { num1, num2 };

            ////Create the expression body
            //BinaryExpression body = Expression.Add(num1, num2);
            ////Expression predicateBody = Expression.OrElse(e1, e2);
            ////Create the expression 
            //Expression<Func<int, int, int>> expression = Expression.Lambda<Func<int, int, int>>(body, parameters);

            //// Compile the expression
            //Func<int, int, int> compiledExpression = expression.Compile();

            //// Execute the expression. 
            //int result = compiledExpression(3, 4);

            return await base.InternalSearch(key, holder);
        }
        #endregion

        #region Security
        /// <summary>
        /// This method provides a link to the Microservice to the security service, that provides authentication and encryption support.
        /// </summary>
        public virtual ISecurityService Security { get; set; } 
        #endregion
    }
}
