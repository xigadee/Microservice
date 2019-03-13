using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
namespace Test.Xigadee
{
    [TestClass]
    public class Microservice_Validate_Persistence
    {
        #region Declarations
        protected RepositoryMemory<Guid, MyTestEntity1> repo1;
        protected RepositoryMemory<Guid, MyTestEntity2> repo2;

        protected PersistenceCommand<Guid, MyTestEntity1> mPersistenceCommand1;
        protected PersistenceCommand<Guid, MyTestEntity2> mPersistenceCommand2;

        protected PersistenceClient<Guid, MyTestEntity1> mPersistenceService1;
        protected PersistenceClient<Guid, MyTestEntity2> mPersistenceService2;

        protected MicroservicePipeline mMs;
        #endregion

        [TestInitialize]
        public void Initialise()
        {
            mMs = new MicroservicePipeline();

            repo1 = new RepositoryMemory<Guid, MyTestEntity1>((MyTestEntity1 e) => e.Id);
            repo2 = new RepositoryMemory<Guid, MyTestEntity2>((MyTestEntity2 e) => e.Id
                    , versionPolicy: new VersionPolicy<MyTestEntity2>((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid())
            );

            mMs.AddChannelIncoming("internal")
                .AttachPersistenceRepositoryCommand(repo1, out mPersistenceCommand1)
                .AttachPersistenceRepositoryCommand(repo2, out mPersistenceCommand2)
                .AttachPersistenceClient(out mPersistenceService1)
                .AttachPersistenceClient(out mPersistenceService2)
                ;

            mMs.Start();
        }

        [TestMethod]
        public void CreateReadUpdateDeleteEntityWithOptimisticLocking()
        {
            Assert.AreEqual(mMs.ToMicroservice().Status, ServiceStatus.Running);

            var entity = new MyTestEntity2();

            //mPersistenceCommand2.DiagnosticsSetMessageDelay(TimeSpan.FromSeconds(5));
            var response1 = mPersistenceService2.Create(entity, new RepositorySettings() { WaitTime = TimeSpan.FromSeconds(10) }).Result;
            Assert.IsTrue(response1.IsSuccess);
            Assert.AreEqual(response1.ResponseCode, 201);

            var response2 = mPersistenceService2.Read(entity.Id).Result;
            Assert.IsTrue(response2.IsSuccess);
            Assert.AreEqual(response2.ResponseCode, 200);

            var response3 = mPersistenceService2.Update(response2.Entity).Result;
            Assert.IsTrue(response3.IsSuccess);

            var response4 = mPersistenceService2.Delete(entity.Id).Result;
            Assert.IsTrue(response4.IsSuccess);
            Assert.AreEqual(response4.ResponseCode, 200);

            //var response5 = mPersistenceService2.Version(entity.Id).Result;
            //Assert.IsTrue(response5.IsSuccess);
            //Assert.AreEqual(response5.Entity.Item2, 200);

            var response6 = mPersistenceService2.Read(entity.Id).Result;
            Assert.IsFalse(response6.IsSuccess);
        }

        [TestMethod]
        public void CreateReadUpdateDeleteEntity()
        {
            Assert.AreEqual(mMs.ToMicroservice().Status, ServiceStatus.Running);

            var entity = new MyTestEntity1();

            var response1 = mPersistenceService1.Create(entity).Result;
            Assert.IsTrue(response1.IsSuccess);
            Assert.AreEqual(response1.ResponseCode, 201);

            var response2 = mPersistenceService1.Read(entity.Id).Result;
            Assert.IsTrue(response2.IsSuccess);
            Assert.AreEqual(response2.ResponseCode, 200);

            var response3 = mPersistenceService1.Update(entity).Result;
            Assert.IsTrue(response3.IsSuccess);

            var response4 = mPersistenceService1.Delete(entity.Id).Result;
            Assert.IsTrue(response4.IsSuccess);
            Assert.AreEqual(response4.ResponseCode, 200);

            var response5 = mPersistenceService1.Read(entity.Id).Result;
            Assert.IsFalse(response5.IsSuccess);
        }

        public class MyTestEntity1
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public string Hello { get; set; } = DateTime.UtcNow.ToLongDateString();
        }

        public class MyTestEntity2
        {
            public Guid Id { get; set; } = Guid.NewGuid();

            public Guid VersionId { get; set; } = Guid.NewGuid();

            public string Hello { get; set; } = DateTime.UtcNow.ToLongDateString();
        }
    }
}
