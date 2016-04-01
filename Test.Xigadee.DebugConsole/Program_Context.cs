using System;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        class Context
        {
            public Context()
            {
                Client = new PopulatorClient();
                ClientPersistence = new Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>(() => Client.Persistence);
                Server = new PopulatorServer();
                ServerPersistence = new Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>(() => Server.Persistence);
            }

            public PopulatorClient Client { get; private set; }

            public PopulatorServer Server { get; private set; }

            public Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> ClientPersistence;

            public Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> ServerPersistence;

            public Func<int> PersistenceStatus = () => 0;

            public int SlotCount;

            public PersistenceOptions PersistenceType = PersistenceOptions.DocumentDb;

            public void SetPersistenceOption(string value)
            {
                switch (value)
                {
                    case "sql":
                        PersistenceType = PersistenceOptions.Sql;
                        break;
                    case "blob":
                        PersistenceType = PersistenceOptions.Blob;
                        break;
                    case "docdbsdk":
                        PersistenceType = PersistenceOptions.DocumentDbSdk;
                        break;
                    case "docdb":
                        PersistenceType = PersistenceOptions.DocumentDb;
                        break;
                    case "redis":
                        PersistenceType = PersistenceOptions.RedisCache;
                        break;
                    default:
                        PersistenceType = PersistenceOptions.DocumentDb;
                        break;
                }
            }

            public Guid EntityVersionid;

            public Guid EntityId;

            public string EntityReference
            {
                get
                {
                    return $"guy+{EntityId.ToString("N")}@hotmail.com";
                }
            }

            public bool ClientCacheEnabled { get; set; }

            public bool ServerCacheEnabled { get; set; }
        }
    }
}
