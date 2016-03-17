using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static ConsoleMenu sMainMenu = new ConsoleMenu(
                "Xigadee Microservice Scrathpad Test Console"
                , new ConsoleOption("Start client (internal)"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceClient(false));
                    }
                    , enabled: (m, o) => clientStatus == 0
                )
                , new ConsoleOption("Start client (external)"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceClient(true));
                    }
                    , enabled: (m, o) => clientStatus == 0
                )
                , new ConsoleOption("Start client (internal and external)"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceClient());
                    }
                    , enabled: (m, o) => clientStatus == 0
                )
                , new ConsoleOption("Stop client"
                    , (m, o) =>
                    {
                        Task.Run(() => sClient.Stop());
                    }
                    , enabled: (m, o) => clientStatus == 2
                   )
                , new ConsoleOption("Set Persistence options"
                    , (m, o) =>
                    {
                    }
                    , enabled: (m, o) => serverStatus == 0
                    , childMenu: sPersistenceMenu
                )
                , new ConsoleOption("Start server"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceServer());
                    }
                    , enabled: (m, o) => serverStatus == 0
                )
                , new ConsoleOption("Stop server"
                    , (m, o) =>
                    {
                        Task.Run(() => sServer.Stop());
                    }
                    , enabled: (m, o) => serverStatus == 2
                   )
                , new ConsoleOption("Client Persistence methods"
                , (m, o) =>
                {
                    mPersistenceStatus = () => clientStatus;
                }
                , childMenu: sPersistenceMenu
                , enabled: (m, o) => clientStatus == 2)
                , new ConsoleOption("Server Shared Service Persistence methods"
                , (m, o) =>
                {
                    mPersistenceStatus = () => serverStatus;
                }
                , childMenu: sPersistenceMenu
                , enabled: (m, o) => serverStatus == 2)
                );
    }
}
