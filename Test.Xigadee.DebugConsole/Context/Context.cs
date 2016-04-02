using System;
using Xigadee;
namespace Test.Xigadee
{
    /// <summary>
    /// This class is used to manage the state of the console application.
    /// </summary>
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
