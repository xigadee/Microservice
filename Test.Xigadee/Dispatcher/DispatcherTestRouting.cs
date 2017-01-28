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
    public class DispatcherTestRouting
    {
        IPipeline mPipeline = null;
        ManualChannelListener mListener = null;
        ManualChannelSender mSender = null;
        DispatcherCommand mDCommand = null;

        const string channelIn = "internalIn";
        const string channelOut = "internalOut";

        [TestInitialize]
        public void TearUp()
        {
            try
            {
                mPipeline = new MicroservicePipeline(GetType().Name)
                    .AddChannelIncoming(channelIn)
                        .AttachManualListener(out mListener)
                        .AttachCommand(new DispatcherCommand(), assign: (c) => mDCommand = c)
                        .Revert()
                    .AddChannelOutgoing(channelOut)
                        .AttachManualSender(out mSender)
                        .Revert()
                    ;

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

        private void Wait(ManualResetEvent mre)
        {
#if (DEBUG)
            mre.WaitOne();
#else 
            mre.WaitOne(2000);
#endif
        }

        [TestMethod]
        public void DispatcherTestSuccess()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            bool success = false;

            var message = new TransmissionPayload(channelIn, "friday", "feeling"
                , release: (e,f) =>
                {
                    success = true;
                    mre.Set();
                }
            , options: ProcessOptions.RouteInternal);

            mListener.Inject(message);

            Wait(mre);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void DispatcherTestCommandException()
        {
            try
            {
                ManualResetEvent mre = new ManualResetEvent(false);
                bool success = false;

                var message = new TransmissionPayload(channelIn, "friday", "exception", release: (e, f) =>
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

                Wait(mre);

                Assert.IsFalse(success);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [TestMethod]
        public void DispatcherTestCommandSuccess()
        {
            try
            {
                ManualResetEvent mre = new ManualResetEvent(false);
                bool success = false;

                var message = new TransmissionPayload(channelIn, "friday", "standard", release: (e, f) =>
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

                Wait(mre);

                Assert.IsTrue(success);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [TestMethod]
        public void DispatcherTestFail()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            bool success = false;

            var message = new TransmissionPayload(channelIn, "friday", "funky", release: (e, f) =>
            {
                mre.Set();
            }
            , options: ProcessOptions.RouteInternal);

            mPipeline.Service.Events.ProcessRequestUnresolved += (sender, e) => {
                success = e.Payload.Id == message.Id;
                mre.Set();
            };


            mListener.Inject(message);

            Wait(mre);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void DispatcherTestOut()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            bool success = false;

            var message = new TransmissionPayload(channelOut, "another", "monday", release: (e, f) =>
            {
                //mre.Set();
            }
            , options: ProcessOptions.RouteExternal);

            mSender.OnProcess += (sender, e) =>
            {
                success = e.Id == message.Id;
                mre.Set();
            };

            mListener.Inject(message);

            Wait(mre);

            Assert.IsTrue(success);
        }
    }
}
