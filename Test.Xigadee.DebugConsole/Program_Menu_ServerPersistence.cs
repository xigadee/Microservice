using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sMenuServerPersistence = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                "Persistence"
                , Create(sContext.ServerPersistence)
                , Read(sContext.ServerPersistence)
                , ReadByReference(sContext.ServerPersistence)
                , Update(sContext.ServerPersistence)
                , Delete(sContext.ServerPersistence)
                , DeleteByReference(sContext.ServerPersistence)
                , Version(sContext.ServerPersistence)
                , VersionByReference(sContext.ServerPersistence)
                , Search(sContext.ServerPersistence)
                , StressTest(sContext.ServerPersistence)
                , StressCrudTest(sContext.ServerPersistence)
               )
            );
    }
}
