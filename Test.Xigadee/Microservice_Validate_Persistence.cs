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
        protected PersistenceManagerHandlerMemory<Guid, MyTestEntity> mPersistence;
        protected PersistenceSharedService<Guid, MyTestEntity> mService;

        protected override void RegisterCommands()
        {
            base.RegisterCommands();

            mPersistence = (PersistenceManagerHandlerMemory<Guid, MyTestEntity>)Service.RegisterCommand(new PersistenceManagerHandlerMemory<Guid, MyTestEntity>((e) => e.Id, (e) => new Guid(e)) { ChannelId = "internal" });
            mService = (PersistenceSharedService<Guid, MyTestEntity>)Service.RegisterCommand(new PersistenceSharedService<Guid, MyTestEntity>() { ChannelId = "internal" } );
        }

        [TestMethod]
        public void CreateReadUpdateDeleteEntity()
        {
            var entity = new MyTestEntity();

            var response1 = mService.Create(entity).Result;
            Assert.IsTrue(response1.IsSuccess);

            var response2 = mService.Read(entity.Id).Result;
            Assert.IsTrue(response2.IsSuccess);

            //var response3 = mService.Update(entity).Result;
            //Assert.IsTrue(response3.IsSuccess);

            //var response4 = mService.Delete(entity.Id).Result;
            //Assert.IsTrue(response4.IsSuccess);

            //var response5 = mService.Read(entity.Id).Result;
            //Assert.IsFalse(response5.IsSuccess);
        }
    }
}
