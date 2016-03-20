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
        public int Status { get; set; } = 0;

        public event EventHandler<CommandRegisterEventArgs> OnRegister;

        public readonly ResourceProfile mResourceDocDb = new ResourceProfile("DocDB");

        public readonly ResourceProfile mResourceBlob = new ResourceProfile("Blob");

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
            if (OnRegister != null)
                OnRegister(this, new CommandRegisterEventArgs(Service, Config));

            //Service.RegisterCommand(new TestMasterJob(Channels.MasterJob));
            //Service.RegisterCommand(new TestMasterJob2(Channels.MasterJob));

            //Service.RegisterCommand(new DelayedProcessingJob());
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

    public class CommandRegisterEventArgs: EventArgs
    {
        public CommandRegisterEventArgs(Microservice service, ConfigConsole config)
        {
            Service = service;
            Config = config;
        }

        public Microservice Service { get; set; }

        public ConfigConsole Config { get; set; }
    }
}
