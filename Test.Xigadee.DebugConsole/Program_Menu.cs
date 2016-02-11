using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    partial class Program
    {
        static ConsoleMenu sMainMenu;

        static ConsoleMenu sPersistenceMenu;

        static int serverStatus = 0;

        static int clientStatus = 0;

        static Func<int> mPersistenceStatus;

        static void PersistenceLog(string Action, bool success)
        {
            sPersistenceMenu.AddInfoMessage(string.Format("{0} {1}", Action, success ? "OK" : "Fail")
                , true, success ? EventLogEntryType.Information : EventLogEntryType.Error);
        }


        static void Main(string[] args)
        {
            mPersistenceStatus = () => 0;

            var id = new Guid("5ac0802f-7768-433c-bc54-975940964363");
            var value = id.ToByteArray();
            int push = (value[0] >> 6)-1;
            

            var switches = args.CommandArgsParse();
            int processes = switches.ContainsKey("processes") ? int.Parse(switches["processes"]) : Environment.ProcessorCount*4*4*2;

            //var testid = Guid.NewGuid();
            var versionid = Guid.NewGuid();
            var testid = new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1");
            //var testid = Guid.NewGuid();

            sPersistenceMenu = new ConsoleMenu(
                "Persistence"
                , new ConsoleOption("Create document db entity"
                    , (m, o) =>
                    {
                        testid = Guid.NewGuid();
                        var result = sPersistence.Create(new MondayMorningBlues { Id = testid, ContentId = testid, VersionId = versionid, Message = DateTime.Now.ToString(), NotEnoughCoffee = true }, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                        versionid = result.Entity.VersionId;
                        PersistenceLog("Create", result.IsSuccess);
                    }
                    , enabled: (m, o) => mPersistenceStatus() == 2
                    )
                , new ConsoleOption("Read document db entity"
                    , (m, o) =>
                    {
                        var result = sPersistence.Read(testid, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                        PersistenceLog("Read", result.IsSuccess);
                    }
                    , enabled: (m, o) => mPersistenceStatus() == 2
                    )
                , new ConsoleOption("Update document db entity"
                    , (m, o) =>
                    {
                        var result = sPersistence.Update(new MondayMorningBlues() { Id = testid, ContentId = testid, VersionId = versionid, Message = "Hello mom2", NotEnoughCoffee = false }, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                        //var result = sPersistence.Update(new MondayMorningBlues() { ContentId = testid, VersionId = versionid, Message = "Hello mom2", NotEnoughCoffee = false }, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                        PersistenceLog("Update", result.IsSuccess);
                        if (result.IsSuccess)
                        {
                            versionid = result.Entity.VersionId;
                        }
                    }
                    , enabled: (m, o) => mPersistenceStatus() == 2
                    )
                , new ConsoleOption("Delete document db entity"
                    , (m, o) =>
                    {
                        var result = sPersistence.Delete(testid, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5), VersionId = versionid.ToString() }).Result;
                        PersistenceLog("Delete", result.IsSuccess);
                    }
                    , enabled: (m, o) => mPersistenceStatus() == 2
                    )
                    , new ConsoleOption("Create 100000 document db entities async"
                    , (m, o) =>
                    {
                        var batchId = Guid.NewGuid().ToString("N").ToUpperInvariant();
                        int i=0;
                        for (; i < 100000; i++)
                        {
                            try
                            {
                                var result = sPersistence.Create(
                                    new MondayMorningBlues() { ContentId = Guid.NewGuid(), Message = i.ToString(), NotEnoughCoffee = true }
                                    , new RepositorySettings()
                                    {
                                        WaitTime = TimeSpan.FromMinutes(15),
                                        ProcessAsync = true,
                                        BatchId = batchId,
                                        CorrelationId = string.Format("{0}/{1}", batchId, i)
                                    }).Result;
                            }
                            catch (Exception ex)
                            {
                                
                                throw ex;
                            }
                        }

                        PersistenceLog("100000 enqueued", true);
                    }
                    , enabled: (m, o) => mPersistenceStatus() == 2
                    )
                    );

            sMainMenu = new ConsoleMenu(
                "Xigadee Microservice Scrathpad Test Console"
                , new ConsoleOption("Start client (internal)"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceClient(processes, false));
                    }
                    , enabled: (m, o) => clientStatus == 0
                )
                , new ConsoleOption("Start client (external)"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceClient(processes, true));
                    }
                    , enabled: (m, o) => clientStatus == 0
                )
                , new ConsoleOption("Start client (internal and external)"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceClient(processes));
                    }
                    , enabled: (m, o) => clientStatus == 0
                )
                , new ConsoleOption("Stop client"
                    , (m, o) =>
                    {
                        Task.Run(() => client.Stop());
                    }
                    , enabled: (m, o) => clientStatus == 2
                   )
                , new ConsoleOption("Start server"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceServer(processes));
                    }
                    , enabled: (m, o) => serverStatus == 0
                )
                , new ConsoleOption("Start server (internal stress test)"
                    , (m, o) =>
                    {
                        Task.Run(() => InitialiseMicroserviceServerStressTest(processes));
                    }
                    , enabled: (m, o) => serverStatus == 0
                )
                , new ConsoleOption("Stop server"
                    , (m, o) =>
                    {
                        Task.Run(() => server.Stop());
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

            sMainMenu.Show(args, 9);
        }

    }
}
