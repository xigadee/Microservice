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

        static Lazy<ConsoleMenu> sMenuApiPersistence = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
               "Persistence"
                    , Create(sContext.ApiServer)
                    , Read(sContext.ApiServer)
                    , ReadByReference(sContext.ApiServer)
                    , Update(sContext.ApiServer)
                    , Delete(sContext.ApiServer)
                    , DeleteByReference(sContext.ApiServer)
                    , Version(sContext.ApiServer)
                    , VersionByReference(sContext.ApiServer)
                    , StressTest(sContext.ApiServer)

                   )
                );
    }
}
