using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sServerStressTestsMenu = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                $"Server Stress Tests ({sContext.PersistenceType.ToString()})"
                //, new ConsoleOption("Sql based"
                //    , (m, o) =>
                //    {
                //        sContext.PersistenceType = PersistenceOptions.Sql;
                //    }
                //    , enabled: (m, o) => clientStatus == 0

                //)
                , new ConsoleOption("DocumentDb based"
                    , (m, o) =>
                    {
                        sContext.PersistenceType = PersistenceOptions.DocumentDb;
                    }
                    , enabled: (m, o) => true
                )
                , new ConsoleOption("Blob storage based"
                    , (m, o) =>
                    {
                        sContext.PersistenceType = PersistenceOptions.Blob;
                    }
                    , enabled: (m, o) => true
                )
                , new ConsoleOption("Redis Cache based"
                    , (m, o) =>
                    {
                        sContext.PersistenceType = PersistenceOptions.RedisCache;
                    }
                    , enabled: (m, o) => true
                )
                )
            );
    }
}
