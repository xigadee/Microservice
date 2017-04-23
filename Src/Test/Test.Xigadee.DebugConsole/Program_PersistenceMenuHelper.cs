#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
        /// <summary>
        /// This static method creates a new entity for persistence testing.
        /// </summary>
        /// <param name="Id">The optional Guid</param>
        /// <param name="versionId">The optional version id.</param>
        /// <param name="email">The optional email.</param>
        /// <returns>Returns a new entity.</returns>
        static MondayMorningBlues CreateEntity(Guid? Id = null, Guid? versionId = null, string email = null)
        {
            Guid newId = Id ?? Guid.NewGuid();

            return new MondayMorningBlues
            {
                Id = newId,
                ContentId = newId,
                VersionId = versionId ?? Guid.NewGuid(),
                Message = DateTime.Now.ToString(),
                NotEnoughCoffee = true,
                NotEnoughSleep = true,
                Email = email
            };
        }

        static void PersistenceLog(ConsoleMenu menu, string action, bool success)
        {
            menu.AddInfoMessage($"{action} {(success ? "OK" : "Fail")}"
                , true, success ? EventLogEntryType.Information : EventLogEntryType.Error);
        }

        static ConsoleOption Create(IConsolePersistence<Guid,MondayMorningBlues> repo)
        {
            return new ConsoleOption("Create entity"
            , (m, o) =>
            {
                sContext.EntityState.Id = Guid.NewGuid();

                var result = repo.Persistence.Create(CreateEntity(sContext.EntityState.Id, email: sContext.EntityState.Reference)
                    , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5), Source = "Xigadee"}).Result;

                if (result.IsSuccess)
                    sContext.EntityState.Versionid = result.Entity.VersionId;

                PersistenceLog(m, "Create", result.IsSuccess);
            });
        }

        static ConsoleOption Read(IConsolePersistence<Guid, MondayMorningBlues> repo)
        {
            return new ConsoleOption("Read entity"
               , (m, o) =>
               {
                   var result = repo.Persistence.Read(sContext.EntityState.Id
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog(m, "Read", result.IsSuccess);
               });
        }

        static ConsoleOption ReadByReference(IConsolePersistence<Guid, MondayMorningBlues> repo)
        {
            return new ConsoleOption("Read entity by reference"
               , (m, o) =>
               {
                   var result = repo.Persistence.ReadByRef("email", sContext.EntityState.Reference
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;
                   PersistenceLog(m, "Read By Reference", result.IsSuccess);
               });
        }

        static ConsoleOption Update(IConsolePersistence<Guid, MondayMorningBlues> repo)
        {
            return new ConsoleOption("Update entity"
               , (m, o) =>
               {
                   var result = repo.Persistence.Update(
                       new MondayMorningBlues()
                       {
                           Id = sContext.EntityState.Id,
                           ContentId = new Guid(),
                           VersionId = sContext.EntityState.Versionid,
                           Message = $"Hello mom2 -{DateTime.Now.ToString()}",
                           NotEnoughCoffee = false,
                           NotEnoughSleep = false,
                           Email = sContext.EntityState.Reference
                       }
                       , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) })
                       .Result;

                   PersistenceLog(m, "Update", result.IsSuccess);

                   if (result.IsSuccess)
                   {
                       sContext.EntityState.Versionid = result.Entity.VersionId;
                   }
               });
        }

        static ConsoleOption Delete(IConsolePersistence<Guid, MondayMorningBlues> repo)
        {
            return new ConsoleOption("Delete entity"
               , (m, o) =>
               {
                   var result = repo.Persistence.Delete(sContext.EntityState.Id
                       , new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sContext.EntityState.Versionid.ToString()
                       }).Result;
                   PersistenceLog(m, "Delete", result.IsSuccess);
               });
        }

        static ConsoleOption DeleteByReference(IConsolePersistence<Guid, MondayMorningBlues> repo)
        {
            return new ConsoleOption("Delete entity by reference"
               , (m, o) =>
               {
                   var result = repo.Persistence.DeleteByRef("email", sContext.EntityState.Reference,
                       new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sContext.EntityState.Versionid.ToString()
                       }).Result;
                   PersistenceLog(m, "Delete By Reference", result.IsSuccess);
               });
        }

        static ConsoleOption Version(IConsolePersistence<Guid, MondayMorningBlues> repo)
        {
            return new ConsoleOption("Version entity"
               , (m, o) =>
               {
                   var result = repo.Persistence.Version(sContext.EntityState.Id,
                       new RepositorySettings()
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sContext.EntityState.Versionid.ToString()
                       }).Result;
                   PersistenceLog(m, "Version", result.IsSuccess);
               });
        }

        static ConsoleOption VersionByReference(IConsolePersistence<Guid, MondayMorningBlues> repo)
        {
            return new ConsoleOption("Version entity by reference"
               , (m, o) =>
               {
                   var result = repo.Persistence.VersionByRef("EMAIL", sContext.EntityState.Reference
                       , new RepositorySettings
                       {
                           WaitTime = TimeSpan.FromMinutes(5)
                            , VersionId = sContext.EntityState.Versionid.ToString()
                       }).Result;

                   PersistenceLog(m, "Version By Reference", result.IsSuccess);
               });
        }

        static ConsoleOption Search(IConsolePersistence<Guid, MondayMorningBlues> repo)
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

        static ConsoleOption StressTest(IConsolePersistence<Guid, MondayMorningBlues> repo)
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

        static ConsoleOption StressCrudTest(IConsolePersistence<Guid, MondayMorningBlues>  repo)
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
