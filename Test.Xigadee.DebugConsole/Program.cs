using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    partial class Program
    {
        static PopulatorClient sClient;

        static PopulatorServer sServer;

        static IPopulatorConsole sService;

        static void InitialiseMicroserviceClient(int processes, bool? internalComms = null)
        {
            sClient = new PopulatorClient();
            sService = sClient;

            sClient.Service.StatusChanged += ClientStatusChanged;

            sClient.Start();
        }

        static void InitialiseMicroserviceServer(int processes)
        {
            sServer = new PopulatorServer();
            sService = sServer;

            sServer.Service.StartRequested += ServerStartRequested;
            sServer.Service.StopRequested += Server_StopRequested;
            sServer.Service.StatusChanged += ServerStatusChanged;

            sServer.Start();
        }


        private static void Server_StopRequested(object sender, StopEventArgs e)
        {

        }

        private static void ServerStartRequested(object sender, StartEventArgs e)
        {
            e.ConfigurationOptions.ConcurrentRequestsMax = 4;
            e.ConfigurationOptions.ConcurrentRequestsMin = 1;
            e.ConfigurationOptions.StatusLogFrequency = TimeSpan.FromSeconds(15);

            //e.ConfigurationOptions.OverloadProcessLimit = 2;
            //if (e.ConfigurationOptions.OverloadProcessLimit == 0)
            //    e.ConfigurationOptions.OverloadProcessLimit = 11;        
        }
    }
}
