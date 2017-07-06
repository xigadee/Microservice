using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class CommandInitiatorTest1
    {
        //[Ignore]
        [TestMethod]
        public void TimeOut1()
        {
            try
            {
                var pipeOut = new MicroservicePipeline("out");
                CommandInitiator init;

                pipeOut
                    .AddCommandInitiator(out init)
                    .AddChannelIncoming((c) => "return")
                    .Start();

                var rs = init.Process<string,string>("F1","F2","F3", "hello"
                    , new RequestSettings() { WaitTime = TimeSpan.FromSeconds(5) }).Result;

                Assert.IsTrue(rs?.ResponseCode == 408);
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
    }
}
