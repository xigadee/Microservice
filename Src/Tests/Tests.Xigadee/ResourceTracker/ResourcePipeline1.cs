using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.ResourceTracker
{
    [TestClass]
    public class ResourcePipeline1
    {
        public class PipeTest1
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public string Message { get; set; }
        }

        [TestMethod]
        public void RPipe1()
        {
            PersistenceClient<Guid,PipeTest1> persistence, persistence2;
            CommandInitiator init;

            var server = new MicroservicePipeline(nameof(RPipe1));

            server.ToMicroservice().Events.ProcessRequestError += Events_ProcessRequestError;
            server.ToMicroservice().Events.ProcessRequestUnresolved += Events_ProcessRequestUnresolved;
            server
                .AddChannelIncoming("inChannel")
                    .AttachPersistenceManagerHandlerMemory((PipeTest1 e) => e.Id, (s) => new Guid(s),resourceProfile:"Entity")
                    .AttachPersistenceClient(out persistence)
                    .Revert()
                .AddChannelIncoming("inChannel2")
                    .AttachPersistenceClient(out persistence2)
                    .Revert()
                .AddChannelIncoming("backout")
                    .AttachCommandInitiator(out init)
                    ;
            server.Start();

            var result = persistence.Create(new PipeTest1() { Message = "Hello" }
                , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }
                ).Result;

            var result2 = persistence2.Create(new PipeTest1() { Message = "Hello" }
                , new RepositorySettings() { WaitTime = TimeSpan.FromMinutes(5) }
                ).Result;

            var result3 = init.Process<string,string>(("franky","four","fingers"), ""
                , new RequestSettings { WaitTime = TimeSpan.FromMinutes(5) }, routing: ProcessOptions.RouteInternal
                ).Result;

        }

        private void Events_ProcessRequestUnresolved(object sender, DispatcherRequestUnresolvedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Events_ProcessRequestError(object sender, ProcessRequestErrorEventArgs e)
        {
        }
    }
}
