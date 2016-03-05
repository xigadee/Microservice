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

            sServer.Service.StatusChanged += ServerStatusChanged;

            sServer.Service.StartRequested += ServerStartRequested;
            sServer.Service.StopRequested += ServerStopRequested;

            sServer.Start();
        }

    }
}
