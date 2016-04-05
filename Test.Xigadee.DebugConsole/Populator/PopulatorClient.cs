using System;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This is the client populator. This is used to configure and start the Microservice.
    /// </summary>
    internal class PopulatorClient: PopulatorConsoleBase<MicroserviceClient>
    {
        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            var cacheManager = new RedisCacheManager<Guid, MondayMorningBlues>(Config.RedisCacheConnection, true);

            Persistence = (IRepositoryAsync<Guid, MondayMorningBlues>)Service.RegisterCommand(
                new PersistenceMessageInitiator<Guid, MondayMorningBlues>(cacheManager)
                {
                    ChannelId = Channels.TestB
                    , ResponseChannelId = Channels.Interserve
                });

            Service.RegisterCommand(
                new PersistenceMessageInitiator<Guid, Blah2>()
                {
                      ChannelId = Channels.TestB
                    , ResponseChannelId = Channels.Interserve
                });
        }

        protected override void RegisterCommunication()
        {
            base.RegisterCommunication();

            Service.RegisterListener(new AzureSBTopicListener(
                  Channels.Interserve
                , Config.ServiceBusConnection
                , Channels.Interserve
                , deleteOnStop: false
                , listenOnOriginatorId: true
                , priorityPartitions: ListenerPartitionConfig.Init(0, 1)));

            Service.RegisterListener(new AzureSBQueueListener(
                  Channels.TestA
                , Config.ServiceBusConnection
                , Channels.TestA
                , ListenerPartitionConfig.Init(0, 1)
                , resourceProfiles: new[] { mResourceDocDb, mResourceBlob }));

            Service.RegisterSender(new AzureSBQueueSender(
                  Channels.TestB
                , Config.ServiceBusConnection
                , Channels.TestB
                , SenderPartitionConfig.Init(0, 1)));

        }
    }
}
