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
    public class SimpleCommandUnitTest: CommandUnitTestBase<SimpleCommand>
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

                //var result1 = mCommandInit.Process<Blah, string>("internalIn", "franky", "johnny1",
                //    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromHours(1) }).Result;

                //var result2 = mCommandInit.Process<Blah, string>("internalIn", "simples2", "sync",
                //    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromHours(1) }).Result;

                //var result3 = mCommandInit.Process<Blah, string>("internalIn", "simples2", "syncout",
                //    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromHours(1) }).Result;

                var end = ConversionHelper.DeltaAsTimeSpan(start);

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }

    public class SimpleCommand: CommandBase
    {
        public SimpleCommand() : base(null){ }


        [CommandContract(messageType: "franky", actionType: "johnny1")]
        private async Task ThisisMeStupid1(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {

        }

        [CommandContract(messageType: "SimpleCommand2", actionType: "johnny6")]
        [return: PayloadOut]
        private async Task<string> ThisisMeStupid6n(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {
            return "ff";
        }

        [CommandContract(messageType: "franky", actionType: "johnny2")]
        private void ThisisMeStupid2([PayloadIn]Blah item)
        {

        }


        [CommandContract(typeof(IDoSomething1))]
        [return: PayloadOut]
        public string ThisisMeStupid3([PayloadIn]Blah item)
        {
            return "Hmm";
        }

        [CommandContract(messageType: "franky", actionType: "johnny4")]
        protected void ThisisMeStupid4([PayloadIn]string item)
        {

        }

        [CommandContract(messageType: "franky", actionType: "johnny5")]
        [CommandContract(messageType: "franky", actionType: "johnny5b")]
        private void ThisisMeStupid5([PayloadIn]Blah item, [PayloadOut]out string response)
        {
            response = null;
        }

        [CommandContract(messageType: "franky", actionType: "johnny6")]
        [return: PayloadOut]
        private async Task<string> ThisisMeStupid6(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {
            return "ff";
        }

        [CommandContract(messageType: "franky", actionType: "johnny7")]
        [return: PayloadOut]
        private async Task<Blah> ThisisMeStupid7(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {
            return new Blah() { Message = "Hello Mom" };
        }
    }
}
