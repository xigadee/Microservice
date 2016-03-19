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
                "Xigadee Microservice Scrathpad Test Console"
                , new ConsoleOption("Load Settings"
                    , (m, o) =>
                    {
                    }
                    , enabled: (m, o) => sContext.Client.Status == 0
                )
                , new ConsoleOption("Start client"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceClient());
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
                        Task.Run(() => InitialiseMicroserviceServer());
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
                    , childMenu: sPersistenceMenu.Value
                    , enabled: (m, o) => sContext.Client.Status == 2
                )
                , new ConsoleOption("Server Shared Service Persistence methods"
                    , (m, o) =>
                    {
                        sContext.PersistenceStatus = () => sContext.Server.Status;
                    }
                    , childMenu: sPersistenceMenu.Value
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
