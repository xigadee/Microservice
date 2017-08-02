using System;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This is the master job test command.
    /// </summary>
    /// <seealso cref="Xigadee.CommandBase" />
    public class TestMasterJobCommand:CommandBase
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
            ){}

    }
}
