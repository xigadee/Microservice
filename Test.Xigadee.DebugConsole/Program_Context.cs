using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        class Context
        {
            public PopulatorClient Client;

            public PopulatorServer Server;

            public IRepositoryAsync<Guid, MondayMorningBlues> Persistence;

            public Func<int> PersistenceStatus = () => 0;

            public int SlotCount;

            public PersistenceOptions PersistenceType = PersistenceOptions.DocumentDb;

            public Guid Versionid;

            public Guid Testid;
        }
    }
}
