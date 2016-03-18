using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    static partial class Program
    {
        static void InitialiseMicroserviceClient()
        {
            sContext.Client = new PopulatorClient();
            sContext.Persistence = sContext.Client.Persistence;

            sContext.Client.Service.StatusChanged += ClientStatusChanged;

            sContext.Client.Start();
        }

        static void InitialiseMicroserviceServer()
        {
            sContext.Server = new PopulatorServer();
            sContext.Persistence = sContext.Server.Persistence;

            sContext.Server.Service.StatusChanged += ServerStatusChanged;

            sContext.Server.Service.StartRequested += ServerStartRequested;
            sContext.Server.Service.StopRequested += ServerStopRequested;

            sContext.Server.Start();
        }
    }
}
