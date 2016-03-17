using Xigadee;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
namespace Test.Xigadee
{
    static partial class Program
    {
        static ConsoleMenu sPersistenceSettingsMenu = new ConsoleMenu(
           "Persistence"
            , new ConsoleOption("Create entity"
            , (m, o) =>
            {
                sState.Testid = Guid.NewGuid();
                var result = sService.Persistence.Create(
                    new MondayMorningBlues { Id = sState.Testid, ContentId = sState.Testid, VersionId = sState.Versionid, Message = DateTime.Now.ToString(), NotEnoughCoffee = true, Email = "paul@hotmail.com" }
                    , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;

                if (result.IsSuccess)
                    sState.Versionid = result.Entity.VersionId;

                PersistenceLog("Create", result.IsSuccess);
            }
               , enabled: (m, o) => mPersistenceStatus() == 2
               )
           , new ConsoleOption("Read entity"
               , (m, o) =>
               {
                   var result = sService.Persistence.Read(sState.Testid, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog("Read", result.IsSuccess);
               }
               , enabled: (m, o) => mPersistenceStatus() == 2
               )
           , new ConsoleOption("Read entity by reference"
               , (m, o) =>
               {
                   var result = sService.Persistence.ReadByRef("email", "paul@hotmail.com", new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog("Read", result.IsSuccess);
               }
               , enabled: (m, o) => mPersistenceStatus() == 2
               )
           , new ConsoleOption("Update entity"
               , (m, o) =>
               {
                   var result = sService.Persistence.Update(new MondayMorningBlues() { Id = sState.Testid, ContentId = sState.Testid, VersionId = sState.Versionid, Message = "Hello mom2", NotEnoughCoffee = false }, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                           //var result = sPersistence.Update(new MondayMorningBlues() { ContentId = testid, VersionId = versionid, Message = "Hello mom2", NotEnoughCoffee = false }, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                           PersistenceLog("Update", result.IsSuccess);
                   if (result.IsSuccess)
                   {
                       sState.Versionid = result.Entity.VersionId;
                   }
               }
               , enabled: (m, o) => mPersistenceStatus() == 2
               )
           , new ConsoleOption("Delete entity"
               , (m, o) =>
               {
                   var result = sService.Persistence.Delete(sState.Testid, new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5), VersionId = sState.Versionid.ToString() }).Result;
                   PersistenceLog("Delete", result.IsSuccess);
               }
               , enabled: (m, o) => mPersistenceStatus() == 2
               )
           , new ConsoleOption("Delete entity by reference"
               , (m, o) =>
               {
                   var result = sService.Persistence.DeleteByRef("email", "paul@hotmail.com", new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5), VersionId = sState.Versionid.ToString() }).Result;
                   PersistenceLog("Delete", result.IsSuccess);
               }
               , enabled: (m, o) => mPersistenceStatus() == 2
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
                           var result = sService.Persistence.Create(
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

    }
}
