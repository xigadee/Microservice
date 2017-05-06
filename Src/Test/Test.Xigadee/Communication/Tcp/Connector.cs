using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Net;

namespace Test.Xigadee
{
    [Ignore]
    [TestClass]
    public class TcpTlsConnectionTests
    {
        [TestMethod]
        public void Connector1()
        {

            var harness = new TcpTlsChannelListenerHarness();

            try
            {
                harness.Start();
            }
            catch (Exception ex)
            {

                throw;
            }


            //connListen.s
        }
    }
}
