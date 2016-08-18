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
            Server = new PopulatorServer();

            ClientPersistence = new ContextPersistence<Guid, MondayMorningBlues>( new Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>(() => Client.Persistence));

            ServerPersistence = new ContextPersistence<Guid, MondayMorningBlues>(new Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>(() => Server.Persistence));

            ApiPersistence = new ContextPersistence<Guid, MondayMorningBlues>(new Lazy<IRepositoryAsync<Guid, MondayMorningBlues>>(() => new ApiProviderAsyncV2<Guid, MondayMorningBlues>(ApiUri)));
        }
        /// <summary>
        /// This is the Uri for the Api listener and Api Provider.
        /// </summary>
        public Uri ApiUri { get; set; }
        /// <summary>
        /// This is the client Microservice.
        /// </summary>
        public PopulatorClient Client { get; private set; }
        /// <summary>
        /// This is the server Microservice.
        /// </summary>
        public PopulatorServer Server { get; private set; }
        /// <summary>
        /// This is the Api instancer service.
        /// </summary>
        public IDisposable ApiServer { get; set; }

        /// <summary>
        /// This is the client persistence class.
        /// </summary>//ContextPersistence<K, E>
        public ContextPersistence<Guid, MondayMorningBlues> ClientPersistence { get; private set;}
        /// <summary>
        /// This is the server persistence class.
        /// </summary>
        public ContextPersistence<Guid, MondayMorningBlues> ServerPersistence { get; private set; }
        /// <summary>
        /// This is the Api Persistence client that will connect to the Api Service over a RESTful Api connection.
        /// </summary>
        public ContextPersistence<Guid, MondayMorningBlues> ApiPersistence { get; private set; }

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
