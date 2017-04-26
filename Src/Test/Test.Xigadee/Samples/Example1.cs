using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Xigadee.Samples
{
    [TestClass]
    public class Example1
    {
        public class Sample1
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public Guid VersionId { get; set; } = Guid.NewGuid();
        }

        [TestMethod]
        public void MicroserviceExample1()
        {
            DebugMemoryDataCollector memp1;
            PersistenceMessageInitiator<Guid, Sample1> init;

            var p1 = new MicroservicePipeline(nameof(MicroserviceExample1))
                .AddDebugMemoryDataCollector(out memp1)
                .AdjustPolicyCommunication((p) => p.BoundaryLoggingActiveDefault = true)
                .AddChannelIncoming("fredo")
                    .AttachPersistenceManagerHandlerMemory(
                        (Sample1 e) =>e.Id
                        , (s) => new Guid(s)
                        , versionPolicy: ((e) => e.Id.ToString("N"), (e) => e.VersionId = Guid.NewGuid(), true)
                        , resourceProfile:("paul1",true)
                        )
                    .AttachPersistenceMessageInitiator(out init)
                    .Revert()
                    ;

            p1.Start();

            var sample = new Sample1();

            //Run a set of simple entity tests.
            Assert.IsTrue(init.Create(sample).Result.IsSuccess);
            Assert.IsTrue(init.Read(sample.Id).Result.IsSuccess);
            Assert.IsTrue(init.Update(sample).Result.IsSuccess);
            Assert.IsTrue(init.Read(sample.Id).Result.IsSuccess);
            Assert.IsTrue(init.Delete(sample.Id).Result.IsSuccess);
            Assert.IsFalse(init.Read(sample.Id).Result.IsSuccess);

            p1.Stop();
        }
    }
}
