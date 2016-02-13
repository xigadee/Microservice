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
