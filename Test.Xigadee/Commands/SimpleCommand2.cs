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
        public void TestStandard2()
        {
            DefaultTest();
        }

        [TestMethod]
        public void PipelineCommand2()
        {
            IPipeline pipeline = null;
            try
            {
                pipeline = Pipeline();

                pipeline.Start();
                pipeline.Service.ProcessRequestUnresolved += Service_ProcessRequestUnresolved;
                int start = Environment.TickCount;

                var result1 = mCommandInit.Process<Blah, string>("internalIn", "simples2", "async",
                    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromSeconds(4) }).Result;

                var result2 = mCommandInit.Process<Blah, string>("internalIn", "simples2", "sync",
                    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromSeconds(4) }).Result;

                var result3 = mCommandInit.Process<Blah, string>("internalIn", "simples2", "syncout",
                    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromSeconds(4) }).Result;

                //var result4 = mCommandInit.Process<Blah, Blah>("internalIn", "simples2", "asyncobj",
                //    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromSeconds(4) }).Result;

                var end = ConversionHelper.DeltaAsTimeSpan(start);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                pipeline?.Stop();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Service_ProcessRequestUnresolved(object sender, DispatcherRequestUnresolvedEventArgs e)
        {
            
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
