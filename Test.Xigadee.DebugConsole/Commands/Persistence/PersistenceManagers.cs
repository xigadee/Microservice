using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    public class PersistenceBlah2: PersistenceManagerHandlerDocumentDb<Guid, Blah2>
    {
        public PersistenceBlah2(DocumentDbConnection connection, string database, Func<Blah2, Guid> keyMaker
            , Func<RepositoryHolder<Guid, Blah2>, JsonHolder<Guid>> jsonMaker = null
            , string databaseCollection = null
            , string entityName = null
            , VersionPolicy<Blah2> versionMaker = null
            , TimeSpan? defaultTimeout = default(TimeSpan?)
            , ShardingPolicy<Guid> shardingPolicy = null
            , ResourceProfile resourceProfile = null) 

            : base(connection, database, keyMaker, jsonMaker, databaseCollection, entityName, versionMaker, defaultTimeout, shardingPolicy
                  , resourceProfile:resourceProfile)
        {
        }
    }

    public class PersistenceMondayMorningBlues: PersistenceMessageHandlerRedisCache<Guid, MondayMorningBlues>
    {
        public PersistenceMondayMorningBlues(string redisConnection, VersionPolicy<MondayMorningBlues> versionPolicy = null) 
            :base(redisConnection, (k) => k.Id, versionPolicy: versionPolicy)
        {

        }
    }

    //public class PersistenceMondayMorningBlues: PersistenceManagerHandlerDocumentDb<Guid, MondayMorningBlues>
    //public class PersistenceMondayMorningBlues: PersistenceMessageHandlerRedisCache<Guid, MondayMorningBlues>
    //{
    //    public PersistenceMondayMorningBlues(DocumentDbConnection connection, string database, Func<MondayMorningBlues, Guid> keyMaker, 
    //        Func<RepositoryHolder<Guid, MondayMorningBlues>, JsonHolder<Guid>> jsonMaker = null, 
    //        string databaseCollection = null, string entityName = null, 
    //        VersionPolicy<MondayMorningBlues> versionMaker = null, 
    //        TimeSpan? defaultTimeout = default(TimeSpan?), 
    //        ShardingPolicy<Guid> shardingPolicy = null,
    //        ResourceProfile resourceProfile = null) 
    //        : base(connection, database, keyMaker, jsonMaker, databaseCollection, entityName, versionMaker, defaultTimeout, shardingPolicy, resourceProfile: resourceProfile)
    //    {
    //    }

    //}

    public class PersistenceMondayMorningBlues2: PersistenceMessageHandlerAzureBlobStorageBase<Guid, MondayMorningBlues>
    {
        public PersistenceMondayMorningBlues2(StorageCredentials credentials, VersionPolicy<MondayMorningBlues> versionMaker = null,
            ResourceProfile resourceProfile = null)
            : base(credentials, (b) => b.ContentId, (k)=>k.ToString("N").ToUpperInvariant(), defaultTimeout: TimeSpan.FromSeconds(1)
                  , versionPolicy: versionMaker
                  , resourceProfile:resourceProfile)
        {
        }
    }
}
