#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class Microservice_Validate_Persistence
    {
        #region Declarations
        protected PersistenceManagerHandlerMemory<Guid, MyTestEntity1> mPersistenceCommand1;
        protected PersistenceClient<Guid, MyTestEntity1> mPersistenceService1;

        protected PersistenceManagerHandlerMemory<Guid, MyTestEntity2> mPersistenceCommand2;
        protected PersistenceClient<Guid, MyTestEntity2> mPersistenceService2;

        protected MicroservicePipeline mMs;
        #endregion

        [TestInitialize]
        public void Initialise()
        {
            mMs = new MicroservicePipeline();

            mMs.AddChannelIncoming("internal")
                .AttachPersistenceManagerHandlerMemory((MyTestEntity1 e) => e.Id, (e) => new Guid(e))
                .AttachPersistenceManagerHandlerMemory((MyTestEntity2 e) => e.Id
                    , (e) => new Guid(e)
                    , versionPolicy: new VersionPolicy<MyTestEntity2>((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid())
                    )
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
