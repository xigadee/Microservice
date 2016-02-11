using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Xigadee;

namespace Test.Xigadee
{
    partial class Program
    {
        static MicroserviceBase client, server;

        static DocumentDbConnection docDbCreds = DocumentDbConnection.ToConnection("dataintperftest",
            "4UaoqIKW2UHKbtYaD680DrUr246nN0nbw9O+McmVh7o5PUsZ3zF+E5lp0A5B2HX7fqLBzYZhDWw6rKXivdtGlw==");
        const string docDBdatabase = "UnitTest";

        static StorageCredentials storageCreds = new StorageCredentials("dataintperftest",
            "wX5m6LzPuK85fOWMwIRNxW24aOfa3GnpaYjctRRaGH02G7dGuAxVuv2lwG+IeGPXPKLpLQoA6Nos5+6DRX966w==");

        static string serviceBusConnection =
            "Endpoint=sb://dataintperftest.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=B+QhaZdHZA0C+HWjqQMTwhiAp9m6zUlfegkmnXMFMpg=";

        static ResourceProfile mResourceDocDb = new ResourceProfile("DocDB");
        static ResourceProfile mResourceBlob = new ResourceProfile("Blob");

        static void InitialiseMicroserviceClient(int processes, bool? internalComms = null)
        {
            client = new MicroserviceClient();
            client.StatusChanged += ClientStatusChanged;
            client.RegisterPayloadSerializer(new JsonContractSerializer());
            client.ConfigurationOptions.StatusLogFrequency = TimeSpan.FromSeconds(15);
            //client.ConfigurationOptions.ConcurrentRequestsMax = processes;
            InitialiseEventSource(client);
            InitialiseLoggers(client);

            InitialiseInitiators(client);

            if (!(internalComms ?? false))
                CreateReceivers().ForEach((r) => client.RegisterCommand(r));

            if (internalComms ?? true)
                InitialiseAzureCommsClient();

            InitialiseJobs(client);

            client.Start();
        }

        static void InitialiseMicroserviceServer(int processes)
        {
            server = new MicroserviceServer();

            server.StartRequested += ServerStartRequested;
            server.StatusChanged += ServerStatusChanged;

            server.RegisterPayloadSerializer(new JsonContractSerializer());

            InitialisePersistenceSharedService(server);
            InitialiseEventSource(server);
            InitialiseLoggers(server);

            CreateReceivers().ForEach((r) => server.RegisterCommand(r));

            InitialiseAzureCommsServer();

            InitialiseJobs(server);
            server.RegisterCommand(new DoNothingJob { ChannelId = "testb" });
            //server.RegisterJob(new DelayedProcessingJob {ChannelId = "testc"});

            server.Start();
        }
        static void InitialiseMicroserviceServerStressTest(int processes)
        {
            server = new MicroserviceServer();

            server.StartRequested += ServerStartRequested;
            server.StopRequested += Server_StopRequested;
            //server.ConfigurationOptions.ConcurrentRequestsMax = processes;
            //server.ConfigurationOptions.OverloadProcessLimit = processes / 10;
            //if (server.ConfigurationOptions.OverloadProcessLimit == 0)
            //    server.ConfigurationOptions.OverloadProcessLimit = 2;

            server.StatusChanged += ServerStatusChanged;
            server.RegisterPayloadSerializer(new JsonContractSerializer());

            InitialisePersistenceSharedService(server);
            InitialiseEventSource(server);
            InitialiseLoggers(server);

            CreateReceivers().ForEach((r) => server.RegisterCommand(r));

            InitialiseAzureCommsServer();

            InitialiseJobs(server);
            server.RegisterCommand(new DoNothingJob { ChannelId = "testb" });
            //server.RegisterJob(new DelayedProcessingJob {ChannelId = "testc"});

            server.Start();
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
