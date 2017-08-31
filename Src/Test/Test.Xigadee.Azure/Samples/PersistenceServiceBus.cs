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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.AzureSamples
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
        /// All hail the Microsoft test magic man!
        /// This class can be populated with values through Visual Studio menus -> [Test>Test Settings>Select Test Settings File] and then selecting a file with the extension .runsettings
        /// See here for details: https://msdn.microsoft.com/en-us/library/jj635153.aspx
        /// There is a default file default.runsettings that has a set of empty CI injection parameters specified for testing in this project.
        /// </summary>
        public TestContext TestContext
        {
            get;set;
        }

        /// <summary>
        /// A refactored client-server example using a manual communication bridge.
        /// </summary>
        [TestMethod]
        public void PersistenceAzureClientServer()
        {
            try
            {
                //Either use a .runsettings file to set this value 'CI_ServiceBusConnection' or just manually set the value here if you want to run the test.
                var sbConnection = TestContext.GetCISettingAsString(AzureConfigShortcut.ServiceBusConnection.ToSettingKey());

                PersistenceClient <Guid, Sample1> repo;

                var p1 = new MicroservicePipeline("Server")
                    .AzureConfigurationOverrideSet(AzureConfigShortcut.ServiceBusConnection, sbConnection)
                    .AddChannelIncoming("request")
                        .AttachPersistenceManagerHandlerMemory(
                              keyMaker: (Sample1 e) => e.Id
                            , keyDeserializer: (s) => new Guid(s)
                            , versionPolicy: ((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid(), true)
                            )
                        .AttachAzureServiceBusQueueListener()
                        .Revert()
                    .AddChannelOutgoing("response")
                        .AttachAzureServiceBusTopicSender()
                        ;

                var p2 = new MicroservicePipeline("Client")
                    .AzureConfigurationOverrideSet(AzureConfigShortcut.ServiceBusConnection, sbConnection)
                    .AddChannelIncoming("response")
                        .AttachAzureServiceBusTopicListener()
                        .Revert()
                    .AddChannelOutgoing("request")
                        .AttachAzureServiceBusQueueSender()
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
