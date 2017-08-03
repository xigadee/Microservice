using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Threading.Tasks;
using System.Threading;

namespace Test.Xigadee
{
    [TestClass]
    public class CommunicationBridgeReroute
    {
        [Contract("fredo", "Do", "Something1")]
        public interface IContractInitial : IMessageContract { }

        [Contract("crequest", "Did", "SomethingElse")]
        public interface IContractFinal : IMessageContract { }

        /// <summary>
        /// This test is used to check that a message can be rerouted to another channel using a message Clone and SetDestination command.
        /// </summary>
        [TestMethod]
        public void TestReroute()
        {
            var bridgeOut = new ManualCommunicationBridgeAgent(CommunicationBridgeMode.RoundRobin);
            bool success = false;
            ManualResetEvent mre = new ManualResetEvent(false);
            DebugMemoryDataCollector memp1,memp2;

            var p1 = new MicroservicePipeline("Sender")
                .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                .AddDebugMemoryDataCollector(out memp1)
                .AddChannelIncoming("fredo")
                    .AttachCommand(typeof(IContractInitial), (ctx) =>
                    {
                        ctx.Responses.Add(new TransmissionPayload(ctx.Request.Message.Clone().SetDestination<IContractFinal>()));
                        return Task.FromResult(0);
                    })
                    .Revert()
                .AddChannelOutgoing("crequest")
                    .AttachSender(bridgeOut.GetSender())
                    .Revert()
                    ;

            var p2 = new MicroservicePipeline("Receiver")
                .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                .AddDebugMemoryDataCollector(out memp2)
                .AddChannelIncoming("crequest")
                    .AttachListener(bridgeOut.GetListener())
                    .AttachCommand(typeof(IContractFinal), (ctx) =>
                    {
                        var value = ctx.PayloadSerializer.PayloadDeserialize<string>(ctx.Request);

                        success = value == "Hello";
                        mre.Set();
                        return Task.FromResult(0);
                    })
                    .Revert()                    
                    ;

            p1.Start();
            p2.Start();

            //Send the message to the command asyncronously.
            p1.ToMicroservice().Dispatch.Process<IContractInitial>("Hello");

            mre.WaitOne();

            Assert.IsTrue(success);

            p1.Stop();
            p2.Stop();
        }
    }
}
