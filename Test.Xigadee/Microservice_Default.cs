using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class Microservice_Default
    {
        MicroserviceBase mService;

        [TestInitialize]
        public void Initialise()
        {
            mService = new MicroserviceBase();
            mService.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            mService.Stop();
        }

        [TestMethod]
        public void UnhandledMessage()
        {
            ManualResetEvent reset = new ManualResetEvent(false);

            bool isFaulted = false;

            var del = new EventHandler<ProcessRequestUnresolvedEventArgs>((sender, e) =>
            {
                isFaulted = true;
                reset.Set();
            });

            mService.ProcessRequestUnresolved += del;

            mService.Process("Unknown", options: ProcessOptions.RouteInternal);
            reset.WaitOne();

            Assert.IsTrue(isFaulted);

            mService.ProcessRequestUnresolved -= del;
        }

    }
}
