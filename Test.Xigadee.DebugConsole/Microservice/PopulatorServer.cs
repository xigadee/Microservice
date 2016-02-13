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
    /// This is the server Microservice.
    /// </summary>
    public class MicroserviceServer: MicroserviceBase
    {
        public MicroserviceServer()
        {
            ServicePointManager.DefaultConnectionLimit = 50000;
        }
    }

    /// <summary>
    /// This populator is used to configure the server Microservice.
    /// </summary>
    internal class PopulatorServer: PopulatorConsoleBase<MicroserviceServer>
    {
        static readonly VersionPolicy<MondayMorningBlues> mVersionBlues =
            new VersionPolicy<MondayMorningBlues>(e => e.VersionId.ToString("N").ToLowerInvariant(), e => e.VersionId = Guid.NewGuid());

        static VersionPolicy<Blah2> mVersionBlah2 =
            new VersionPolicy<Blah2>((e) => e.VersionId.ToString("N").ToLowerInvariant(), (e) => e.VersionId = Guid.NewGuid());

        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            var initiator = new PersistenceSharedService<Guid, MondayMorningBlues>(Channels.Internal) { ChannelId = Channels.TestB };

            Service.RegisterCommand(initiator);

        }

        protected override void RegisterCommunication()
        {
            base.RegisterCommunication();

            Service.RegisterListener(new AzureSBQueueListener("testb"
                , Config.ServiceBusConnection, "testb", ListenerPartitionConfig.Init(0, 1)
                , resourceProfiles: new[] { mResourceDocDb, mResourceBlob }));

            Service.RegisterSender(new AzureSBQueueSender("testa"
                , Config.ServiceBusConnection, "testa", SenderPartitionConfig.Init(0, 1)));

        }
    }
}
