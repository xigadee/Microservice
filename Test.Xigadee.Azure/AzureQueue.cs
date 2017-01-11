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

namespace Test.Xigadee.Azure
{
    /// <summary>
    /// This is the shared operation contract.
    /// </summary>
    [Contract(AzureQueue.ChannelIn, "Simple", "Command")]
    public interface ISimpleCommand: IMessageContract{}
    /// <summary>
    /// This is a simple command that receives a string and returns a string.
    /// </summary>
    public class SimpleCommand: CommandBase, IRequireDataCollector
    {
        public IDataCollection Collector{get;set;}

        [CommandContract(typeof(ISimpleCommand))]
        [return: PayloadOut]
        private string Method1([PayloadIn] string inData)
        {
            Collector.Write(new LogEvent("Hello"), true);

            return "mom";
        }

        [CommandContract("process","deadletter")]
        private void ProcessDeadletter(TransmissionPayload inData)
        {
            Collector.Write(new LogEvent("Hello"), true);
        }

        protected override void StartInternal()
        {
            base.StartInternal();
        }
    }

    [TestClass]
    public class AzureQueue
    {
        public const string ChannelIn = "remote";
        public const string SbConn = "Endpoint=sb://xigadeedev-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=WXZaLdK1w03CeZv7cvDZxENp6Nxy4OlEmWcKF1x/y+E=";

        [TestMethod]
        public void TestMethod1()
        {
            CommandInitiator init;

            try
            {
                var sender = new MicroservicePipeline("initiator")
                    .ConfigurationOverrideSet(AzureExtensionMethods.KeyServiceBusConnection, SbConn)
                    .AddChannelOutgoing("remote")
                        .AttachAzureServiceBusQueueSender()
                    .Revert()
                        .AddChannelIncoming("response")
                            .AttachAzureServiceBusTopicListener(listenOnOriginatorId: true)
                            .AttachCommandInitiator(out init)
                    ;

                var listener = new MicroservicePipeline("responder")
                    .ConfigurationOverrideSet(AzureExtensionMethods.KeyServiceBusConnection, SbConn)
                    .AddChannelIncoming("remote")
                        .AttachAzureServiceBusQueueListener()
                        .AttachCommand(new SimpleCommand())
                        .Revert()
                    .AddChannelIncoming("deadletter")
                        .AttachMessageRedirectRule((p) => true, new ServiceMessageHeader("remote", "process", "deadletter"))
                        .AttachAzureServiceBusQueueListener("remote", isDeadLetterListener: true)
                        .Revert()
                    .AddChannelOutgoing("response")
                        .AttachAzureServiceBusTopicSender()
                    ;

                listener.Start();

                sender.Start();


                var rs = init.Process<ISimpleCommand, string, string>("hello")?.Result;

                Assert.IsTrue(rs?.Response == "mom");

                sender.Stop();
                listener.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
