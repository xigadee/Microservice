using System;
using System.Net;
using Xigadee;

namespace PiO
{
    class Program
    {
        static void Main(string[] args)
        {
            var mservice = new MicroservicePipeline("PiO", description: "PiO Server");

            mservice
                .AdjustPolicyTaskManagerForDebug()
                .ConfigurationSetFromConsoleArgs(args)
                .AddChannelIncoming("incoming", "Incoming UDP traffic", ListenerPartitionConfig.Init(1))
                    .AttachUdpListener(new IPEndPoint(IPAddress.Any, 9761))
                    .Revert()
                .AddChannelOutgoing("status", "Outgoing UDP status", SenderPartitionConfig.Init(1))
                    .AttachUdpSender(new IPEndPoint(IPAddress.Any, 44723))
                    .Revert();
                ;


            mservice.StartWithConsole();
        }
    }
}
