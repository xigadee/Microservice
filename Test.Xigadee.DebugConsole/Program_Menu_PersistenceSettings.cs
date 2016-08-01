using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sPersistenceSettingsMenu = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                $"Persistence store options"
                , new ConsoleOption("Sql based Persistence"
                    , (m, o) =>
                    {
                        sServerContext.PersistenceType = PersistenceOptions.Sql;
                    }
                    , enabled: (m, o) => true
                    , selected: (m, o) => sServerContext.PersistenceType == PersistenceOptions.Sql
                )
                , new ConsoleOption("DocumentDb based Persistence"
                    , (m, o) =>
                    {
                        sServerContext.PersistenceType = PersistenceOptions.DocumentDb;
                    }
                    , enabled: (m, o) => true
                    , selected: (m, o) => sServerContext.PersistenceType == PersistenceOptions.DocumentDb
                )
                , new ConsoleOption("DocumentDb Sdk based Persistence"
                    , (m, o) =>
                    {
                        sServerContext.PersistenceType = PersistenceOptions.DocumentDbSdk;
                    }
                    , enabled: (m, o) => true
                    , selected: (m, o) => sServerContext.PersistenceType == PersistenceOptions.DocumentDbSdk
                )
                , new ConsoleOption("Blob storage based Persistence"
                    , (m, o) =>
                    {
                        sServerContext.PersistenceType = PersistenceOptions.Blob;
                    }
                    , enabled: (m, o) => true
                    , selected: (m, o) => sServerContext.PersistenceType == PersistenceOptions.Blob
                )
                , new ConsoleOption("Redis Cache based Persistence"
                    , (m, o) =>
                    {
                        sServerContext.PersistenceType = PersistenceOptions.RedisCache;
                    }
                    , enabled: (m, o) => true
                    , selected: (m, o) => sServerContext.PersistenceType == PersistenceOptions.RedisCache
                )
                , new ConsoleOption("Memory based Persistence"
                    , (m, o) =>
                    {
                        sServerContext.PersistenceType = PersistenceOptions.Memory;
                    }
                    , enabled: (m, o) => true
                    , selected: (m, o) => sServerContext.PersistenceType == PersistenceOptions.Memory
                )
                , new ConsoleOption("Client RedisCache enabled"
                    , (m, o) =>
                    {
                        sServerContext.ClientCacheEnabled = !sServerContext.ClientCacheEnabled;
                    }
                    , enabled: (m, o) => true
                    , selected: (m, o) => sServerContext.ClientCacheEnabled
                )
                , new ConsoleOption("Server RedisCache enabled"
                    , (m, o) =>
                    {
                        sServerContext.ServerCacheEnabled = !sServerContext.ServerCacheEnabled;
                    }
                    , enabled: (m, o) => true
                    , selected: (m, o) => sServerContext.ServerCacheEnabled
                )
                )
            );
    }
}
