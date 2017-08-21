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
                DebugMemoryDataCollector memp1;
                PersistenceClient<Guid, Sample1> repo;

                var p1 = new MicroservicePipeline("Local")
                    .AddDebugMemoryDataCollector(out memp1)
                    .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddChannelIncoming("fredo")
                        .AttachPersistenceManagerHandlerMemory(
                            (Sample1 e) => e.Id, (s) => new Guid(s)
                            , versionPolicy: ((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid(), true)
                            , resourceProfile: ("paul1", true)
                            )
                        .AttachPersistenceClient(out repo)
                        .Revert()
                        ;

                p1.Start();

                var sample = new Sample1();

                //Run a set of simple version entity tests.
                //Create
                Assert.IsTrue(repo.Create(sample).Result.IsSuccess);
                //Read
                Assert.IsTrue(repo.Read(sample.Id).Result.IsSuccess);
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
                DebugMemoryDataCollector memp1;
                PersistenceClient<Guid, Sample1> repo;

                var p1 = new MicroservicePipeline("Local")
                    .AddDebugMemoryDataCollector(out memp1)
                    .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddChannelIncoming("fredo")
                        .AttachPersistenceManagerHandlerMemory(
                            (Sample1 e) => e.Id, (s) => new Guid(s)
                            , versionPolicy: ((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid(), true)
                            , resourceProfile: ("paul1", true)
                            )
                        .AttachPersistenceClient(out repo)
                        .Revert()
                        ;


                var p2 = new MicroservicePipeline("Local")
                    .AddDebugMemoryDataCollector(out memp1)
                    .AdjustPolicyCommunication((p, c) => p.BoundaryLoggingActiveDefault = true)
                    .AddChannelIncoming("fredo")
                        .AttachPersistenceManagerHandlerMemory(
                            (Sample1 e) => e.Id, (s) => new Guid(s)
                            , versionPolicy: ((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid(), true)
                            , resourceProfile: ("paul1", true)
                            )
                        .AttachPersistenceClient(out repo)
                        .Revert()
                        ;

                p1.Start();
                p2.Start();

                var sample = new Sample1();

                //Run a set of simple version entity tests.
                //Create
                Assert.IsTrue(repo.Create(sample).Result.IsSuccess);
                //Read
                Assert.IsTrue(repo.Read(sample.Id).Result.IsSuccess);
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

    }
}
