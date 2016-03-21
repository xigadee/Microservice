using System;
using Microsoft.WindowsAzure.Storage.Auth;
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
                Server = new PopulatorServer();
            }

            public PopulatorClient Client;

            public PopulatorServer Server;

            public IRepositoryAsync<Guid, MondayMorningBlues> Persistence;

            public Func<int> PersistenceStatus = () => 0;

            public int SlotCount;

            public PersistenceOptions PersistenceType = PersistenceOptions.DocumentDb;

            public Guid Versionid;

            public Guid Testid;

            public bool ClientCacheEnabled { get; set; }

            public bool ServerCacheEnabled { get; set; }
        }
    }
}
