using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {

        static ConsoleMenu sPersistenceMenu = new ConsoleMenu(
                "Xigadee Microservice Persistence options"
                , new ConsoleOption("Sql based"
                , (m, o) =>
                {

                }
                , enabled: (m, o) => clientStatus == 0)
                , new ConsoleOption("DocumentDb based"
                , (m, o) =>
                {

                }
                , enabled: (m, o) => clientStatus == 0)
                , new ConsoleOption("Blob storage based"
                , (m, o) =>
                {

                }
                , enabled: (m, o) => clientStatus == 0)
                , new ConsoleOption("Redis Cache based"
                , (m, o) =>
                {

                }
                , enabled: (m, o) => clientStatus == 0)
                );

    }
}
