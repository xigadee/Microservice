using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    internal abstract class PopulatorConsoleBase<M>:PopulatorBase<M, ConfigConsole>
        where M : MicroserviceBase, new()
    {

        protected ResourceProfile mResourceDocDb = new ResourceProfile("DocDB");
        protected ResourceProfile mResourceBlob = new ResourceProfile("Blob");

        protected override void RegisterCommunication()
        {
            base.RegisterCommunication();

            Service.RegisterListener(new AzureSBTopicListener("interserv", Config.ServiceBusConnection
                , "interserv",
                deleteOnStop: false, priorityPartitions: ListenerPartitionConfig.Default, mappingChannelId: "mychannel"));

            Service.RegisterSender(new AzureSBTopicSender("interserv", Config.ServiceBusConnection
                , "interserv", priorityPartitions: SenderPartitionConfig.Default));


            Service.RegisterListener(new AzureSBTopicListener("masterjob", Config.ServiceBusConnection
                , "masterjob", ListenerPartitionConfig.Init(2)));
            Service.RegisterSender(new AzureSBTopicSender("masterjob", Config.ServiceBusConnection
                , "masterjob", SenderPartitionConfig.Init(2)));

        }
    }
}
