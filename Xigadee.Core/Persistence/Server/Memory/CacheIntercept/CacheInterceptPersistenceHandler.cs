﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Xigadee
//{
//    /// <summary>
//    /// This message handler is used to intercept persistence messages and to return a memory cached 
//    /// version of the entity if there is one stored. For messages that cannot be cached, the original message will be 
//    /// passed to the actual persistence manager.
//    /// </summary>
//    /// <typeparam name="K">The key type.</typeparam>
//    /// <typeparam name="E">The entity type.</typeparam>
//    public class CacheInterceptPersistenceHandler<K, E>: PersistenceMessageHandlerBase<K, E, PersistenceStatistics>
//        where K : IEquatable<K>
//    {
//        protected override E EntityMaker(string data)
//        {
//            throw new NotImplementedException();
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
//    }
//}
