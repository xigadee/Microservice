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
        #region Declarations
        IPipeline mPipeline = null;

        ManualFabricBridge fabric = null;
        ManualChannelListener mListener = null;
        ManualChannelSender mSender = null;
        DispatcherCommand mDCommand = null;



        const string cnChannelIn = "internalIn";
        const string cnChannelOut = "internalOut";
        #endregion
        #region TearUp/TearDown
        [TestInitialize]
        public void TearUp()
        {
            fabric = new ManualFabricBridge();
            var bridgeOut = fabric[FabricMode.Queue];
            var bridgein = fabric[FabricMode.Broadcast];
            mListener = (ManualChannelListener)bridgein.GetListener();
            mSender = (ManualChannelSender)bridgeOut.GetSender();

            try
            {
                mPipeline = new MicroservicePipeline(GetType().Name)
                    .AddChannelIncoming(cnChannelIn)
                        .AttachListener(mListener)
                        .AttachCommand(new DispatcherCommand(), assign: (c) => mDCommand = c)
                        .Revert()
                    .AddChannelOutgoing(cnChannelOut)
                        .AttachSender(mSender)
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
        #endregion
        #region Wait(ManualResetEvent mre)
        private void Wait(ManualResetEvent mre)
        {
#if (DEBUG)
            mre.WaitOne();
#else 
            mre.WaitOne(2000);
#endif
        } 
        #endregion

        /// <summary>
        /// This test 
        /// </summary>
        [TestMethod]
        public void DispatcherTestSuccess()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            bool success = false;

            var message = new TransmissionPayload(cnChannelIn, "friday", "feeling"
                , release: (e,f) =>
                {
                    success = true;
                    mre.Set();
                }
                , options: ProcessOptions.RouteInternal
                
            );

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

                var message = new TransmissionPayload(cnChannelIn, "friday", "exception"
                    , release: (e, f) =>
                    {
                        mre.Set();
                    }
                    , options: ProcessOptions.RouteInternal
                );

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

                var message = new TransmissionPayload(cnChannelIn, "friday", "standard"
                    , release: (e, f) =>
                    {
                        mre.Set();
                    }
                    , options: ProcessOptions.RouteInternal
                );

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

            var message = new TransmissionPayload(cnChannelIn, "friday", "funky"
                , release: (e, f) =>
                {
                    mre.Set();
                }
                , options: ProcessOptions.RouteInternal
            );

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

            var message = new TransmissionPayload(cnChannelOut, "another", "monday"
                , release: (e, f) =>
                {
                    //mre.Set();
                }
                , options: ProcessOptions.RouteExternal
            );

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
