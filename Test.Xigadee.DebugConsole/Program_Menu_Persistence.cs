using Xigadee;
using System;
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
                    , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }).Result;

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

        static ConsoleOption SequenceTest(Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> repo)
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
    }
}
