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
                , Create(sContext.ApiServer)
                , Read(sContext.ApiServer)
                , ReadByReference(sContext.ApiServer)
                , Update(sContext.ApiServer)
                , Delete(sContext.ApiServer)
                , DeleteByReference(sContext.ApiServer)
                , Version(sContext.ApiServer)
                , VersionByReference(sContext.ApiServer)
                , Search(sContext.ApiServer)
                , StressTest(sContext.ApiServer)
                , StressCrudTest(sContext.ApiServer)
               )
            );
    }
}
