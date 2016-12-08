using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee.Pipeline
{
    public interface IHello
    {
        void DoSomething();

    }

    public class CommandHello: CommandBase, IHello
    {
        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class PipelineUnityWebApi
    {
        [TestMethod]
        public void TestMethod1()
        {
            var pipe = new UnityWebApiMicroservicePipeline();

            pipe
                .AddChannelIncoming("freddyin", autosetPartition01: false)
                    .AttachPriorityPartition(0, 1, 2)
                    .AttachCommandUnity(typeof(IHello),new CommandHello())
                .Revert()
                .AddChannelOutgoing("freddyout", autosetPartition01: false)
                    .AttachPriorityPartition(1, 2)
                ;

            Assert.IsNotNull(pipe.Unity.Resolve(typeof(IHello)));

            pipe.Start();

            pipe.Stop();
        }
    }
}
