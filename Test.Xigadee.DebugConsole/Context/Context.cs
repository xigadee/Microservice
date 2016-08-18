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
            ApiServer = new PopulatorApiService();
        }
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
        public PopulatorApiService ApiServer { get; private set; }

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
