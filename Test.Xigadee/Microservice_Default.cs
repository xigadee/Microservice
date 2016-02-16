using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class Microservice_Default: MicroService_Setup
    {

        [TestMethod]
        public void UnhandledMessageCheck()
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

        [TestMethod]
        public void GoodMessageCheck()
        {
            ManualResetEvent reset = new ManualResetEvent(false);

            bool isSuccess = false;

            var del = new EventHandler<Tuple<TransmissionPayload, List<TransmissionPayload>>>((sender, e) =>
            {
                isSuccess = true;
                reset.Set();
            });

            mCommand.OnExecute += del;

            mService.Process<IDoSomething>(options: ProcessOptions.RouteInternal);
            reset.WaitOne();

            Assert.IsTrue(isSuccess);

            mCommand.OnExecute -= del;
        }
    }

    [Contract("MyChannel", "Do", "Something")]
    public interface IDoSomething: IMessageContract
    {

    }
}
