using Xigadee;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Test.Xigadee
{
    static partial class Program
    {
        static void PersistenceLog(ConsoleMenu menu, string action, bool success)
        {
            menu.AddInfoMessage($"{action} {(success ? "OK" : "Fail")}"
                , true, success ? EventLogEntryType.Information : EventLogEntryType.Error);
        }

        static ConsoleOption Create(IPopulatorConsole repo)
        {
            return new ConsoleOption("Create entity"
            , (m, o) =>
            {
                sContext.EntityId = Guid.NewGuid();

                var result = repo.Persistence.Create(CreateEntity(sContext.EntityId, email: sContext.EntityReference)
                    , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5), Source = "Xigadee"}).Result;

                if (result.IsSuccess)
                    sContext.EntityVersionid = result.Entity.VersionId;

                PersistenceLog(m, "Create", result.IsSuccess);
            });
        }

        static ConsoleOption Read(IPopulatorConsole repo)
        {
            return new ConsoleOption("Read entity"
               , (m, o) =>
               {
                   var result = repo.Persistence.Read(sContext.EntityId
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog(m, "Read", result.IsSuccess);
               });
        }

        static ConsoleOption ReadByReference(IPopulatorConsole repo)
        {
            return new ConsoleOption("Read entity by reference"
               , (m, o) =>
               {
                   var result = repo.Persistence.ReadByRef("email", sContext.EntityReference
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog(m, "Read By Reference", result.IsSuccess);
               });
        }

        static ConsoleOption Update(IPopulatorConsole repo)
        {
            return new ConsoleOption("Update entity"
               , (m, o) =>
               {
                   var result = repo.Persistence.Update(
                       new MondayMorningBlues()
                       {
                           Id = sContext.EntityId,
                           ContentId = new Guid(),
                           VersionId = sContext.EntityVersionid,
                           Message = $"Hello mom2 -{DateTime.Now.ToString()}",
                           NotEnoughCoffee = false,
                           NotEnoughSleep = false,
                           Email = sContext.EntityReference
                       }
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) })
                       .Result;

                   PersistenceLog(m, "Update", result.IsSuccess);

                   if (result.IsSuccess)
                   {
                       sContext.EntityVersionid = result.Entity.VersionId;
                   }
               });
        }

        static ConsoleOption Delete(IPopulatorConsole repo)
        {
            return new ConsoleOption("Delete entity"
               , (m, o) =>
               {
                   var result = repo.Persistence.Delete(sContext.EntityId
                       , new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sContext.EntityVersionid.ToString()
                       }).Result;
                   PersistenceLog(m, "Delete", result.IsSuccess);
               });
        }

        static ConsoleOption DeleteByReference(IPopulatorConsole repo)
        {
            return new ConsoleOption("Delete entity by reference"
               , (m, o) =>
               {
                   var result = repo.Persistence.DeleteByRef("email", sContext.EntityReference,
                       new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sContext.EntityVersionid.ToString()
                       }).Result;
                   PersistenceLog(m, "Delete By Reference", result.IsSuccess);
               });
        }

        static ConsoleOption Version(IPopulatorConsole repo)
        {
            return new ConsoleOption("Version entity"
               , (m, o) =>
               {
                   var result = repo.Persistence.Version(sContext.EntityId,
                       new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sContext.EntityVersionid.ToString()
                       }).Result;
                   PersistenceLog(m, "Version", result.IsSuccess);
               });
        }

        static ConsoleOption VersionByReference(IPopulatorConsole repo)
        {
            return new ConsoleOption("Version entity by reference"
               , (m, o) =>
               {
                   var result = repo.Persistence.VersionByRef("EMAIL", sContext.EntityReference
                       , new RepositorySettings
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sContext.EntityVersionid.ToString()
                       }).Result;

                   PersistenceLog(m, "Version By Reference", result.IsSuccess);
               });
        }

        static ConsoleOption Search(IPopulatorConsole repo)
        {
            return new ConsoleOption("Search entity"
               , (m, o) =>
               {
                   var search = new SearchRequest();

                   var result = repo.Persistence.Search(search,
                       new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                       }).Result;

                   PersistenceLog(m, "Search", result.IsSuccess);
               });
        }

        static ConsoleOption StressTest(IPopulatorConsole repo)
        {
            return new ConsoleOption("Create 100000 entities async"
               , (m, o) =>
               {
                   var batchId = Guid.NewGuid().ToString("N").ToUpperInvariant();
                   int i = 0;
                   for (; i < 100000; i++)
                   {
                       try
                       {
                           var result = repo.Persistence.Create(CreateEntity()
                               , new RepositorySettings()
                               {
                                   WaitTime = TimeSpan.FromMinutes(15),
                                   ProcessAsync = true,
                                   BatchId = batchId,
                                   CorrelationId = string.Format("{0}/{1}", batchId, i)
                               }
                               ).Result;
                       }
                       catch (Exception ex)
                       {
                           throw ex;
                       }
                   }

                   PersistenceLog(m, "100000 enqueued", true);
               });
        }

        static ConsoleOption StressCrudTest(IPopulatorConsole repo)
        {
            return new ConsoleOption("Create, Read, Update, Delete 1000 entities async"
               , (m, o) =>
               {
                   //// Create a work queue to process
                   //var batchId = Guid.NewGuid().ToString("N").ToUpperInvariant();
                   //var workQueue = new ConcurrentQueue<Tuple<int, Func<Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>, string, Task<bool>>>>();
                   //for (var j = 0; j < 1000; j++)
                   //{
                   //    workQueue.Enqueue(new Tuple<int, Func<Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>, string, Task<bool>>>(j, (r, b) => PerformCrud(r, b, m)));                       
                   //}

                   //for (int i = 0; i < 50; i++)
                   //{
                   //    var taskId = i;
                   //    Task.Run(async () =>
                   //    {
                   //        Tuple<int, IRepositoryAsync<Guid, MondayMorningBlues>, string, Task<bool>>> queueItem;
                   //        while (workQueue.TryDequeue(out queueItem))
                   //        {
                   //            var crudStart = DateTime.UtcNow;
                   //            var result = await queueItem.Item2(repo.Persistence, batchId);
                   //            if (!result)
                   //            {
                   //                PersistenceLog(m, "Failure for " + taskId, false);
                   //                return;
                   //            }
                   //            Console.WriteLine($"Crud for worker {taskId} processed {queueItem.Item1} successfully after {DateTime.UtcNow.Subtract(crudStart).TotalSeconds} seconds");
                   //        }
                   //        PersistenceLog(m, $"{taskId} Finished Crud {queueItem.Item1}", true);
                   //    });
                   //}

                   PersistenceLog(m, "1000 enqueued", true);
               });
        }

        private static async Task<bool> PerformCrud(ContextPersistence<Guid, MondayMorningBlues> repo, string batchId, ConsoleMenu m)
        {
            var createEntity = CreateEntity();
            var repoSettings = new RepositorySettings { BatchId = batchId };
            var createResult = await repo.Persistence.Create(createEntity, repoSettings);
            if (!createResult.IsSuccess)
            {
                PersistenceLog(m, $"StressCrudTest Create {createResult.ResponseCode}", createResult.IsSuccess);
                return false;
            }
            var readResult = await repo.Persistence.Read(createEntity.Id, repoSettings);
            if (!readResult.IsSuccess)
            {
                PersistenceLog(m, $"StressCrudTest Read {readResult.ResponseCode}", readResult.IsSuccess);
                return false;
            }
            var updateResult = await repo.Persistence.Update(readResult.Entity, repoSettings);
            if (!updateResult.IsSuccess)
            {
                PersistenceLog(m, $"StressCrudTest Update {updateResult.ResponseCode}", updateResult.IsSuccess);
                return false;
            }
            var deleteResult = await repo.Persistence.Delete(createEntity.Id, repoSettings);
            if (!deleteResult.IsSuccess)
            {
                PersistenceLog(m, $"StressCrudTest Delete {deleteResult.ResponseCode}", deleteResult.IsSuccess);
                return false;
            }

            return true;
        }
    }
}
