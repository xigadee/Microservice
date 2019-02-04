using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This class checks the inheritance behaviour for the attributes and policy for the command.
    /// </summary>
    [TestClass]
    public class CommandHarnessPolicyReflection
    {
        #region Command: CommandRoot
        /// <summary>
        /// This is the root class.
        /// </summary>
        /// <seealso cref="Xigadee.CommandBase" />
        class CommandRoot : CommandBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandRoot"/> class.
            /// </summary>
            /// <param name="policy">The optional command policy. If this is null, then the policy will be created.</param>
            public CommandRoot(CommandPolicy policy = null) : base(policy) { }
            /// <summary>
            /// Command1s the specified in parameter.
            /// </summary>
            /// <param name="inParam">The in parameter.</param>
            /// <param name="inPayload">The in payload.</param>
            /// <param name="outPayload">The out payload.</param>
            /// <returns>A return string.</returns>
            [CommandContract("one", "base")]
            [return: PayloadOut]
            protected async Task<string> Command1([PayloadIn]string inParam, TransmissionPayload inPayload, List<TransmissionPayload> outPayload)
            {
                var back = await Outgoing.Process<string, string>(("one", "two", "three"), "Hello");
                return back.Response;
            }
            /// <summary>
            /// Schedule1s this instance.
            /// </summary>
            /// <returns>Async.</returns>
            [JobSchedule("1base")]
            protected async Task Schedule1()
            {
                var back = await Outgoing.Process<string, string>(("one", "two", "four"), "Hello");
            }
        }
        #endregion
        #region Command: CommandTop <- CommandRoot
        /// <summary>
        /// Inherited command with base functions.
        /// </summary>
        /// <seealso cref="Test.Xigadee.Commands.Harness.HarnessPolicyReflection.CommandRoot" />
        class CommandTop : CommandRoot
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandTop"/> class.
            /// </summary>
            /// <param name="policy">The optional command policy. If this is null, then the policy will be created.</param>
            public CommandTop(CommandPolicy policy = null) : base(policy) { }

            /// <summary>
            /// Schedules the base.
            /// </summary>
            /// <returns>Async.</returns>
            [JobSchedule("1top")]
            private async Task ScheduleBase()
            {
                var back = await Outgoing.Process<string, string>(("one", "two", "four"), "Hello");
            }
            /// <summary>
            /// Commands the base.
            /// </summary>
            /// <param name="inParam">The in parameter.</param>
            /// <param name="inPayload">The in payload.</param>
            /// <param name="outPayload">The out payload.</param>
            /// <returns>The string.</returns>
            [CommandContract("one", "top")]
            [return: PayloadOut]
            private async Task<string> CommandBase([PayloadIn]string inParam, TransmissionPayload inPayload, List<TransmissionPayload> outPayload)
            {
                var back = await Outgoing.Process<string, string>(("one", "two", "three"), "Hello");
                return back.Response;
            }
        } 
        #endregion

        [TestMethod]
        public void TestInheritance()
        {
            var policy = new CommandPolicy()
                { ChannelId = "default", JobSchedulesEnabled = true, CommandContractAttributeInherit = true, JobScheduleAttributeInherit = true };
            //Default state.
            var harness = new CommandHarness<CommandTop>(policy);

            harness.Start();

            Assert.IsTrue(harness.RegisteredSchedules.Count == 2);
            Assert.IsTrue(harness.RegisteredCommandMethods.Count == 2);

            Assert.IsTrue(harness.HasCommand(("one", "top")));
            Assert.IsTrue(harness.HasSchedule("1top"));

            Assert.IsTrue(harness.HasCommand(("one", "base")));
            Assert.IsTrue(harness.HasSchedule("1base"));
        }

        [TestMethod]
        public void TestSchedulesDisabled()
        {
            var policy = new CommandPolicy()
            { ChannelId = "default", JobSchedulesEnabled = false, CommandContractAttributeInherit = true, JobScheduleAttributeInherit = true };
            //Default state.
            var harness = new CommandHarness<CommandTop>(policy);

            harness.Start();

            Assert.IsTrue(harness.RegisteredSchedules.Count == 0);
            Assert.IsTrue(harness.RegisteredCommandMethods.Count == 2);

            Assert.IsTrue(harness.HasCommand(("one", "top")));
            Assert.IsFalse(harness.HasSchedule("1top"));

            Assert.IsTrue(harness.HasCommand(("one", "base")));
            Assert.IsFalse(harness.HasSchedule("1base"));
        }

        [TestMethod]
        public void TestInheritanceDisabled()
        {
            var policy = new CommandPolicy()
                { ChannelId = "default", JobSchedulesEnabled = true, CommandContractAttributeInherit = false, JobScheduleAttributeInherit = false };
            //Default state.
            var harness = new CommandHarness<CommandTop>(policy);

            harness.Start();

            Assert.IsTrue(harness.RegisteredSchedules.Count == 1);
            Assert.IsTrue(harness.RegisteredCommandMethods.Count == 1);

            Assert.IsTrue(harness.HasCommand(("one", "top")));
            Assert.IsTrue(harness.HasSchedule("1top"));

            Assert.IsFalse(harness.HasCommand(("one", "base")));
            Assert.IsFalse(harness.HasSchedule("1base"));

        }
    }
}
