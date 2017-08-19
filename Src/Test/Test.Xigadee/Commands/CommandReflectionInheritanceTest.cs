using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Commands
{
    [TestClass]
    public class CommandReflectionInheritanceTest
    {
        public class CommandRoot: CommandBase
        {
            [JobSchedule("1")]
            public void Schedule1(Schedule schedule) { }

            [MasterJobSchedule("1a")]
            public void Schedule1a(Schedule schedule) { }

            [CommandContract("one", "1")]
            public void Command1(TransmissionPayload payload) { }

            [MasterJobCommandContract("one", "1a")]
            public void Command1a(TransmissionPayload payload) { }

        }

        public class CommandRoot2: CommandRoot
        {
            [JobSchedule("2")]
            public void Schedule2() { }

            [MasterJobSchedule("2a")]
            public void Schedule2a(Schedule schedule){}

            [CommandContract("one", "2")]
            public void Command2(TransmissionPayload payload) { }

            [MasterJobCommandContract("one", "2a")]
            public void Command2a(TransmissionPayload payload) { }

        }

        [TestMethod]
        public void CommandAttributeInheritanceTestSchedule()
        {
            var command = new CommandRoot2();

            var attrs1 = command.CommandMethods<JobScheduleAttribute>(true).ToList();
            var attrs2 = command.CommandMethods<JobScheduleAttribute>(false).ToList();
            var attrs1a = command.CommandMethods<MasterJobScheduleAttribute>(true).ToList();
            var attrs2a = command.CommandMethods<MasterJobScheduleAttribute>(false).ToList();

            Assert.IsTrue(attrs1.Count == 2);
            Assert.IsTrue(attrs2.Count == 1);
            Assert.IsTrue(attrs1a.Count == 2);
            Assert.IsTrue(attrs2a.Count == 1);
            Assert.IsTrue(attrs2[0].Item1.Name == "2");
            Assert.IsTrue(attrs2a[0].Item1.Name == "2a");
        }


        [TestMethod]
        public void CommandAttributeInheritanceTestContract()
        {
            var command = new CommandRoot2();

            var attrs1 = command.CommandMethods<CommandContractAttribute>(true).ToList();
            var attrs2 = command.CommandMethods<CommandContractAttribute>(false).ToList();
            var attrs1a = command.CommandMethods<MasterJobCommandContractAttribute>(true).ToList();
            var attrs2a = command.CommandMethods<MasterJobCommandContractAttribute>(false).ToList();

            Assert.IsTrue(attrs1.Count == 2);
            Assert.IsTrue(attrs2.Count == 1);
            Assert.IsTrue(attrs1a.Count == 2);
            Assert.IsTrue(attrs2a.Count == 1);
            Assert.IsTrue(attrs2[0].Item1.Reference == "/one/2");
            Assert.IsTrue(attrs2a[0].Item1.Reference == "/one/2a");
        }
    }
}
