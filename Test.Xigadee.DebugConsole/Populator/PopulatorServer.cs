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
        readonly VersionPolicy<MondayMorningBlues> mVersionBlues =
            new VersionPolicy<MondayMorningBlues>(e => e.VersionId.ToString("N").ToLowerInvariant(), e => e.VersionId = Guid.NewGuid());

        readonly VersionPolicy<Blah2> mVersionBlah2 =
            new VersionPolicy<Blah2>((e) => e.VersionId.ToString("N").ToLowerInvariant(), (e) => e.VersionId = Guid.NewGuid());

        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            Persistence = (IRepositoryAsync<Guid, MondayMorningBlues>)Service.RegisterCommand
                (
                    new PersistenceSharedService<Guid, MondayMorningBlues>(Channels.Internal)
                    { ChannelId = Channels.TestB }
                );


            Service.RegisterCommand(new DoNothingJob { ChannelId = Channels.TestB });
        }

        protected override void RegisterCommunication()
        {
            base.RegisterCommunication();

            Service.RegisterListener(new AzureSBQueueListener(
                  Channels.TestB
                , Config.ServiceBusConnection
                , Channels.TestB
                , ListenerPartitionConfig.Init(0, 1)
                , resourceProfiles: new[] { mResourceDocDb, mResourceBlob }));

            Service.RegisterSender(new AzureSBQueueSender(
                  Channels.TestA
                , Config.ServiceBusConnection
                , Channels.TestA
                , SenderPartitionConfig.Init(0, 1)));

        }
    }
}
