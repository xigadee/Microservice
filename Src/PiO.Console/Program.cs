using System;
using System.Net;
using System.Threading.Tasks;
using Xigadee;

namespace PiO
{



    class Program
    {
        static MicroservicePipeline mservice;

        static void Main(string[] args)
        {
            DebugMemoryDataCollector coll;
            mservice = new MicroservicePipeline("PiO", description: "PiO Server");

            mservice
                .AdjustPolicyTaskManagerForDebug()
                .ConfigurationSetFromConsoleArgs(args)
                .AddDebugMemoryDataCollector(out coll)
                .AddChannelIncoming("lightwave", "LightwaveRF UDP traffic", ListenerPartitionConfig.Init(1))
                    .AttachUdpListener(new IPEndPoint(IPAddress.Any, 9761)
                        , addressSend: ("message","in")
                        , deserialize: (holder) =>
                        {
                            holder.Object = new LightwaveMessage(holder);
                            holder.ObjectType = typeof(LightwaveMessage);
                        })
                    .AttachCommand(async (ctx) => 
                    {
                        //Do nothing
                        await Task.Delay(10);
                    }
                    , ("message", "in"))
                    .Revert()
                .AddChannelOutgoing("status", "Outgoing UDP status", SenderPartitionConfig.Init(1))
                    .AttachUdpSender(new IPEndPoint(IPAddress.Any, 44723)
                        , StatisticsSummaryLog.MimeContentType, new StatisticsSummaryLogUdpSerializer())
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

            mservice.StartWithConsole();
        }
    }
}
