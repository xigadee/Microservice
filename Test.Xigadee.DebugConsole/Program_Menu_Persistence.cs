using Xigadee;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    static partial class Program
    {
        static ConsoleOption Create(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Create entity"
            , (m, o) =>
            {
                sServerContext.EntityId = Guid.NewGuid();

                var result = repo.Value.Create(CreateEntity(sServerContext.EntityId, email: sServerContext.EntityReference)
                    , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5), Source = "Xigadee"}).Result;

                if (result.IsSuccess)
                    sServerContext.EntityVersionid = result.Entity.VersionId;

                PersistenceLog(m, "Create", result.IsSuccess);
            }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption Read(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Read entity"
               , (m, o) =>
               {
                   var result = repo.Value.Read(sServerContext.EntityId
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog(m, "Read", result.IsSuccess);
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption ReadByReference(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Read entity by reference"
               , (m, o) =>
               {
                   var result = repo.Value.ReadByRef("email", sServerContext.EntityReference
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog(m, "Read By Reference", result.IsSuccess);
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption Update(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Update entity"
               , (m, o) =>
               {
                   var result = repo.Value.Update(
                       new MondayMorningBlues()
                       {
                           Id = sServerContext.EntityId,
                           ContentId = new Guid(),
                           VersionId = sServerContext.EntityVersionid,
                           Message = $"Hello mom2 -{DateTime.Now.ToString()}",
                           NotEnoughCoffee = false,
                           NotEnoughSleep = false,
                           Email = sServerContext.EntityReference
                       }
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) })
                       .Result;

                   PersistenceLog(m, "Update", result.IsSuccess);

                   if (result.IsSuccess)
                   {
                       sServerContext.EntityVersionid = result.Entity.VersionId;
                   }
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption Delete(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Delete entity"
               , (m, o) =>
               {
                   var result = repo.Value.Delete(sServerContext.EntityId
                       , new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sServerContext.EntityVersionid.ToString()
                       }).Result;
                   PersistenceLog(m, "Delete", result.IsSuccess);
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption DeleteByReference(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Delete entity by reference"
               , (m, o) =>
               {
                   var result = repo.Value.DeleteByRef("email", sServerContext.EntityReference,
                       new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sServerContext.EntityVersionid.ToString()
                       }).Result;
                   PersistenceLog(m, "Delete By Reference", result.IsSuccess);
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption Version(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Version entity"
               , (m, o) =>
               {
                   var result = repo.Value.Version(sServerContext.EntityId,
                       new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sServerContext.EntityVersionid.ToString()
                       }).Result;
                   PersistenceLog(m, "Version", result.IsSuccess);
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption VersionByReference(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Version entity by reference"
               , (m, o) =>
               {
                   var result = repo.Value.VersionByRef("EMAIL", sServerContext.EntityReference
                       , new RepositorySettings
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sServerContext.EntityVersionid.ToString()
                       }).Result;

                   PersistenceLog(m, "Version By Reference", result.IsSuccess);
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption Search(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Search entity"
               , (m, o) =>
               {
                   var search = new SearchRequest();

                   var result = repo.Value.Search(search,
                       new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                       }).Result;

                   PersistenceLog(m, "Search", result.IsSuccess);
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption StressTest(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
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
                           var result = repo.Value.Create(CreateEntity()
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
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        static ConsoleOption StressCrudTest(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
        {
            return new ConsoleOption("Create, Read, Update, Delete 1000 entities async"
               , (m, o) =>
               {
                   // Create a work queue to process
                   var batchId = Guid.NewGuid().ToString("N").ToUpperInvariant();
                   var workQueue = new ConcurrentQueue<Tuple<int, Func<Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>, string, Task<bool>>>>();
                   for (var j = 0; j < 1000; j++)
                   {
                       workQueue.Enqueue(new Tuple<int, Func<Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>, string, Task<bool>>>(j, (r, b) => PerformCrud(r, b, m)));                       
                   }

                   for (int i = 0; i < 50; i++)
                   {
                       var taskId = i;
                       Task.Run(async () =>
                       {
                           Tuple<int, Func<Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>, string, Task<bool>>> queueItem;
                           while (workQueue.TryDequeue(out queueItem))
                           {
                               var crudStart = DateTime.UtcNow;
                               var result = await queueItem.Item2(repo, batchId);
                               if (!result)
                               {
                                   PersistenceLog(m, "Failure for " + taskId, false);
                                   return;
                               }
                               Console.WriteLine($"Crud for worker {taskId} processed {queueItem.Item1} successfully after {DateTime.UtcNow.Subtract(crudStart).TotalSeconds} seconds");
                           }
                           PersistenceLog(m, $"{taskId} Finished Crud {queueItem.Item1}", true);
                       });
                   }

                   PersistenceLog(m, "1000 enqueued", true);
               }
               , enabled: (m, o) => sServerContext.PersistenceStatus() == 2
               );
        }

        private static async Task<bool> PerformCrud(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo, string batchId, ConsoleMenu m)
        {
            var createEntity = CreateEntity();
            var repoSettings = new RepositorySettings { BatchId = batchId };
            var createResult = await repo.Value.Create(createEntity, repoSettings);
            if (!createResult.IsSuccess)
            {
                PersistenceLog(m, $"StressCrudTest Create {createResult.ResponseCode}", createResult.IsSuccess);
                return false;
            }
            var readResult = await repo.Value.Read(createEntity.Id, repoSettings);
            if (!readResult.IsSuccess)
            {
                PersistenceLog(m, $"StressCrudTest Read {readResult.ResponseCode}", readResult.IsSuccess);
                return false;
            }
            var updateResult = await repo.Value.Update(readResult.Entity, repoSettings);
            if (!updateResult.IsSuccess)
            {
                PersistenceLog(m, $"StressCrudTest Update {updateResult.ResponseCode}", updateResult.IsSuccess);
                return false;
            }
            var deleteResult = await repo.Value.Delete(createEntity.Id, repoSettings);
            if (!deleteResult.IsSuccess)
            {
                PersistenceLog(m, $"StressCrudTest Delete {deleteResult.ResponseCode}", deleteResult.IsSuccess);
                return false;
            }

            return true;
        }
    }
}
