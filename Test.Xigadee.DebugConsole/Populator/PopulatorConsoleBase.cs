using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    internal abstract class PopulatorConsoleBase<M>: PopulatorBase<M, ConfigConsole>, IPopulatorConsole where M : Microservice, new()
    {

        public override void Start()
        {
            try
            {
                Populate();

                base.Start();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        protected ResourceProfile mResourceDocDb = new ResourceProfile("DocDB");
        protected ResourceProfile mResourceBlob = new ResourceProfile("Blob");

        public IRepositoryAsync<Guid, MondayMorningBlues> Persistence { get; protected set; }

        protected override void RegisterCommunication()
        {
            Service.RegisterListener(new AzureSBTopicListener(
                  Channels.Interserve
                , Config.ServiceBusConnection
                , Channels.Interserve
                , deleteOnStop: false
                , priorityPartitions: ListenerPartitionConfig.Default
                , mappingChannelId: "mychannel"));

            Service.RegisterSender(new AzureSBTopicSender(
                  Channels.Interserve
                , Config.ServiceBusConnection
                , Channels.Interserve
                , priorityPartitions: SenderPartitionConfig.Default));

            Service.RegisterListener(new AzureSBTopicListener(
                  Channels.MasterJob
                , Config.ServiceBusConnection
                , Channels.MasterJob
                , ListenerPartitionConfig.Init(2)));

            Service.RegisterSender(new AzureSBTopicSender(
                  Channels.MasterJob
                , Config.ServiceBusConnection
                , Channels.MasterJob
                , SenderPartitionConfig.Init(2)));
        }

        protected override void RegisterCommands()
        {
            Service.RegisterCommand(new TestMasterJob(Channels.MasterJob));
            Service.RegisterCommand(new TestMasterJob2(Channels.MasterJob));

            Service.RegisterCommand(new DelayedProcessingJob());
        }

        protected override void RegisterEventSources()
        {
            Service.RegisterEventSource(new AzureStorageEventSource(
                  Config.Storage
                , Service.Name
                , resourceProfile: mResourceBlob));

        }

        protected override void RegisterLogging()
        {
            base.RegisterLogging();

            Service.RegisterLogger(new TraceEventLogger());
            Service.RegisterLogger(new AzureStorageLogger(
                  Config.Storage
                , Service.Name
                , resourceProfile: mResourceBlob));

        }
    }
}
