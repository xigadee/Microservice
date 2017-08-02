using System;
using Xigadee;

namespace Test.Xigadee
{
    public class TestMasterJobCommand:CommandBase
    {
        public TestMasterJobCommand() : base(
            new CommandPolicy
            {
                  MasterJobEnabled = true
                , MasterJobName = "freddy"
                , MasterJobPollFrequency = TimeSpan.FromSeconds(1)
                , MasterJobPollInitialWait = TimeSpan.FromSeconds(1)
                , TransmissionPayloadTraceEnabled = true
                , MasterJobNegotiationStrategy = new MasterJobNegotiationStrategyDebug()
            }
            ){}

    }
}
