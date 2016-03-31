using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static Lazy<ConsoleMenu> sPersistenceMenuClient = new Lazy<ConsoleMenu>(
            () => new ConsoleMenu(
           "Persistence"
            , new ConsoleOption("Create entity"
            , (m, o) =>
                {
                    sContext.Testid = Guid.NewGuid();
                    sContext.TestByRef = $"paul+{sContext.Testid.ToString("N")}@hotmail.com";

                    var result = sContext.Client.Persistence.Create(
                        new MondayMorningBlues
                        {
                            Id = sContext.Testid,
                            ContentId = sContext.Testid,
                            VersionId = sContext.Versionid,
                            Message = DateTime.Now.ToString(),
                            NotEnoughCoffee = true,
                            Email = sContext.TestByRef
                        }
                        , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;

                    if (result.IsSuccess)
                        sContext.Versionid = result.Entity.VersionId;

                    PersistenceLog(m, "Create", result.IsSuccess);
                }
               , enabled: (m, o) => sContext.PersistenceStatus() == 2
               )
           , new ConsoleOption("Read entity"
               , (m, o) =>
               {
                   var result = sContext.Client.Persistence.Read(sContext.Testid, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog(m, "Read", result.IsSuccess);
               }
               , enabled: (m, o) => sContext.PersistenceStatus() == 2
               )
           , new ConsoleOption("Read entity by reference"
               , (m, o) =>
               {
                   var result = sContext.Client.Persistence.ReadByRef("EMAIL", sContext.TestByRef, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog(m, "Read", result.IsSuccess);
               }
               , enabled: (m, o) => sContext.PersistenceStatus() == 2
               )
           , new ConsoleOption("Update entity"
               , (m, o) =>
               {
                   var result = sContext.Client.Persistence.Update(
                       new MondayMorningBlues()
                       {
                           Id = sContext.Testid,
                           ContentId = sContext.Testid,
                           VersionId = sContext.Versionid,
                           Message = "Hello mom2",
                           NotEnoughCoffee = false,
                           Email = sContext.TestByRef
                       }
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) })
                       .Result;

                    PersistenceLog(m, "Update", result.IsSuccess);

                    if (result.IsSuccess)
                    {
                        sContext.Versionid = result.Entity.VersionId;
                    }
               }
               , enabled: (m, o) => sContext.PersistenceStatus() == 2
               )
           , new ConsoleOption("Delete entity"
               , (m, o) =>
               {
                   var result = sContext.Client.Persistence.Delete(sContext.Testid, 
                       new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5), VersionId = sContext.Versionid.ToString() }).Result;
                   PersistenceLog(m, "Delete", result.IsSuccess);
               }
               , enabled: (m, o) => sContext.PersistenceStatus() == 2
               )
           , new ConsoleOption("Delete entity by reference"
               , (m, o) =>
               {
                   var result = sContext.Client.Persistence.DeleteByRef("EMAIL", sContext.TestByRef, new RepositorySettings { WaitTime = TimeSpan.FromMinutes(5), VersionId = sContext.Versionid.ToString() }).Result;
                   PersistenceLog(m, "Delete", result.IsSuccess);
               }
               , enabled: (m, o) => sContext.PersistenceStatus() == 2
               )
           , new ConsoleOption("Version entity"
               , (m, o) =>
               {
                   var result = sContext.Client.Persistence.Version(sContext.Testid,
                       new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5), VersionId = sContext.Versionid.ToString() }).Result;
                   PersistenceLog(m, "Version", result.IsSuccess);
               }
               , enabled: (m, o) => sContext.PersistenceStatus() == 2
               )
           , new ConsoleOption("Version entity by reference"
               , (m, o) =>
               {
                   var result = sContext.Client.Persistence.VersionByRef("EMAIL", sContext.TestByRef, new RepositorySettings { WaitTime = TimeSpan.FromMinutes(5), VersionId = sContext.Versionid.ToString() }).Result;
                   PersistenceLog(m, "Version By Ref", result.IsSuccess);
               }
               , enabled: (m, o) => sContext.PersistenceStatus() == 2
               )
           , new ConsoleOption("Create 100000 entities async"
               , (m, o) =>
               {
                   var batchId = Guid.NewGuid().ToString("N").ToUpperInvariant();
                   int i = 0;
                   for (; i < 100000; i++)
                   {
                       try
                       {
                           var result = sContext.Client.Persistence.Create(
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

                   PersistenceLog(m, "100000 enqueued", true);
               }
               , enabled: (m, o) => sContext.PersistenceStatus() == 2
               )
               )
            );

    }
}
