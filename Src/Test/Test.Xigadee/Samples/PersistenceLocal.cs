using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Samples
{
    [TestClass]
    public class PersistenceLocal
    {
        [TestMethod]
        public void PersistenceLocal1()
        {
            try
            {
                DebugMemoryDataCollector memp1;
                PersistenceClient<Guid, Sample1> init;

                var p1 = new MicroservicePipeline(nameof(PersistenceLocal1))
                    .AddDebugMemoryDataCollector(out memp1)
                    .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                    .AddChannelIncoming("fredo")
                        .AttachPersistenceManagerHandlerMemory(
                            (Sample1 e) => e.Id, (s) => new Guid(s)
                            , versionPolicy: ((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid(), true)
                            , resourceProfile: ("paul1", true)
                            )
                        .AttachPersistenceClient(out init)
                        .Revert()
                        ;

                p1.Start();

                var sample = new Sample1();

                //Run a set of simple version entity tests.
                //Create
                Assert.IsTrue(init.Create(sample).Result.IsSuccess);
                //Read
                Assert.IsTrue(init.Read(sample.Id).Result.IsSuccess);
                //Update success
                var rs = init.Update(sample).Result;
                Assert.IsTrue(rs.IsSuccess);
                //We have enabled version policy and optimitic locking so the next command should fail.
                //Update fail as old version
                Assert.IsFalse(init.Update(sample).Result.IsSuccess);
                //But this one should pass.
                //Update pass as new entity.
                Assert.IsTrue(init.Update(rs.Entity).Result.IsSuccess);
                //Read
                Assert.IsTrue(init.Read(sample.Id).Result.IsSuccess);
                //Delete
                Assert.IsTrue(init.Delete(sample.Id).Result.IsSuccess);
                //Read fail.
                Assert.IsFalse(init.Read(sample.Id).Result.IsSuccess);

                p1.Stop();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// This is the local class used to test persistence.
        /// </summary>
        public class Sample1
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid VersionId { get; set; } = Guid.NewGuid();
        }
    }
}
