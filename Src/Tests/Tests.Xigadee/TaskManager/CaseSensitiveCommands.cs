using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [Contract("INTERNALIN", "FRANKY", "johnny5")]
    public interface ICaseSensitiveTest1: IMessageContract { }

    [TestClass]
    public class CaseSensitiveCommands
    {
        /// <summary>
        /// This pipeline test specifically checks the AttachMessagePriorityOverrideForResponse functionality.
        /// </summary>
        [TestMethod]
        public void PipelineCaseInsensitive()
        {
            try
            {
                DebugMemoryDataCollector collector1, collector2;
                CommandInitiator init = null;

                var fabric = new ManualCommunicationFabric();
                var bridgeOut = fabric[ManualCommunicationFabricMode.Queue];
                var bridgeReturn = fabric[ManualCommunicationFabricMode.Broadcast];

                var pClient = new MicroservicePipeline("Client");
                var pServer = new MicroservicePipeline("Server");

                pServer
                    .AdjustPolicyTaskManagerForDebug()
                    .AddDebugMemoryDataCollector(out collector2)
                    .AddPayloadSerializerDefaultJson()
                    .AddChannelIncoming("INTERNALIN", internalOnly: false
                        , autosetPartition01: false)
                        .AttachPriorityPartition((0, 1.0M), (1, 0.9M))
                        .AttachListener(bridgeOut.GetListener())
                        .AttachMessagePriorityOverrideForResponse()
                        .AttachCommand((CommandMethodRequestContext ctx) =>
                        {                    
                            var payload = ctx.Request.Message.Holder;
                            ctx.ResponseSet(200, payload.Object);
                            return Task.FromResult(0);
                        }, ("FRANKY", "johnny5"))
                        .AttachCommand((CommandMethodRequestContext ctx) =>
                        {
                            var payload = ctx.RequestPayloadGet<Blah>();
                            ctx.ResponseSet(201, payload.Message);
                            return Task.FromResult(0);
                        }, ("franky", "JoHnny6"))
                        .Revert()
                    .AddChannelOutgoing("return")
                        .AttachSender(bridgeReturn.GetSender())
                        .Revert();
                ;

                pClient
                    .AdjustPolicyTaskManagerForDebug()
                    .AddDebugMemoryDataCollector(out collector1)
                    .AddChannelIncoming("Return")
                        .AttachListener(bridgeReturn.GetListener())
                        .AttachMessagePriorityOverrideForResponse()
                        .AttachCommandInitiator(out init)
                        .Revert()
                    .AddChannelOutgoing("internalIn", internalOnly: false
                        , autosetPartition01: false)
                        .AttachPriorityPartition(0, 1)
                        .AttachSender(bridgeOut.GetSender())
                        .Revert()
                        ;

                pClient.Start();
                pServer.Start();

                var list = new List<Task<ResponseWrapper<string>>>();

                list.Add(init.Process<ICaseSensitiveTest1, Blah, string>(new Blah() { Message = "hello1" }));
                list.Add(init.Process<Blah, string>("Internalin", "franky", "johnny5", new Blah() { Message = "hello2" }));
                list.Add(init.Process<Blah, string>(("InternalIn", "Franky", "johnny5"), new Blah() { Message = "hello3" }));
                list.Add(init.Process<Blah, string>(("internalIN", "FRANKY", "johnny6"), new Blah() { Message = "hello3" }));

                var result = Task.WhenAll(list).Result;

                result.ForEach((r) => Assert.IsTrue(r.ResponseCode == 200 || r.ResponseCode == 201));

                pClient.Stop();
                pServer.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
