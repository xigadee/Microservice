//#region using
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks; 
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This persistence class is used to hold entities in memory during the lifetime of the 
//    /// Microservice and does not persist to any backing store.
//    /// This class is used extensively by the Unit test projects. The class inherits from Json base
//    /// and so employs the same logic as that used by the Azure Storage and DocumentDb persistence classes.
//    /// </summary>
//    /// <typeparam name="K">The key type.</typeparam>
//    /// <typeparam name="E">The entity type.</typeparam>
//    public class PersistenceManagerHandlerMemory<K, E, S>: PersistenceManagerHandlerJsonBase<K, E, S, PersistenceCommandPolicy>
//        where K : IEquatable<K>
//        where S : PersistenceStatistics, new()
//    {
//        #region Declarations
//        /// <summary>
//        /// This container holds the entities.
//        /// </summary>
//        protected ConcurrentDictionary<K, JsonHolder<K>> mContainer;
//        /// <summary>
//        /// This container holds the key references.
//        /// </summary>
//        protected ConcurrentDictionary<string, K> mContainerReference;
//        #endregion

//        /// <summary>
//        /// Constructor.
//        /// </summary>
//        public PersistenceManagerHandlerMemory(
//            string entityName = null
//            , VersionPolicy<E> versionPolicy = null
//            , TimeSpan? defaultTimeout = null
//            , PersistenceRetryPolicy persistenceRetryPolicy = null
//            , ResourceProfile resourceProfile = null)
//            :base(entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy, resourceProfile)
//        {

//        }

//        protected override void StartInternal()
//        {
//            mContainer = new ConcurrentDictionary<K, JsonHolder<K>>();
//            mContainerReference = new ConcurrentDictionary<string, K>();
//            base.StartInternal();
//        }

//        protected override void StopInternal()
//        {
//            mContainerReference.Clear();
//            mContainer.Clear();
//            mContainerReference = null;
//            mContainer = null;
//            base.StopInternal();
//        }

//        protected override Task ProcessCreate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            throw new NotImplementedException();
//        }

//        protected override Task ProcessDelete(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            throw new NotImplementedException();
//        }

//        protected override Task ProcessDeleteByRef(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            throw new NotImplementedException();
//        }

//        protected override Task ProcessRead(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            throw new NotImplementedException();
//        }

//        protected override Task ProcessReadByRef(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            throw new NotImplementedException();
//        }

//        protected override Task ProcessUpdate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            throw new NotImplementedException();
//        }

//        protected override Task ProcessVersion(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            throw new NotImplementedException();
//        }

//        protected override Task ProcessVersionByRef(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            throw new NotImplementedException();
//        }

//        protected override void ProcessOutputEntity(K key, PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, IResponseHolder holderResponse)
//        {
//            throw new NotImplementedException();
//        }

//        protected override void ProcessOutputKey(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, IResponseHolder holderResponse)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
