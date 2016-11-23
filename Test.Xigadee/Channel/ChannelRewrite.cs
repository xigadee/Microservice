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

            ms
                .AddChannelIncoming("freddy")
                    .AttachMessageRedirectRule(MessageRedirectRule.DoNothing)
                .Revert()
                .AddChannelIncoming("findlay")
                .Revert();
                //.AttachServiceMessageHeaderRewriteRule(

            ms.Start();
        }
    }

    public class RewriteCommand: CommandBase
    {

        [CommandContract("findlay","one","two")]
        public string Verify(string inData)
        {
            return "hello";
        }
    }
}
