using System;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This is the master job test command.
    /// </summary>
    /// <seealso cref="Xigadee.CommandBase" />
    public class TestMasterJobCommand: CommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestMasterJobCommand"/> class.
        /// </summary>
        public TestMasterJobCommand() : base(
            new CommandPolicy
            {
                  MasterJobEnabled = true
                , MasterJobNegotiationStrategy = new MasterJobNegotiationStrategyDebug()
                , MasterJobName = "freddy"
                , TransmissionPayloadTraceEnabled = true
            }
            )
        { }

        [JobSchedule("00:00:00", "00:00:05")]
        public async Task JobSchedule1(Schedule incoming)
        {
            //Hello

        }

        [MasterJobSchedule("00:00:00","00:00:05",name:"Coolio")]
        public async Task MasterjobSchedule1(Schedule incoming)
        {
            //Hello

        }
    }
}
