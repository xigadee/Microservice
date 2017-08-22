using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Samples
{
    /// <summary>
    /// Simple test to demonstrate Microservice refactoring.
    /// </summary>
    [TestClass]
    public class PersistenceLocal
    {

        /// <summary>
        /// This is the POCO class used to test persistence.
        /// </summary>
        public class Sample1
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid VersionId { get; set; } = Guid.NewGuid();
            public string Message { get; set; }
        }

        /// <summary>
        /// A single Microservice example.
        /// </summary>
        [TestMethod]
        public void PersistenceSingle()
        {
            try
            {
                PersistenceClient<Guid, Sample1> repo;

                var p1 = new MicroservicePipeline("Local")
                    .AddChannelIncoming("request")
                        .AttachPersistenceManagerHandlerMemory(
                              keyMaker: (Sample1 e) => e.Id
                            , keyDeserializer: (s) => new Guid(s)
                            , versionPolicy: ((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid(), true)
                        )
                        .AttachPersistenceClient(out repo)
                    .Revert()
                    ;

                p1.Start();

                var sample = new Sample1() { Message = "Hello mom" };
                var id = sample.Id;
                //Run a set of simple version entity tests.
                //Create
                Assert.IsTrue(repo.Create(sample).Result.IsSuccess);
                //Read
                var result = repo.Read(id).Result;
                Assert.IsTrue(result.IsSuccess);
                Assert.IsTrue(result.Entity.Message == "Hello mom");
                //Update success
                var rs = repo.Update(sample).Result;
                Assert.IsTrue(rs.IsSuccess);
                //We have enabled version policy and optimistic locking so the next command should fail.
                //Update fail as old version
                Assert.IsFalse(repo.Update(sample).Result.IsSuccess);
                //But this one should pass.
                //Update pass as new entity.
                Assert.IsTrue(repo.Update(rs.Entity).Result.IsSuccess);
                //Read
                Assert.IsTrue(repo.Read(sample.Id).Result.IsSuccess);
                //Delete
                Assert.IsTrue(repo.Delete(sample.Id).Result.IsSuccess);
                //Read fail.
                Assert.IsFalse(repo.Read(sample.Id).Result.IsSuccess);

                p1.Stop();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// A refactored client-server example using a manual communication bridge.
        /// </summary>
        [TestMethod]
        public void PersistenceClientServer()
        {
            try
            {
                var fabric = new ManualFabricBridge();
                var bridgeRequest = new ManualCommunicationBridgeAgent(fabric, CommunicationBridgeMode.RoundRobin);
                var bridgeResponse = new ManualCommunicationBridgeAgent(fabric, CommunicationBridgeMode.Broadcast);

                PersistenceClient<Guid, Sample1> repo;

                var p1 = new MicroservicePipeline("Server")
                    .AddChannelIncoming("request")
                        .AttachPersistenceManagerHandlerMemory(
                              keyMaker: (Sample1 e) => e.Id
                            , keyDeserializer: (s) => new Guid(s)
                            , versionPolicy: ((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid(), true)
                            )
                        .AttachListener(bridgeRequest.GetListener())
                        .Revert()
                    .AddChannelOutgoing("response")
                        .AttachSender(bridgeResponse.GetSender())
                        ;

                var p2 = new MicroservicePipeline("Client")
                    .AddChannelIncoming("response")
                        .AttachListener(bridgeResponse.GetListener())
                        .Revert()
                    .AddChannelOutgoing("request")
                        .AttachSender(bridgeRequest.GetSender())
                        .AttachPersistenceClient("response",out repo)
                        .Revert()
                        ;

                p1.Start();
                p2.Start();

                var sample = new Sample1() { Message = "Hello mom" };
                var id = sample.Id;
                //Run a set of simple version entity tests.
                //Create
                Assert.IsTrue(repo.Create(sample).Result.IsSuccess);
                //Read
                var result = repo.Read(id).Result;
                Assert.IsTrue(result.IsSuccess);
                Assert.IsTrue(result.Entity.Message == "Hello mom");
                //Update success
                var rs = repo.Update(sample).Result;
                Assert.IsTrue(rs.IsSuccess);
                //We have enabled version policy and optimistic locking so the next command should fail.
                //Update fail as old version
                Assert.IsFalse(repo.Update(sample).Result.IsSuccess);
                //But this one should pass.
                //Update pass as new entity.
                Assert.IsTrue(repo.Update(rs.Entity).Result.IsSuccess);
                //Read
                Assert.IsTrue(repo.Read(sample.Id).Result.IsSuccess);
                //Delete
                Assert.IsTrue(repo.Delete(sample.Id).Result.IsSuccess);
                //Read fail.
                Assert.IsFalse(repo.Read(sample.Id).Result.IsSuccess);

                p1.Stop();
                p2.Stop();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
