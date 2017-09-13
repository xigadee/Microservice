using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

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
        }

        class CommandTop : CommandRoot
        {
            public CommandTop(CommandPolicy policy = null) : base(policy){}
        }

        [TestMethod]
        public void TestInheritance()
        {
            //Default state.
            var harness1 = new CommandHarness<CommandTop>();

            harness1.Start();
            Assert.IsTrue(harness1.RegisteredSchedules.Count == 0);
            Assert.IsTrue(harness1.RegisteredCommandMethods.Count == 0);
        }
    }
}
