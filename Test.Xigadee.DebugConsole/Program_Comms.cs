using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    partial class Program
    {

        static void InitialiseAzureCommsClient()
        {
            client.RegisterListener(new AzureSBQueueListener("testa", serviceBusConnection, "testa", ListenerPartitionConfig.Init( 0, 1 )
                , resourceProfiles: new[] { mResourceDocDb, mResourceBlob }));
            client.RegisterSender(new AzureSBQueueSender("testb", serviceBusConnection, "testb", SenderPartitionConfig.Init(0, 1)));

            InitialiseAzureCommsCommon(client);
        }

        static void InitialiseAzureCommsServer()
        {
            server.RegisterListener(new AzureSBQueueListener("testb", serviceBusConnection, "testb", ListenerPartitionConfig.Init(0, 1 )
                , resourceProfiles: new [] { mResourceDocDb, mResourceBlob }));
            server.RegisterSender(new AzureSBQueueSender("testa", serviceBusConnection, "testa", SenderPartitionConfig.Init(0, 1)));

            InitialiseAzureCommsCommon(server);
        }

        static void InitialiseAzureCommsCommon(MicroserviceBase service)
        {
            //service.RegisterListener(new AzureSBTopicListener("interserv", serviceBusConnection, "interserv",
            //    DefaultTimeout: service.ConfigurationOptions.ProcessingTimeMax, deleteOnStop: true, priorityPartitions: new int[] { 1 }));

            service.RegisterListener(new AzureSBTopicListener("interserv", serviceBusConnection, "interserv",
                deleteOnStop: false, priorityPartitions: ListenerPartitionConfig.Default, mappingChannelId: "mychannel"));

            service.RegisterSender(new AzureSBTopicSender("interserv", serviceBusConnection, "interserv", priorityPartitions: SenderPartitionConfig.Default));


            service.RegisterListener(new AzureSBTopicListener("masterjob", serviceBusConnection, "masterjob", ListenerPartitionConfig.Init(2)));
            service.RegisterSender(new AzureSBTopicSender("masterjob", serviceBusConnection, "masterjob", SenderPartitionConfig.Init(2)));
        }
    }
}
