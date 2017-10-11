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
    public class ServiceBusTest1
    {
        ServiceBusConnectionStringBuilder connMgmt = new ServiceBusConnectionStringBuilder("Endpoint=sb://x2test.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=PSTVeTZ23D7A07vuFfR6lvHSFV2FZb6zyS5/GkiJO5Q=");
        ServiceBusConnectionStringBuilder connRdWr = new ServiceBusConnectionStringBuilder("Endpoint=sb://x2test.servicebus.windows.net/;SharedAccessKeyName=ConnectionOnly;SharedAccessKey=gyM2OKVVnSuFWJcXObKBbhacbqb4G8AN1nu4uBURVBg=");
        //sb://x2test.servicebus.windows.net/fredo123

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
        public void Pipeline1()
        {
            //var fabric = new ManualFabricBridge();

            var fabric = new AzureServiceBusFabricBridge(connMgmt);
             
            var server = new MicroservicePipeline()
                .ConfigResolverSetTestContext(TestContext)
                .FabricConfigure(fabric)
                .AdjustPolicyTaskManagerForDebug()
                .AddChannelIncoming("incoming")
                    .AttachListener(fabric.Queue.GetListener())
                    .AttachCommand((ctx) => 
                    {
                        var incoming = ctx.RequestPayloadGet<string>();
                        ctx.ResponseSet(200, "Howdy");
                        return Task.FromResult(0);
                    }
                    , ("one","two"))
                    .Revert()
                .AddChannelOutgoing("outgoing")
                    .AttachSender(fabric.Broadcast.GetSender())
                    .Revert()
                ;

            ICommandInitiator init;
            var client = new MicroservicePipeline()
                .ConfigResolverSetTestContext(TestContext)
                .AdjustPolicyTaskManagerForDebug()
                .AddChannelOutgoing("incoming")
                    .AttachSender(fabric.Queue.GetSender())
                    .Revert()
                .AddChannelIncoming("outgoing")
                    .AttachListener(fabric.Broadcast.GetListener())
                    .AttachICommandInitiator(out init)
                    .Revert()
                ;

            server.Start();
            client.Start();

            int t1 = Environment.TickCount;
            var result1 = init.Process<string, string>(("incoming", "one", "two"), "Hello").Result;
            int t2 = Environment.TickCount;
            var result2 = init.Process<string, string>(("incoming", "one", "two"), "Hello").Result;
            int t3 = Environment.TickCount;
            var result3 = init.Process<string, string>(("incoming", "one", "two"), "Hello").Result;
            int t4 = Environment.TickCount;
        }
    }
}
