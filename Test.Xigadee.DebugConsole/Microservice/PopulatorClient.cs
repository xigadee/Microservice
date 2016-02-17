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
    /// This is the client Microservice.
    /// </summary>
    internal class MicroserviceClient: Microservice
    {
        public MicroserviceClient()
        {
            ServicePointManager.DefaultConnectionLimit = 50000;
        }
    }

    /// <summary>
    /// This is the client populator. This is used to configure and start the Microservice.
    /// </summary>
    internal class PopulatorClient: PopulatorConsoleBase<MicroserviceClient>
    {

        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            Persistence = (IRepositoryAsync<Guid, MondayMorningBlues>)Service.RegisterCommand(
                new PersistenceMessageInitiator<Guid, MondayMorningBlues>()
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

            Service.RegisterListener(new AzureSBQueueListener(Channels.TestA
                , Config.ServiceBusConnection
                , Channels.TestA
                , ListenerPartitionConfig.Init(0, 1)
                , resourceProfiles: new[] { mResourceDocDb, mResourceBlob }));

            Service.RegisterSender(new AzureSBQueueSender(Channels.TestB
                , Config.ServiceBusConnection
                , Channels.TestB
                , SenderPartitionConfig.Init(0, 1)));

        }


    }
}
