using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sClientStressTestsMenu = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                $"Client Stress Tests({sContext.PersistenceType.ToString()})"
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
                        sClientStressTestsMenu.Value.Refresh();
                    }
                    , enabled: (m, o) => true
                )
                , new ConsoleOption("Blob storage based"
                    , (m, o) =>
                    {
                        sContext.PersistenceType = PersistenceOptions.Blob;
                        sClientStressTestsMenu.Value.Refresh();
                    }
                    , enabled: (m, o) => true
                )
                , new ConsoleOption("Redis Cache based"
                    , (m, o) =>
                    {
                        sContext.PersistenceType = PersistenceOptions.RedisCache;
                        sClientStressTestsMenu.Value.Refresh();
                    }
                    , enabled: (m, o) => true
                )
                )
            );
    }
}
