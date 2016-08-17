using Xigadee;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    static partial class Program
    {

        static Lazy<ConsoleMenu> sPersistenceMenuApi = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
               "Persistence"
                    , Create(sContext.ApiPersistence)
                    , Read(sContext.ApiPersistence)
                    , ReadByReference(sContext.ApiPersistence)
                    , Update(sContext.ApiPersistence)
                    , Delete(sContext.ApiPersistence)
                    , DeleteByReference(sContext.ApiPersistence)
                    , Version(sContext.ApiPersistence)
                    , VersionByReference(sContext.ApiPersistence)
                    , StressTest(sContext.ApiPersistence)

                   )
                );
    }
}
