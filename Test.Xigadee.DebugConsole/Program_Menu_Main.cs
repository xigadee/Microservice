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
                    , enabled: (m, o) => sContext.Client.Status == 0 && sContext.Server.Status == 0
                )
                , new ConsoleOption("Set Persistence storage options"
                    , (m, o) =>
                    {
                    }
                    , enabled: (m, o) => sContext.Server.Status == 0
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
                        Task.Run(() => sContext.Client.Stop());
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
                        Task.Run(() => sContext.Server.Stop());
                        return true;
                    }
                    , shortcut: "startserver"
                )
                , new ConsoleOption("Client Persistence methods"
                    , (m, o) =>
                    {
                        //sContext.ClientPersistence = () => sContext.Client.Status;
                    }
                    , childMenu: sPersistenceMenuClient.Value
                    , enabled: (m, o) => sContext.Client.Status == 2
                )
                , new ConsoleOption("API Client Persistence methods"
                    , (m, o) =>
                    {
                        //sContext.ApiPersistenceStatus = () => 2;
                    }
                    , childMenu: sPersistenceMenuApi.Value
                    , enabled: (m, o) => sContext.ApiPersistence != null
                )
                , new ConsoleOption("Server Shared Service Persistence methods"
                    , (m, o) =>
                    {
                        //sContext.PersistenceStatus = () => sContext.Server.Status;
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
                , Search(sContext.ServerPersistence)
                , StressTest(sContext.ServerPersistence)
                , StressCrudTest(sContext.ServerPersistence)
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

    }
}
