using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This populator is used to configure the server Microservice.
    /// </summary>
    internal class PopulatorServer: PopulatorConsoleBase<MicroserviceServer>
    {

        //public readonly VersionPolicy<Blah2> VersionBlah2 =
        //    new VersionPolicy<Blah2>((e) => e.VersionId.ToString("N").ToLowerInvariant(), (e) => e.VersionId = Guid.NewGuid());

        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            Persistence = (IRepositoryAsync<Guid, MondayMorningBlues>)Service.RegisterCommand
                (
                    new PersistenceSharedService<Guid, MondayMorningBlues>(Channels.Internal)
                    { ChannelId = Channels.TestB }
                );

            Service.RegisterCommand(new DoNothingJob { ChannelId = Channels.TestC, ResponseChannelId = Channels.InternalCallback });

            Service.RegisterCommand(new DelayedProcessingJob { ChannelId = Channels.TestA, ResponseChannelId = Channels.InternalCallback });

            //Service.RegisterCommand(new DelayedProcessingJob { ChannelId = Channels.TestA });
            //Service.RegisterCommand(new DelayedProcessingJob { ChannelId = Channels.TestA });
        }

        protected override void RegisterCommunication()
        {
            base.RegisterCommunication();

            Service.RegisterSender(new AzureSBTopicSender(
                  Channels.Interserve
                , Config.ServiceBusConnection()
                , Channels.Interserve
                , priorityPartitions: SenderPartitionConfig.Init(0, 1)));

            Service.RegisterListener(new AzureSBQueueListener(
                  Channels.TestB
                , Config.ServiceBusConnection()
                , Channels.TestB
                , ListenerPartitionConfig.Init(0, 1)
                , resourceProfiles: new[] { mResourceDocDb, mResourceBlob }));

            Service.RegisterListener(new AzureSBQueueListener(
                  Channels.TestC
                , Config.ServiceBusConnection()
                , Channels.TestC
                , ListenerPartitionConfig.Init(0, 1)
                , resourceProfiles: new[] { mResourceDocDb, mResourceBlob }));

            Service.RegisterSender(new AzureSBQueueSender(
                  Channels.TestA
                , Config.ServiceBusConnection()
                , Channels.TestA
                , SenderPartitionConfig.Init(0, 1)));
        }

        protected override void RegisterLogging()
        {
            base.RegisterLogging();
        }
    }
}
