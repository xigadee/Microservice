using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    partial class Program
    {
        static PopulatorClient client;
        static PopulatorServer server;
        static IPopulatorConsole sService;

        static DocumentDbConnection docDbCreds = DocumentDbConnection.ToConnection("dataintperftest",
            "4UaoqIKW2UHKbtYaD680DrUr246nN0nbw9O+McmVh7o5PUsZ3zF+E5lp0A5B2HX7fqLBzYZhDWw6rKXivdtGlw==");
        const string docDBdatabase = "UnitTest";

        static StorageCredentials storageCreds = new StorageCredentials("dataintperftest",
            "wX5m6LzPuK85fOWMwIRNxW24aOfa3GnpaYjctRRaGH02G7dGuAxVuv2lwG+IeGPXPKLpLQoA6Nos5+6DRX966w==");

        static string serviceBusConnection =
            "Endpoint=sb://dataintperftest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=B+QhaZdHZA0C+HWjqQMTwhiAp9m6zUlfegkmnXMFMpg=";

        static void InitialiseMicroserviceClient(int processes, bool? internalComms = null)
        {
            client = new PopulatorClient();
            client.Service.StatusChanged += ClientStatusChanged;

            client.Service.Start();
        }

        static void InitialiseMicroserviceServer(int processes)
        {
            server = new PopulatorServer();

            server.Service.StartRequested += ServerStartRequested;
            server.Service.StatusChanged += ServerStatusChanged;

            server.Service.Start();
        }

        static void InitialiseMicroserviceServerStressTest(int processes)
        {
            server = new PopulatorServer();

            server.Service.StartRequested += ServerStartRequested;
            server.Service.StopRequested += Server_StopRequested;

            server.Service.StatusChanged += ServerStatusChanged;


            server.Service.Start();
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
