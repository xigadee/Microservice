#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
#endregion
namespace Xigadee
{
    #region PersistenceManagerHandlerMemory<K, E>
    /// <summary>
    /// This persistence class is used to hold entities in memory during the lifetime of the 
    /// Microservice and does not persist to any backing store.
    /// This class is used extensively by the Unit test projects. The class inherits from Json base
    /// and so employs the same logic as that used by the Azure Storage and DocumentDb persistence classes.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class PersistenceManagerHandlerMemory<K, E> : PersistenceManagerHandlerMemory<K, E, PersistenceStatistics>
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// This is the document db persistence agent.
        /// </summary>
        /// <param name="keyMaker">This function creates a key of type K from an entity of type E</param>
        /// <param name="keyDeserializer"></param>
        /// <param name="entityName">The entity name to be used in the collection. By default this will be set through reflection.</param>
        /// <param name="versionPolicy"></param>
        /// <param name="defaultTimeout">This is the default timeout period to be used when connecting to documentDb.</param>
        /// <param name="persistenceRetryPolicy"></param>
        /// <param name="resourceProfile"></param>
        /// <param name="cacheManager"></param>
        /// <param name="referenceMaker"></param>
        /// <param name="referenceHashMaker"></param>
        /// <param name="keySerializer"></param>
        public PersistenceManagerHandlerMemory(Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null
            )
            : base(keyMaker, keyDeserializer
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , referenceMaker: referenceMaker
                  , referenceHashMaker : referenceHashMaker
                  , keySerializer: keySerializer
                  )
        {
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// This persistence class is used to hold entities in memory during the lifetime of the 
    /// Microservice and does not persist to any backing store.
    /// This class is used extensively by the Unit test projects. The class inherits from Json base
    /// and so employs the same logic as that used by the Azure Storage and DocumentDb persistence classes.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="S">An extended statistics class.</typeparam>
    public abstract class PersistenceManagerHandlerMemory<K, E, S> : PersistenceManagerHandlerJsonBase<K, E, S, PersistenceCommandPolicy>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
    {
        #region Declarations
        /// <summary>
        /// This container holds the memory back entity collection.
        /// </summary>
        protected PersistenceEntityContainer<K, E> mContainer;
        /// <summary>
        /// This is the time span for the delay.
        /// </summary>
        private TimeSpan? mDelay = null;
        #endregion
        #region Constructor
        protected PersistenceManagerHandlerMemory(Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = default(TimeSpan?)
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null)
            : base(keyMaker, keyDeserializer, entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy, resourceProfile, cacheManager, referenceMaker, referenceHashMaker, keySerializer)
        {
        }
        #endregion

        #region Start/Stop
        protected override void StartInternal()
        {
            try
            {
                mContainer = new PersistenceEntityContainer<K, E>();
                PrePopulate();
            }
            catch (Exception ex)
            {

                throw;
            }

            base.StartInternal();
        }

        protected override void StopInternal()
        {
            base.StopInternal();

            mContainer.Clear();
            mContainer = null;
        }
        #endregion

        #region PrePopulate()
        /// <summary>
        /// This method is used to prepopulate entities in to the memory cache.
        /// </summary>
        public virtual void PrePopulate()
        {

        } 
        #endregion

        /// <summary>
        /// This method can be used during testing. It will insert a delay to the task.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public void DiagnosticsSetMessageDelay(TimeSpan? delay)
        {
            mDelay = delay;
        }

        /// <summary>
        /// This override process the Directive command which can be used to instigate test behaviour.
        /// </summary>
        protected override void CommandsRegister()
        {
            base.CommandsRegister();

            //PersistenceCommandRegister<MemoryPersistenceDirectiveRequest, MemoryPersistenceDirectiveResponse>("Directive", ProcessDirective);
        }

        #region Behaviour
        /// <summary>
        /// This is not currently used.
        /// </summary>
        /// <param name="rq"></param>
        /// <param name="rs"></param>
        /// <param name="prq"></param>
        /// <param name="prs"></param>
        /// <returns></returns>
        protected virtual async Task ProcessDirective(PersistenceRequestHolder<MemoryPersistenceDirectiveRequest, MemoryPersistenceDirectiveResponse> holder)
        {
            holder.Rs.ResponseCode = (int)PersistenceResponse.NotImplemented501;
            holder.Rs.ResponseMessage = "Not implemented.";
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
            return mContainer.Add(key, entity, mTransform.ReferenceMaker(entity)); ;
        } 
        #endregion

        #region InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
        /// <summary>
        /// This is the create override for the command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="holder">The entity holder.</param>
        /// <returns></returns>
        protected override async Task<IResponseHolder<E>> InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
        {
            //if (await ProvideTaskDelay(holder.Prq.Cancel))
            //    return new PersistenceResponseHolder<E>(PersistenceResponse.RequestTimeout408);
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
            //if (await ProvideTaskDelay(holder.Prq.Cancel))
            //    return new PersistenceResponseHolder<E>(PersistenceResponse.RequestTimeout408);

            E entity;
            bool success = mContainer.TryGetValue(key, out entity);

            if (success)
            {
                JsonHolder<K> jsonHolder = mTransform.JsonMaker(entity);
                return new PersistenceResponseHolder<E>(PersistenceResponse.Ok200, jsonHolder.Json, mTransform.PersistenceEntitySerializer.Deserializer(jsonHolder.Json));
            }
            else
                return new PersistenceResponseHolder<E>(PersistenceResponse.NotFound404);

        } 
        #endregion

        protected override async Task<IResponseHolder<E>> InternalReadByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, E> holder)
        {
            //if (await ProvideTaskDelay(holder.Prq.Cancel))
            //    return new PersistenceResponseHolder<E>(PersistenceResponse.RequestTimeout408);

            E entity;
            bool success = mContainer.TryGetValue(reference, out entity);

            if (success)
            {
                JsonHolder<K> jsonHolder = mTransform.JsonMaker(entity);
                return new PersistenceResponseHolder<E>(PersistenceResponse.Ok200, jsonHolder.Json, mTransform.PersistenceEntitySerializer.Deserializer(jsonHolder.Json));
            }
            else
                return new PersistenceResponseHolder<E>(PersistenceResponse.NotFound404);

        }

        protected override async Task<IResponseHolder<E>> InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
        {
            //if (await ProvideTaskDelay(holder.Prq.Cancel))
            //    return new PersistenceResponseHolder<E>(PersistenceResponse.RequestTimeout408);

            E oldEntity;
            bool successExisting = mContainer.TryGetValue(key, out oldEntity);
            if (!successExisting)
                return new PersistenceResponseHolder<E>(PersistenceResponse.NotFound404);

            E newEntity = holder.Rq.Entity;


            var ver = mTransform.Version;
            if (ver.SupportsOptimisticLocking)
            {
                if (ver.EntityVersionAsString(oldEntity)!= ver.EntityVersionAsString(newEntity))
                    return new PersistenceResponseHolder<E>(PersistenceResponse.PreconditionFailed412);
            }

            if (ver.SupportsVersioning)
                ver.EntityVersionUpdate(newEntity);


            mContainer.Update(key, newEntity, mTransform.ReferenceMaker(newEntity));

            var jsonHolder = mTransform.JsonMaker(newEntity);
            return new PersistenceResponseHolder<E>(PersistenceResponse.Ok200)
            {
                  Content = jsonHolder.Json
                , IsSuccess = true
                , Entity = newEntity
            };

        }

        protected override async Task<IResponseHolder> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            //if (await ProvideTaskDelay(holder.Prq.Cancel))
            //    return new PersistenceResponseHolder<E>(PersistenceResponse.RequestTimeout408);

            if (!mContainer.Remove(key))
                return new PersistenceResponseHolder(PersistenceResponse.NotFound404);

            return new PersistenceResponseHolder(PersistenceResponse.Ok200);
        }

        protected override async Task<IResponseHolder> InternalDeleteByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            //if (await ProvideTaskDelay(holder.Prq.Cancel))
            //    return new PersistenceResponseHolder<E>(PersistenceResponse.RequestTimeout408);

            if (!mContainer.Remove(reference))
                return new PersistenceResponseHolder(PersistenceResponse.NotFound404);

            return new PersistenceResponseHolder(PersistenceResponse.Ok200);

        }

        protected override async Task<IResponseHolder> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            //if (await ProvideTaskDelay(holder.Prq.Cancel))
            //    return new PersistenceResponseHolder<E>(PersistenceResponse.RequestTimeout408);

            E entity;
            bool success = mContainer.TryGetValue(key, out entity);

            if (success)
            {
                JsonHolder<K> jsonHolder = mTransform.JsonMaker(entity);
                return new PersistenceResponseHolder(PersistenceResponse.Ok200, jsonHolder.Json);
            }
            else
                return new PersistenceResponseHolder(PersistenceResponse.NotFound404);
        }

        protected override async Task<IResponseHolder> InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            //if (await ProvideTaskDelay(holder.Prq.Cancel))
            //    return new PersistenceResponseHolder<E>(PersistenceResponse.RequestTimeout408);

            E entity;
            bool success = mContainer.TryGetValue(reference, out entity);

            if (success)
            {
                JsonHolder<K> jsonHolder = mTransform.JsonMaker(entity);
                return new PersistenceResponseHolder(PersistenceResponse.Ok200, jsonHolder.Json);
            }
            else
                return new PersistenceResponseHolder(PersistenceResponse.NotFound404);

        }

        protected async override Task<IResponseHolder<SearchResponse>> InternalSearch(
            SearchRequest key, PersistenceRequestHolder<SearchRequest, SearchResponse> holder)
        {
            var query = mContainer.Values.AsQueryable<E>();

            Expression expression = mTransform.SearchTranslator.Build(key);

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

    }
}
