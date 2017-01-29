using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class MasterJobCommand:CommandBase
    {
        public MasterJobCommand() : base(new CommandPolicy { MasterJobEnabled = true, MasterJobName = "freddy" })
        {
        }
    }
}
