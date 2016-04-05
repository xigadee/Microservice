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
                $"Server Stress Tests ({sServerContext.PersistenceType.ToString()})"
                , new ConsoleOption("Stress test 1"
                    , (m, o) =>
                    {
                        Task.Run(() => StressTest1());
                    }
                    , enabled: (m, o) => true
                )
                )
            );

        static void StressTest1()
        {

        }
    }
}
