using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sPersistenceMenuClient = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
               "Persistence"
                    , Create(sContext.ClientPersistence)
                    , Read(sContext.ClientPersistence)
                    , ReadByReference(sContext.ClientPersistence)
                    , Update(sContext.ClientPersistence)
                    , Delete(sContext.ClientPersistence)
                    , DeleteByReference(sContext.ClientPersistence)
                    , Version(sContext.ClientPersistence)
                    , VersionByReference(sContext.ClientPersistence)
                    , StressTest(sContext.ClientPersistence)
                   )
                );
    }
}
