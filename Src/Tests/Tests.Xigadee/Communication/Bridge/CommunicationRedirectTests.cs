using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class CommunicationRedirectTests
    {
        [TestMethod]
        public void Redirect1()
        {
            try
            {
                var fabric = new ManualFabric();
                var bridgeOut = fabric.Queue;
                var bridgein = fabric.Broadcast;

                ICommandInitiator init;
                DebugMemoryDataCollector memp1, memp2;

                var client = new MicroservicePipeline("Sender")
                    .AdjustPolicyCommunicationBoundaryLoggingActive()
                    .AddDebugMemoryDataCollector(out memp1)
                    .AddChannelIncoming("cresponse", autosetPartition01:false)
                        .AttachPriorityPartition((0,0.9M),(1,1.1M))
                        .AttachListener(bridgein.GetListener())
                        .AttachICommandInitiator(out init)
                        .Revert()
                    .AddChannelOutgoing("crequest")
                        .AttachSender(bridgeOut.GetSender())
                        ;

                var server = new MicroservicePipeline("Receiver")
                    .AdjustPolicyCommunicationBoundaryLoggingActive()
                    .AddDebugMemoryDataCollector(out memp2)
                    .AddChannelIncoming("credirect")
                        .AttachCommand((CommandMethodRequestContext ctx) =>
                        {
                            ctx.ResponseSet(201, "Hi");
                            return Task.FromResult(0);
                        }, ("one", "two"))
                        .Revert()
                    .AddChannelIncoming("crequest")
                        .AttachMessageRedirectRule(
                              canRedirect: (p) => p.Message.MessageType.Equals("bridgeme", StringComparison.InvariantCultureIgnoreCase)
                            , redirect: (p) =>
                            {
                                p.Message.MessageType = "BridgeMe2";
                                p.Message.ActionType = "Whatever";
                            }
                            )
                        .AttachMessageRedirectRule(
                              canRedirect: (p) => p.Message.MessageType.Equals("redirectme", StringComparison.InvariantCultureIgnoreCase)
                            , redirect: (p) =>
                            {
                                p.Message.ChannelId = "credirect";
                                p.Message.MessageType = "one";
                                p.Message.ActionType = "two";
                            }
                            )
                        .AttachListener(bridgeOut.GetListener())
                        .AttachCommand((CommandMethodRequestContext ctx) =>
                        {
                            ctx.ResponseSet(400,"Blah");
                            return Task.FromResult(0);
                        }, ("BridgeMe", "create"))
                        .AttachCommand((CommandMethodRequestContext ctx) =>
                        {
                            ctx.ResponseSet(200, "Yah!");
                            return Task.FromResult(0);
                        }, ("bridgeMe2", "whatever"))
                        .Revert()
                    .AddChannelOutgoing("cresponse")
                        .AttachSender(bridgein.GetSender())
                        ;

                client.Start();
                server.Start();

                int check1 = client.ToMicroservice().Commands.Count();
                int check2 = server.ToMicroservice().Commands.Count();

                var entity = new BridgeMe() { Message = "Momma" };

                var rs = init.Process<BridgeMe, string>(("crequest", "BRIDGEME", "create"), entity).Result;
                Assert.IsTrue(rs.ResponseCode == 200);

                var rs2 = init.Process<BridgeMe, string>(("crequest", "redirectme", "hmm"), entity).Result;
                Assert.IsTrue(rs2.ResponseCode == 201);

                client.Stop();
                server.Stop();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class BridgeMe2
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Message { get; set; }
    }
}
