using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Commands
{
    [TestClass]
    public class CommandRerouteTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var payload = TransmissionPayload.Create();

            var rs = payload.ToResponse();
        }
    }
}
