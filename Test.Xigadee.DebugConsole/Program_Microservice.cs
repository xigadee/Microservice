using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static void InitialiseMicroserviceClient(bool? internalComms = null)
        {
            sClient = new PopulatorClient();
            sService = sClient;

            sClient.Service.StatusChanged += ClientStatusChanged;

            sClient.Start();
        }

        static void InitialiseMicroserviceServer()
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
