using System;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static ConsoleSettings sSettings;

        static MicroservicePersistenceWrapper<Guid, MondayMorningBlues> sClient;
        static MicroservicePersistenceWrapper<Guid, MondayMorningBlues> sServer;
        static ApiPersistenceConnector<Guid, MondayMorningBlues> sApiServer;

        static void Main(string[] args)
        {
            //The context holds the active data for the console application.
            sSettings = new ConsoleSettings(args);

            sClient = new MicroservicePersistenceWrapper<Guid, MondayMorningBlues>("TestClient", sSettings, ClientConfig);
            sServer = new MicroservicePersistenceWrapper<Guid, MondayMorningBlues>("TestServer", sSettings, ServerConfig, ServerInit);
            sApiServer = new ApiPersistenceConnector<Guid, MondayMorningBlues>(ApiConfig);
            
            //Attach the client events.
            sClient.StatusChanged += StatusChanged;
            sServer.StatusChanged += StatusChanged;
            sApiServer.StatusChanged += StatusChanged;

            //Show the main console menu.
            sMenuMain.Value.Show(args, shortcut: sSettings.Shortcut);

            //Detach the client events to allow the application to close.
            sClient.StatusChanged -= StatusChanged;
            sServer.StatusChanged -= StatusChanged;
            sApiServer.StatusChanged -= StatusChanged;
        }

        static void StatusChanged(object sender, StatusChangedEventArgs e)
        {
            var serv = sender as IMicroservice;

            sMenuMain.Value.AddInfoMessage($"{serv.Id.Name}={e.StatusNew.ToString()}{e.Message}", true);
        }
    }
}
