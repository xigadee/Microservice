using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System;
using System.Threading;

namespace Test.Xigadee
{
    [TestClass]
    public class HarnessOutgoing
    {
        #region Command: OutgoingCommandRoot
        /// <summary>
        /// This is the root class.
        /// </summary>
        /// <seealso cref="Xigadee.CommandBase" />
        class OutgoingCommandRoot : CommandBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MasterCommandRoot"/> class.
            /// </summary>
            /// <param name="policy">The optional command policy. If this is null, then the policy will be created.</param>
            public OutgoingCommandRoot(CommandPolicy policy = null) : base(policy) { }
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
            /// The 1base schedule.
            /// </summary>
            /// <returns>Async.</returns>
            [JobSchedule("1base")]
            protected async Task Schedule1()
            {
                var back = await Outgoing.Process<string, string>(("one", "two", "four"), "Hello");
            }
        }
        #endregion

        [TestMethod]
        public void FailNoResponseChannel()
        {
            var policy = new CommandPolicy() { ChannelId = "default", OutgoingRequestsEnabled = true};

            var harness = new CommandHarness<OutgoingCommandRoot>(policy);

            try
            {
                harness.Start();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex is CommandStartupException);
            }

        }

        [TestMethod]
        public void OutgoingTests()
        {
            var policy = new CommandPolicy()
            {
                  ChannelId = "default"
                , JobSchedulesEnabled = true
                , OutgoingRequestMaxProcessingTimeDefault = TimeSpan.FromSeconds(1)
                , OutgoingRequestDefaultTimespan = TimeSpan.FromSeconds(1)
                , OutgoingRequestsEnabled = true
                , ResponseChannelId = "incoming"
            };

            var harness = new CommandHarness<OutgoingCommandRoot>(policy);
            bool timed_out = false;
            var mre = new ManualResetEventSlim();

            harness.Service.OnOutgoingRequest += (object sender, OutgoingRequestEventArgs e) =>
            {

            };

            harness.Service.OnOutgoingRequestTimeout += (object sender, OutgoingRequestEventArgs e) =>
            {
                timed_out = true;
                mre.Set();
            };

            harness.Start();

            //Create the timeout poll
            Task.Run(async () =>
            {
                while (!timed_out)
                {
                    harness.OutgoingTimeoutScheduleExecute();
                    await Task.Delay(100);
                }
            });

            harness.ScheduleExecute("1base");

            mre.Wait();

            Assert.IsTrue(timed_out);
        }

    }
}
