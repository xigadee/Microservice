using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class SimpleCommand3UnitTest: CommandUnitTestBase<SimpleCommand3>
    {
        [TestMethod]
        public void TestStandard3()
        {
            try
            {
                DefaultTest();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [TestMethod]
        public void PipelineCommand3()
        {
            try
            {
                var pipeline = Pipeline();

                pipeline.Start();
                int start = Environment.TickCount;

                var result1 = mCommandInit.Process<Blah, string>("internalIn", "simples3", "asyncin",
                    new Blah() { Message = "hello" }, new RequestSettings() { ProcessAsync=true, WaitTime = TimeSpan.FromSeconds(4) }).Result;

                //var result2 = mCommandInit.Process<Blah, string>("internalIn", "simples3", "syncin",
                //    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromSeconds(4) }).Result;

                //var result3 = mCommandInit.Process<Blah, string>("internalIn", "simples3", "asyncout",
                //    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromSeconds(4) }).Result;

                //var result4 = mCommandInit.Process<Blah, Blah>("internalIn", "simples3", "syncout",
                //    new Blah() { Message = "hello" }, new RequestSettings() { WaitTime = TimeSpan.FromSeconds(4) }).Result;

                var end = ConversionHelper.DeltaAsTimeSpan(start);

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

    }

    public class SimpleCommand3: CommandBase
    {
        public SimpleCommand3() : base(null) { }

        [CommandContract(messageType: "simples3", actionType: "asyncin")]
        private async Task ActionInAsync(TransmissionPayload inPayload)
        {

        }

        [CommandContract(messageType: "simples3", actionType: "syncin")]
        private void ActionInSync(TransmissionPayload inPayload)
        {

        }


        [CommandContract(messageType: "simples3", actionType: "asyncout")]
        private async Task ActionOutAsync(List<TransmissionPayload> outPayload)
        {

        }

        [CommandContract(messageType: "simples3", actionType: "syncout")]
        private void ActionOutSync(List<TransmissionPayload> outPayload)
        {

        }

    }
}
