using System;
using System.Collections.Generic;
using System.Net;
using Xigadee;

namespace PiO
{
    class Program
    {
        static IPipeline mservice;

        static void Main(string[] args)
        {
            DebugMemoryDataCollector coll;
            PersistenceManagerHandlerMemory<int, LightwaveMessage> pm = null;
            //PersistenceManagerHandlerFileSystem<int, LightwaveMessage> pm = null;
            PersistenceClient<int, LightwaveMessage> pc = null;
            mservice = new MicroservicePipeline("PiO", description: "PiO Server");

            mservice
                .AdjustPolicyTaskManagerForDebug()
                .ConfigurationSetFromConsoleArgs(args)
                .AddDebugMemoryDataCollector(out coll)
                .AddChannelIncoming("lightwave", "LightwaveRF UDP traffic", ListenerPartitionConfig.Init(1))
                    .AttachUdpListener(
                          requestAddress: ("message", "in")
                        , deserialize: (holder) => holder.SetObject(new LightwaveMessage(holder.Blob, (IPEndPoint)holder.Metadata),true)
                        , udpExtended: new[] { (1, UdpConfig.UnicastAllIps(9761)), (0, UdpConfig.UnicastAllIps(9762)) }
                        )
                        
                    .AttachCommand(async (ctx) => 
                    {
                        //Do nothing
                        var debug = pm.ChannelId;
                        var rs = await pc.Create(ctx.Request.Message.Holder.Object as LightwaveMessage);
                    }, ("message", "in"))
                    .AttachPersistenceManagerHandlerMemory((LightwaveMessage m) => m.Trans, (s) => int.Parse(s), out pm)
                    //.AttachPersistenceManagerFileSystem((LightwaveMessage m) => m.Trans, (s) => int.Parse(s), out pm)
                    .AttachPersistenceClient(out pc)
                    .Revert()
                .AddChannelOutgoing("status", "Outgoing UDP status", SenderPartitionConfig.Init(1))
                    .AttachUdpSender(UdpConfig.BroadcastAllIps(44723), serializer: new StatisticsSummaryLogUdpSerializer())
                    .Revert()
                .OnDataCollection
                (
                    (ctx,ev) => 
                    {
                        ctx.Outgoing.Process(("status", null, null), ev.Data, 1, ProcessOptions.RouteExternal);
                    }
                    , DataCollectionSupport.Statistics
                )
                ;

            mservice.StartWithConsole(args: args);
        }
    }
}
