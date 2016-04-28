using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class DoNothingJob: CommandBase
    {
        public DoNothingJob() 
        {
        }

        protected override void CommandsRegister()
        {
            base.CommandsRegister();
        }

        //override Policy
    }
}
