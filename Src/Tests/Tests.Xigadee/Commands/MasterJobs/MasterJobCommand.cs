using System;
using System.Threading;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This is the master job test command.
    /// </summary>
    /// <seealso cref="Xigadee.CommandBase" />
    public class TestMasterJobCommand: TestMasterJobCommandRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestMasterJobCommand"/> class.
        /// </summary>
        public TestMasterJobCommand() : base()
        { }




        /// <summary>
        /// Test schedule 1.
        /// </summary>
        /// <param name="incoming">The incoming schedule.</param>
        [JobSchedule("Mikey", "00:00:00", "00:00:05")]
        public async void JobSchedule2(Schedule incoming, CancellationToken cancel)
        {
            //Hello

        }
    }

    public class TestMasterJobCommandRoot: CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestMasterJobCommandRoot"/> class.
        /// </summary>
        public TestMasterJobCommandRoot() : base(
            new CommandPolicy
            {
                MasterJobEnabled = true
                , MasterJobNegotiationStrategy = new MasterJobNegotiationStrategyDebug()
                //, MasterJobNegotiationStrategy = new MasterJobNegotiationStrategy()
                , MasterJobName = "freddy"
                , TransmissionPayloadTraceEnabled = true
            }
            )
        { }

        /// <summary>
        /// Test schedule 1.
        /// </summary>
        /// <param name="incoming">The incoming schedule.</param>
        [MasterJobSchedule("Freedy Fingered")]
        private Task MJSchedule1(CancellationToken cancel, Schedule incoming)//, CancellationToken cancel)
        {
            //Hello
            return Task.FromResult(0);
        }
    }
}
