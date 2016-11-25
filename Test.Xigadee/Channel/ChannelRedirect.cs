using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [TestClass]
    public class ChannelRewrite
    {
        [TestMethod]
        public void Rewrite1()
        {
            var ms = new MicroservicePipeline();
            CommandInitiator init;
            ms
                .AddChannelIncoming("deadletter")
                .Revert()
                .AddChannelIncoming("freddy")
                    .AttachCommand<RewriteCommandVerifyFail>()
                    .AttachMessageRedirectRule(
                        new ServiceMessageHeader("freddy", "one", "two")
                       ,new ServiceMessageHeader("findlay", "three", "four")
                       )
                .Revert()
                .AddChannelIncoming("findlay")
                    .AttachCommand<RewriteCommandVerifySuccess>()
                .Revert()
                .AddChannelOutgoing("response")
                    
                ;
            
            ms.Start();
        }
    }

    public class RewriteCommandVerifySuccess: CommandBase
    {

        [CommandContract("findlay", "three", "four")]
        public string Verify(string inData)
        {
            return "fail";
        }
    }
    public class RewriteCommandVerifyFail: CommandBase
    {

        [CommandContract("freddy", "one", "two")]
        public string Verify(string inData)
        {
            return "pass";
        }
    }
}
