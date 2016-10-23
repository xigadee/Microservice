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
    public class SimpleCommand1UnitTest: CommandUnitTestBase<SimpleCommand1>
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

                var result1 = mCommandInit.Process<IDoSomething1, Blah, string>(new Blah() { Message = "hello" }).Result;

                pipeline.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }

    public class SimpleCommand1: CommandBase
    {
        public SimpleCommand1() : base(null){}

        [CommandContract(messageType: nameof(SimpleCommand1), actionType: "async")]
        private async Task ActionAsync(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {
            Process(incoming, outgoing);
        }

        [CommandContract(messageType: nameof(SimpleCommand1), actionType: "sync")]
        private async void ActionSync(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {
            Process(incoming, outgoing);
        }

        private void Process(TransmissionPayload incoming, List<TransmissionPayload> outgoing)
        {
            var rs = incoming.ToResponse();
            outgoing.Add(rs);
        }

    }


}
