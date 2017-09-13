using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Test.Xigadee.Commands.Harness
{
    /// <summary>
    /// This class checks the inheritance behaviour for the attributes and policy for the command.
    /// </summary>
    [TestClass]
    public class HarnessReflection
    {
        class CommandRoot : CommandBase
        {
            public CommandRoot(CommandPolicy policy = null) : base(policy){}

            [CommandContract("one", "two")]
            [return: PayloadOut]
            public async Task<string> Command1([PayloadIn]string inParam, TransmissionPayload inPayload, List<TransmissionPayload> outPayload)
            {
                var back = Outgoing.Process<string, string>(("one", "two", "three"), "Hello").Result;
                return back.Response;
            }

            [JobSchedule("1")]
            public async Task Schedule1()
            {
                var back = Outgoing.Process<string, string>(("one", "two", "four"), "Hello").Result;
            }
        }

        class CommandTop : CommandRoot
        {
            public CommandTop(CommandPolicy policy = null) : base(policy){}


            [JobSchedule("1base")]
            public async Task ScheduleBase()
            {
                var back = Outgoing.Process<string, string>(("one", "two", "four"), "Hello").Result;
            }

            [CommandContract("one", "base")]
            [return: PayloadOut]
            public async Task<string> CommandBase([PayloadIn]string inParam, TransmissionPayload inPayload, List<TransmissionPayload> outPayload)
            {
                var back = Outgoing.Process<string, string>(("one", "two", "three"), "Hello").Result;
                return back.Response;
            }
        }

        [TestMethod]
        public void TestInheritance()
        {
            var policy = new CommandPolicy() { ChannelId = "default" };
            //Default state.
            var harness = new CommandHarness<CommandTop>(policy);

            harness.Start();
            Assert.IsTrue(harness.RegisteredSchedules.Count == 2);
            Assert.IsTrue(harness.RegisteredCommandMethods.Count == 2);
        }


        [TestMethod]
        public void TestInheritanceDisabled()
        {
            var policy = new CommandPolicy() { ChannelId = "default", CommandContractAttributeInherit  =false, JobScheduleAttributeInherit = false };
            //Default state.
            var harness = new CommandHarness<CommandTop>(policy);

            harness.Start();
            Assert.IsTrue(harness.RegisteredSchedules.Count == 1);
            Assert.IsTrue(harness.RegisteredCommandMethods.Count == 1);
        }
    }
}
