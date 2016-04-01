using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sPersistenceMenuServer = new Lazy<ConsoleMenu>(
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
            , StressTest(sContext.ServerPersistence)

           )
        );

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

        static Lazy<ConsoleMenu> sMainMenu = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                "Xigadee Microservice Scrathpad Test Console"
                , new ConsoleOption("Load Settings"
                    , (m, o) =>
                    {
                        MicroserviceLoadSettings();
                    }
                    , enabled: (m, o) => sContext.Client.Status == 0 && sContext.Server.Status == 0
                )
                , new ConsoleOption("Start client"
                    , (m, o) =>
                    {
                        Task.Run(() => MicroserviceClientStart());
                    }
                    , enabled: (m, o) => sContext.Client.Status == 0
                )
                , new ConsoleOption("Stop client"
                    , (m, o) =>
                    {
                        Task.Run(() => sContext.Client.Stop());
                    }
                    , enabled: (m, o) => sContext.Client.Status == 2
                )
                , new ConsoleOption("Set Persistence storage options"
                    , (m, o) =>
                    {
                    }
                    , enabled: (m, o) => sContext.Server.Status == 0
                    , childMenu: sPersistenceSettingsMenu.Value
                )
                , new ConsoleOption("Start server"
                    , (m, o) =>
                    {
                        Task.Run(() => MicroserviceServerStart());
                    }
                    , enabled: (m, o) => sContext.Server.Status == 0
                )
                , new ConsoleOption("Stop server"
                    , (m, o) =>
                    {
                        Task.Run(() => sContext.Server.Stop());
                    }
                    , enabled: (m, o) => sContext.Server.Status == 2
                )
                , new ConsoleOption("Client Persistence methods"
                    , (m, o) =>
                    {
                        sContext.PersistenceStatus = () => sContext.Client.Status;
                    }
                    , childMenu: sPersistenceMenuClient.Value
                    , enabled: (m, o) => sContext.Client.Status == 2
                )
                , new ConsoleOption("Server Shared Service Persistence methods"
                    , (m, o) =>
                    {
                        sContext.PersistenceStatus = () => sContext.Server.Status;
                    }
                    , childMenu: sPersistenceMenuServer.Value
                    , enabled: (m, o) => sContext.Server.Status == 2
                )
                , new ConsoleOption("Client Stress Tests"
                    , (m, o) =>
                    {
                    }
                    , childMenu: sClientStressTestsMenu.Value
                    , enabled: (m, o) => sContext.Client.Status == 2
                )
                , new ConsoleOption("Server Stress Tests"
                    , (m, o) =>
                    {
                    }
                    , childMenu: sClientStressTestsMenu.Value
                    , enabled: (m, o) => sContext.Server.Status == 2
                )
            ));
    }
}
