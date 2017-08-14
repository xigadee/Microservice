using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Commands.Harness
{
    [TestClass]
    public class Harness1
    {
        public class CommandHarnessTest1: CommandBase
        {
            public CommandHarnessTest1(CommandPolicy policy = null) :base(policy)
            {

            }

            [CommandContract("one","two")]
            public void Command1(TransmissionPayload inPayload, List<TransmissionPayload> outPayload)
            {

            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            var policy = new CommandPolicy();

            try
            {
                var harness = new CommandHarness<CommandHarnessTest1>(() => new CommandHarnessTest1(policy));

                harness.Start();

                harness.Stop();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
