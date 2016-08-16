using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sMainMenu = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                "Xigadee Microservice Scratchpad Test Console"
                , new ConsoleOption("Load Settings"
                    , (m, o) =>
                    {
                        MicroserviceLoadSettings();
                    }
                    , enabled: (m, o) => sServerContext.Client.Status == 0 && sServerContext.Server.Status == 0
                )
                , new ConsoleOption("Set Persistence storage options"
                    , (m, o) =>
                    {
                    }
                    , enabled: (m, o) => sServerContext.Server.Status == 0
                    , childMenu: sPersistenceSettingsMenu.Value
                )                
                , new ConsoleSwitchOption(
                    "Start client", (m, o) =>
                    {
                        Task.Run(() => MicroserviceClientStart());
                        return true;
                    }
                    , "Stop client", (m, o) =>
                    {
                        Task.Run(() => sServerContext.Client.Stop());
                        return true;
                    }
                    ,shortcut: "startclient"
                )
                , new ConsoleSwitchOption(
                    "Start WebAPI client", (m, o) =>
                    {
                        Task.Run(() => MicroserviceWebAPIStart());
                        return true;
                    }
                    , "Stop WebAPI client", (m, o) =>
                    {
                        MicroserviceWebAPIStop();
                        return true;
                    }
                    , shortcut: "startapi"
                )
                , new ConsoleSwitchOption(
                    "Start server", (m, o) =>
                    {
                        Task.Run(() => MicroserviceServerStart());
                        return true;
                    }
                    ,"Stop server", (m, o) =>
                    {
                        Task.Run(() => sServerContext.Server.Stop());
                        return true;
                    }
                    , shortcut: "startserver"
                )
                , new ConsoleOption("Client Persistence methods"
                    , (m, o) =>
                    {
                        sServerContext.PersistenceStatus = () => sServerContext.Client.Status;
                    }
                    , childMenu: sPersistenceMenuClient.Value
                    , enabled: (m, o) => sServerContext.Client.Status == 2
                )
                , new ConsoleOption("API Client Persistence methods"
                    , (m, o) =>
                    {
                        sServerContext.PersistenceStatus = () => sServerContext.Client.Status;
                    }
                    , childMenu: sPersistenceMenuClient.Value
                    , enabled: (m, o) => sServerContext.ApiServer != null
                )
                , new ConsoleOption("Server Shared Service Persistence methods"
                    , (m, o) =>
                    {
                        sServerContext.PersistenceStatus = () => sServerContext.Server.Status;
                    }
                    , childMenu: sPersistenceMenuServer.Value
                    , enabled: (m, o) => sServerContext.Server.Status == 2
                )
                , new ConsoleOption("Client Stress Tests"
                    , (m, o) =>
                    {
                    }
                    , childMenu: sClientStressTestsMenu.Value
                    , enabled: (m, o) => sServerContext.Client.Status == 2
                )
                , new ConsoleOption("Server Stress Tests"
                    , (m, o) =>
                    {
                    }
                    , childMenu: sClientStressTestsMenu.Value
                    , enabled: (m, o) => sServerContext.Server.Status == 2
                )
            ));

        static Lazy<ConsoleMenu> sPersistenceMenuServer = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
                "Persistence"
                , Create(sServerContext.ServerPersistence)
                , Read(sServerContext.ServerPersistence)
                , ReadByReference(sServerContext.ServerPersistence)
                , Update(sServerContext.ServerPersistence)
                , Delete(sServerContext.ServerPersistence)
                , DeleteByReference(sServerContext.ServerPersistence)
                , Version(sServerContext.ServerPersistence)
                , VersionByReference(sServerContext.ServerPersistence)
                , Search(sServerContext.ServerPersistence)
                , StressTest(sServerContext.ServerPersistence)
                , StressCrudTest(sServerContext.ServerPersistence)
               )
            );

        static Lazy<ConsoleMenu> sPersistenceMenuClient = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
               "Persistence"
                    , Create(sServerContext.ClientPersistence)
                    , Read(sServerContext.ClientPersistence)
                    , ReadByReference(sServerContext.ClientPersistence)
                    , Update(sServerContext.ClientPersistence)
                    , Delete(sServerContext.ClientPersistence)
                    , DeleteByReference(sServerContext.ClientPersistence)
                    , Version(sServerContext.ClientPersistence)
                    , VersionByReference(sServerContext.ClientPersistence)
                    , StressTest(sServerContext.ClientPersistence)

                   )
                );
    }
}
