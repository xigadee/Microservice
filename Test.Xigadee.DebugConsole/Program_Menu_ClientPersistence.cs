using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sMenuClientPersistence = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
               "Persistence"
                    , Create(sContext.Client)
                    , Read(sContext.Client)
                    , ReadByReference(sContext.Client)
                    , Update(sContext.Client)
                    , Delete(sContext.Client)
                    , DeleteByReference(sContext.Client)
                    , Version(sContext.Client)
                    , VersionByReference(sContext.Client)
                    , StressTest(sContext.Client)
                   )
                );
    }
}
