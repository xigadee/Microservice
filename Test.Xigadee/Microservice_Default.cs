using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class Microservice_Default: TestPopulator<TestMicroservice, TestConfig>
    {
        protected EventTestCommand<IDoSomething> mCommand;

        protected override void RegisterCommands()
        {
            base.RegisterCommands();
            mCommand = (EventTestCommand<IDoSomething>)Service.RegisterCommand(new EventTestCommand<IDoSomething>());
        }


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

            Service.ProcessRequestUnresolved += del;

            Service.Process("Unknown", options: ProcessOptions.RouteInternal);
            reset.WaitOne();

            Assert.IsTrue(isFaulted);

            Service.ProcessRequestUnresolved -= del;
        }

        [TestMethod]
        public void GoodMessageCheck()
        {
            try
            {
                ManualResetEvent reset = new ManualResetEvent(false);

                bool isSuccess = false;

                var del = new EventHandler<Tuple<TransmissionPayload, List<TransmissionPayload>>>((sender, e) =>
                {
                    isSuccess = true;
                    reset.Set();
                });

                mCommand.OnExecute += del;

                Service.Process<IDoSomething>(options: ProcessOptions.RouteInternal);
                reset.WaitOne();

                Assert.IsTrue(isSuccess);

                mCommand.OnExecute -= del;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }

    [Contract("MyChannel", "Do", "Something")]
    public interface IDoSomething: IMessageContract
    {

    }
}
