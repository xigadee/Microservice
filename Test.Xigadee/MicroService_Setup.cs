using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{

    public abstract class MicroService_Setup
    {
        protected MicroserviceBase mService;
        protected EventTestCommand<IDoSomething> mCommand;

        [TestInitialize]
        public void Initialise()
        {
            mService = new MicroserviceBase();
            mCommand = (EventTestCommand<IDoSomething>)mService.RegisterCommand(new EventTestCommand<IDoSomething>());
            mService.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            mService.Stop();
        }

    }



}
