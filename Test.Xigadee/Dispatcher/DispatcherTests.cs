using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// These unit tests validate the Dispatcher logic.
    /// </summary>
    [TestClass]
    public class DispatcherTests: DispatcherTestsBase<DispatcherCommand>
    {
        [TestInitialize]
        public void TearUp()
        {
            try
            {
                mPipeline = PipelineConfigure(Microservice.Create((s) => service = s, serviceName: GetType().Name));
                mPipeline.Start();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [TestCleanup]
        public void TearDown()
        {
            mPipeline.Stop();
            mPipeline = null;
        }

        [TestMethod]
        public void DispatcherTestSuccess()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            bool success = false;

            var message = new TransmissionPayload(cpipeIn.Channel.Id, "friday", "feeling", release: (e,f) =>
            {
                success = true;
                mre.Set();
            }
            , options: ProcessOptions.RouteInternal);

            mListener.Inject(message);

            mre.WaitOne(2000);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void DispatcherTestCommandSuccess()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            bool success = false;

            var message = new TransmissionPayload(cpipeIn.Channel.Id, "friday", "standard", release: (e, f) =>
            {
                mre.Set();
            }
            , options: ProcessOptions.RouteInternal);

            mDCommand.OnTestCommandReceive += (sender, e) =>
            {
                success = true;
                mre.Set();
            };

            mListener.Inject(message);

            mre.WaitOne(2000);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void DispatcherTestFail()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            bool success = false;

            var message = new TransmissionPayload(cpipeIn.Channel.Id, "friday", "funky", release: (e, f) =>
            {
                mre.Set();
            }
            , options: ProcessOptions.RouteInternal);

            service.ProcessRequestUnresolved += (sender, e) => {
                success = e.Payload.Id == message.Id;
                mre.Set();
            };


            mListener.Inject(message);

            mre.WaitOne(2000);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void DispatcherTestOut()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            bool success = false;

            var message = new TransmissionPayload(cpipeOut.Channel.Id, "friday", "feeling", release: (e, f) =>
            {
                mre.Set();
            }
            , options: ProcessOptions.RouteExternal);

            mSender.OnProcess += (sender, e) =>
            {
                success = e.Id == message.Id;
                mre.Set();
            };

            mListener.Inject(message);

            mre.WaitOne(2000);

            Assert.IsTrue(success);
        }


    }
}
