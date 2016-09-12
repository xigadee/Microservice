#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [Contract("MyChannel", "Do", "Something1")]
    public interface IDoSomething1: IMessageContract{}

    [Contract("MyChannel", "Do", "Something2")]
    public interface IDoSomething2: IMessageContract{}

    [TestClass]
    public class Microservice_Validate_CommandOutgoingRequests: TestPopulator<TestMicroservice, TestConfig>
    {
        protected EventTestCommand<IDoSomething1> mCommand1;
        protected EventTestCommand<IDoSomething2> mCommand2;

        protected override void RegisterCommands()
        {
            base.RegisterCommands();
            mCommand1 = (EventTestCommand<IDoSomething1>)Service.RegisterCommand(new EventTestCommand<IDoSomething1>());
            mCommand2 = (EventTestCommand<IDoSomething2>)Service.RegisterCommand(new EventTestCommand<IDoSomething2>());
        }


        [TestMethod]
        public void UnhandledRequestCheck()
        {
            try
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
                reset.WaitOne(5000);

                Assert.IsTrue(isFaulted);

                Service.ProcessRequestUnresolved -= del;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void VerifyCommandCount()
        {
            Assert.AreEqual(Service.Commands.Count(),2);
        }

        [TestMethod]
        public void IDoSomething1CommandCheck()
        {
            bool isSuccess = false;

            try
            {
                ManualResetEvent reset = new ManualResetEvent(false);

                var del = new EventHandler<Tuple<TransmissionPayload, List<TransmissionPayload>>>((sender, e) =>
                {
                    isSuccess = true;
                    reset.Set();
                });

                mCommand1.OnExecute += del;

                Service.Process<IDoSomething1>(options: ProcessOptions.RouteInternal);
                reset.WaitOne(5000);

                mCommand1.OnExecute -= del;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(isSuccess);
        }

        [TestMethod]
        public void IDoSomething2CommandCheck()
        {
            bool isSuccess = false;

            try
            {
                ManualResetEvent reset = new ManualResetEvent(false);

                var del = new EventHandler<Tuple<TransmissionPayload, List<TransmissionPayload>>>((sender, e) =>
                {
                    isSuccess = true;
                    reset.Set();
                });

                mCommand2.OnExecute += del;

                Service.Process<IDoSomething2>(options: ProcessOptions.RouteInternal);
                reset.WaitOne(5000);

                mCommand2.OnExecute -= del;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(isSuccess);
        }

        /// <summary>
        /// This method checks that the commands are matched using a case-insensitive matching algorithm.
        /// </summary>
        [TestMethod]
        public void IDoSomething2CommandCheckCaseInsensitive()
        {
            bool isSuccess = false;

            try
            {
                ManualResetEvent reset = new ManualResetEvent(false);

                var del = new EventHandler<Tuple<TransmissionPayload, List<TransmissionPayload>>>((sender, e) =>
                {
                    isSuccess = true;
                    reset.Set();
                });

                mCommand2.OnExecute += del;

                Service.Process("MYChannel", "dO", "something2", options: ProcessOptions.RouteInternal);
                reset.WaitOne(5000);

                mCommand2.OnExecute -= del;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.IsTrue(isSuccess);
        }
    }
}
