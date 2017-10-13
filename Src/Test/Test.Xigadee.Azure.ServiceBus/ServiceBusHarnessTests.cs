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
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Models;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using Microsoft.Rest;
using System.Net.Http;
using System.Threading;

namespace Test.Xigadee.Azure.ServiceBus
{
    /// <summary>
    /// These test validate the pipeline.
    /// </summary>
    [TestClass]
    public class ServiceBusHarnessTests
    {
        #region TestContext
        /// <summary>
        /// All hail the Microsoft test magic man!
        /// This class can be populated with values through Visual Studio menus -> [Test>Test Settings>Select Test Settings File] and then selecting a file with the extension .runsettings
        /// See here for details: https://msdn.microsoft.com/en-us/library/jj635153.aspx
        /// There is a default file default.runsettings that has a set of empty CI injection parameters specified for testing in this project.
        /// </summary>
        public TestContext TestContext
        {
            get; set;
        }
        #endregion

        [TestMethod]
        public void HarnessSenderQueueTest()
        {
            var harness = new AzureServiceBusQueueSenderHarness();

            var config = TestContext.ToConfiguration();
            harness.Configure(config, "queuetest");

            harness.Start();
        }

        [TestMethod]
        public void HarnessListenerQueueTest()
        {
            var harness = new AzureServiceBusQueueListenerHarness();

            var config = TestContext.ToConfiguration();
            harness.Configure(config, "queuetest");

            harness.Start();
        }

        [TestMethod]
        public void HarnessSenderTopicTest()
        {
            var harness = new AzureServiceBusTopicSenderHarness();

            var config = TestContext.ToConfiguration();
            harness.Configure(config, "topictest");

            harness.Start();
        }

        [TestMethod]
        public void HarnessListenerTopicTest()
        {
            var harness = new AzureServiceBusTopicListenerHarness();

            var config = TestContext.ToConfiguration();
            harness.Configure(config, "topictest");

            harness.Start();
        }
    }
}
