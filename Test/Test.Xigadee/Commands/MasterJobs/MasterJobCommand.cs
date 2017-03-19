using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class TestMasterJobCommand:CommandBase
    {
        public event EventHandler<string> OnGoingMaster;

        public TestMasterJobCommand() : base(
            new CommandPolicy
            {
                  MasterJobEnabled = true
                , MasterJobName = "freddy"
                , MasterJobPollFrequency = TimeSpan.FromSeconds(1)
                , MasterJobPollInitialWait = TimeSpan.FromSeconds(1)
            }
            ){}


        protected override void MasterJobStart()
        {
            OnGoingMaster?.Invoke(this, this.OriginatorId.Name);
        }
    }
}
