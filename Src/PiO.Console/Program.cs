using System;
using Xigadee;

namespace PiO
{
    class Program
    {
        static void Main(string[] args)
        {
            var ms = new MicroservicePipeline("IOListen", description: "Microservice IO Listener");

            ms
                .AdjustPolicyTaskManagerForDebug()
                .ConfigurationSetFromConsoleArgs(args)
                .AddChannelIncoming("incoming", "Incoming UDP traffic", ListenerPartitionConfig.Init(1))
                    .AttachUdpListener()
                ;
                

            ms.StartWithConsole();
        }
    }
}
