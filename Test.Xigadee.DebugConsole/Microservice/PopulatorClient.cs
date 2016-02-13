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
    internal class MicroserviceClient: MicroserviceBase
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

        protected override void RegisterCommunication()
        {
            base.RegisterCommunication();

            Service.RegisterListener(new AzureSBQueueListener("testa", Config.ServiceBusConnection
                , "testa", ListenerPartitionConfig.Init(0, 1)
                , resourceProfiles: new[] { mResourceDocDb, mResourceBlob }));

            Service.RegisterSender(new AzureSBQueueSender("testb", Config.ServiceBusConnection
                , "testb", SenderPartitionConfig.Init(0, 1)));

        }
    }
}
