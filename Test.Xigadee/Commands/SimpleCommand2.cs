using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class SimpleCommand2UnitTest: CommandUnitTestBase<SimpleCommand2>
    {
        [TestMethod]
        public void TestStandard()
        {
            DefaultTest();
        }


        [TestMethod]
        public void PipelineCommand()
        {
            try
            {
                var pipeline = Pipeline();

                pipeline.Start();

                int start = Environment.TickCount;

                var result1 = mCommandInit.Process<Blah, string>("internalIn", "simples2", "async",
                    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromHours(1) }).Result;

                var result2 = mCommandInit.Process<Blah, string>("internalIn", "simples2", "sync",
                    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromHours(1) }).Result;

                var result3 = mCommandInit.Process<Blah, string>("internalIn", "simples2", "syncout",
                    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromHours(1) }).Result;

                var end = ConversionHelper.DeltaAsTimeSpan(start);

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }

    public class SimpleCommand2: CommandBase
    {
        public SimpleCommand2() : base(null){}

        [CommandContract(messageType: "simples2", actionType: "async")]
        [return: PayloadOut]
        private async Task<string> ActionAsync([PayloadIn] Blah data)
        {
            return "Freaky";
            //var rs = incoming.ToResponse();
            //rs.MessageObject = "Freaky";
            //rs.Message.Status = "204";
            //rs.Message.StatusDescription = "Hello";
            //outgoing.Add(rs);
        }

        [CommandContract(messageType: "simples2", actionType: "sync")]
        [return: PayloadOut]
        private string ActionSync([PayloadIn] Blah data)
        {
            return "Super freaky";
            //var rs = incoming.ToResponse();
            //rs.Message.Blob = PayloadSerializer.PayloadSerialize("");
            //rs.Message.Status = "204";
            //rs.Message.StatusDescription = "Hello";
            //outgoing.Add(rs);
        }

        [CommandContract(messageType: "simples2", actionType: "syncout")]
        private void ActionSyncOut([PayloadIn] Blah data, [PayloadOut] out string item)
        {
            item = "Super freaky doo dah";
            //var rs = incoming.ToResponse();
            //rs.Message.Blob = PayloadSerializer.PayloadSerialize("");
            //rs.Message.Status = "204";
            //rs.Message.StatusDescription = "Hello";
            //outgoing.Add(rs);
        }

    }
}
