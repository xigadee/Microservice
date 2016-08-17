using System;
using Xigadee;
namespace Test.Xigadee
{
    class ContextPersistence<K,E>
        where K: IEquatable<K>
    {
        public ContextPersistence(IRepositoryAsync<Guid, MondayMorningBlues> persistence)
        {
            Persistence = persistence;
        }

        public IRepositoryAsync<Guid, MondayMorningBlues> Persistence { get;}

        public int Status { get; set; }

        public bool CacheEnabled { get; set; }
    }

    /// <summary>
    /// This class is used to manage the state of the console application.
    /// </summary>
    class Context
    {
        public Context()
        {
            Client = new PopulatorClient();
            Server = new PopulatorServer();

            ClientPersistence = new Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>(() => Client.Persistence);

            ServerPersistence = new Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>(() => Server.Persistence);
        }

        public PopulatorClient Client { get; private set; }

        public PopulatorServer Server { get; private set; }

        public IDisposable ApiServer { get; set; }

        public Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> ClientPersistence;

        public Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> ServerPersistence;

        public Lazy<IRepositoryAsync<Guid, MondayMorningBlues>> ApiPersistence;

        public Func<int> PersistenceStatus = () => 0;

        public Func<int> ApiPersistenceStatus = () => 0;

        public int SlotCount;

        public PersistenceOptions PersistenceType = PersistenceOptions.DocumentDb;

        public Guid EntityVersionid;

        public Guid EntityId;

        public string EntityReference
        {
            get
            {
                return $"anyone+{EntityId.ToString("N")}@hotmail.com";
            }
        }

        public bool ClientCacheEnabled { get; set; }

        public bool ServerCacheEnabled { get; set; }

    }
}
