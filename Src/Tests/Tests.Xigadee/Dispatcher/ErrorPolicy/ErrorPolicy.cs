using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Dispatcher.ErrorPolicy
{
    [TestClass]
    public class ErrorPolicy
    {
        public class PipeTest1
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public string Message { get; set; }
        }

        private void Events_ProcessRequestUnresolved(object sender, DispatcherRequestUnresolvedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Events_ProcessRequestError(object sender, ProcessRequestErrorEventArgs e)
        {
        }

        [TestMethod]
        public void RPipeInternal()
        {
            PersistenceClient<Guid,PipeTest1> persistence, persistence2;
            CommandInitiator init;

            var server = new MicroservicePipeline(nameof(RPipeInternal));

            server.ToMicroservice().Events.ProcessRequestError += Events_ProcessRequestError;
            server.ToMicroservice().Events.ProcessRequestUnresolved += Events_ProcessRequestUnresolved;
            server
                .AddChannelIncoming("inChannel")
                    .AttachPersistenceManagerHandlerMemory((PipeTest1 e) => e.Id, (s) => new Guid(s),resourceProfile:"Entity")
                    .AttachPersistenceClient(out persistence)
                    .Revert()
                .AddChannelIncoming("inChannel2")
                    .AttachPersistenceClient(out persistence2
                        , adjustPolicy:(p) => p.TransmissionPayloadTraceEnabled = true)
                    .Revert()
                .AddChannelIncoming("backout")
                    .AttachCommandInitiator(out init, adjustPolicy: (p) => p.TransmissionPayloadTraceEnabled = true)
                    ;
            server.Start();

            var result = persistence.Create(new PipeTest1() { Message = "Hello" }
                , new RepositorySettings() { WaitTime = TimeSpan.FromSeconds(5) }
                ).Result;

            Assert.IsTrue(result.ResponseCode == 201);

            var result2 = persistence2.Create(new PipeTest1() { Message = "Hello" }
                , new RepositorySettings() { WaitTime = TimeSpan.FromSeconds(5) }
                ).Result;

            Assert.IsTrue(result2.ResponseCode == 501);

            var result3 = init.Process<string,string>(("franky","four","fingers"), ""
                , new RequestSettings { WaitTime = TimeSpan.FromSeconds(500) }, routing: ProcessOptions.RouteInternal
                ).Result;

            Assert.IsTrue(result3.ResponseCode == 501);

        }

        [TestMethod]
        public void RPipeExternalCheckCaseSensitivity()
        {
            PersistenceClient<Guid, PipeTest1> persistence;
            CommandInitiator init;
            DebugMemoryDataCollector collectorS, collectorC;

            var fabric = new ManualFabric();
            var bridgeOut = fabric.Queue;
            var bridgeReturn = fabric.Broadcast;

            var server = new MicroservicePipeline($"{nameof(RPipeExternalCheckCaseSensitivity)}server");
            var client = new MicroservicePipeline($"{nameof(RPipeExternalCheckCaseSensitivity)}client");

            server.ToMicroservice().Events.ProcessRequestError += Events_ProcessRequestError;
            server.ToMicroservice().Events.ProcessRequestUnresolved += Events_ProcessRequestUnresolved;

            client.ToMicroservice().Events.ProcessRequestError += Events_ProcessRequestError;
            client.ToMicroservice().Events.ProcessRequestUnresolved += Events_ProcessRequestUnresolved;

            server
                .AdjustPolicyTaskManagerForDebug()
                .AddDebugMemoryDataCollector(out collectorS)
                .AddChannelIncoming("inChannel")
                    .AttachPersistenceManagerHandlerMemory(
                          (PipeTest1 e) => e.Id
                        , (s) => new Guid(s)
                        , resourceProfile: "Entity")
                    .AttachListener(bridgeOut.GetListener())

                    .Revert()
                .AddChannelOutgoing("return")
                    .AttachSender(bridgeReturn.GetSender())
                    .Revert()
                    ;

            client
                .AdjustPolicyTaskManagerForDebug()
                .AddDebugMemoryDataCollector(out collectorC)
                .AddChannelIncoming("return")
                    .AttachCommandInitiator(out init)
                    .AttachListener(bridgeReturn.GetListener())
                    .Revert()
                .AddChannelOutgoing("inChannel")
                    .AttachPersistenceClient("return", out persistence)
                    .AttachSender(bridgeOut.GetSender())
                    .Revert()
                    ;

            server.Start();
            client.Start();

            var result = persistence.Create(new PipeTest1() { Message = "Hello" }
                , new RepositorySettings() { WaitTime = TimeSpan.FromSeconds(5) }
                ).Result;

            Assert.IsTrue(result.ResponseCode == 201);

        }

    }
}
