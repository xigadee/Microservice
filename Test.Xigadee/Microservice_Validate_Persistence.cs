using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    public class MyTestEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Hello { get; set;} = DateTime.UtcNow.ToLongDateString();
    }

    [TestClass]
    public class Microservice_Validate_Persistence: TestPopulator<TestMicroservice, TestConfig>
    {
        protected PersistenceManagerHandlerMemory<Guid, MyTestEntity> mPersistenceCommand;
        protected PersistenceSharedService<Guid, MyTestEntity> mPersistenceService;

        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            mPersistenceCommand = (PersistenceManagerHandlerMemory<Guid, MyTestEntity>)Service.RegisterCommand(new PersistenceManagerHandlerMemory<Guid, MyTestEntity>((e) => e.Id, (e) => new Guid(e)) { ChannelId = "internal" });
            mPersistenceService = (PersistenceSharedService<Guid, MyTestEntity>)Service.RegisterCommand(new PersistenceSharedService<Guid, MyTestEntity>() { ChannelId = "internal" } );
        }

        [TestMethod]
        public void CreateReadUpdateDeleteEntity()
        {
            Assert.AreEqual(Service.Status, ServiceStatus.Running);

            var entity = new MyTestEntity();

            var response1 = mPersistenceService.Create(entity).Result;
            Assert.IsTrue(response1.IsSuccess);
            Assert.AreEqual(response1.ResponseCode, 201);

            var response2 = mPersistenceService.Read(entity.Id).Result;
            Assert.IsTrue(response2.IsSuccess);
            Assert.AreEqual(response2.ResponseCode, 200);

            var response3 = mPersistenceService.Update(entity).Result;
            //Assert.IsTrue(response3.IsSuccess);

            var response4 = mPersistenceService.Delete(entity.Id).Result;
            Assert.IsTrue(response4.IsSuccess);
            Assert.AreEqual(response4.ResponseCode, 200);

            var response5 = mPersistenceService.Read(entity.Id).Result;
            Assert.IsFalse(response5.IsSuccess);
        }
    }
}
