using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This class checks the inheritance behaviour for the attributes and policy for the command.
    /// </summary>
    [TestClass]
    public class CommandHarnessMasterJob
    {
        #region Command: MasterJobCommandRoot
        /// <summary>
        /// This is the root class.
        /// </summary>
        /// <seealso cref="Xigadee.CommandBase" />
        class MasterJobCommandRoot : CommandBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MasterCommandRoot"/> class.
            /// </summary>
            /// <param name="policy">The optional command policy. If this is null, then the policy will be created.</param>
            public MasterJobCommandRoot(CommandPolicy policy = null) : base(policy) { }
            /// <summary>
            /// Command1s the specified in parameter.
            /// </summary>
            /// <param name="inParam">The in parameter.</param>
            /// <param name="inPayload">The in payload.</param>
            /// <param name="outPayload">The out payload.</param>
            /// <returns>A return string.</returns>
            [MasterJobCommandContract("one", "base")]
            [return: PayloadOut]
            protected async Task<string> Command1([PayloadIn]string inParam, TransmissionPayload inPayload, List<TransmissionPayload> outPayload)
            {
                var back = await Outgoing.Process<string, string>(("one", "two", "three"), "Hello");
                return back.Response;
            }
            /// <summary>
            /// The 1base schedule.
            /// </summary>
            /// <returns>Async.</returns>
            [MasterJobSchedule("1base")]
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
        class MasterJobCommandTop : MasterJobCommandRoot
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MasterCommandTop"/> class.
            /// </summary>
            /// <param name="policy">The optional command policy. If this is null, then the policy will be created.</param>
            public MasterJobCommandTop(CommandPolicy policy = null) : base(policy) { }

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

        //[Ignore]
        [TestMethod]
        public void TestMasterJob()
        {
            var policy = new CommandPolicy(){ ChannelId = "default", JobSchedulesEnabled = true, CommandContractAttributeInherit = true, JobScheduleAttributeInherit = true };
            //Default state.
            var harness = new CommandHarness<MasterJobCommandTop>(policy);

            harness.MasterJobNegotiationEnable();

            harness.Start();

            //Wait for the master job to go live.
            harness.MasterJobStart();

            Assert.IsTrue(harness.RegisteredSchedules.Count == 2);
            Assert.IsTrue(harness.Dependencies.Scheduler.Count == 3);
            Assert.IsTrue(harness.RegisteredCommandMethods.Count == 3);

            Assert.IsTrue(harness.HasCommand(("one", "top")));
            Assert.IsTrue(harness.HasSchedule("1top"));

            Assert.IsTrue(harness.HasCommand(("one", "base")));
            Assert.IsTrue(harness.HasSchedule("1base"));

            harness.MasterJobStop();

            Assert.IsFalse(harness.HasCommand(("one", "base")));
            Assert.IsFalse(harness.HasSchedule("1base"));


        }

    }
}
