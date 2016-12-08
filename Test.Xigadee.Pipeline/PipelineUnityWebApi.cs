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

    public class CommandHello: CommandBase<CommandStatistics, CommandInitiatorPolicy>, IHello
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
            try
            {
                var pipe = new UnityWebApiMicroservicePipeline();

                pipe
                    .AddChannelIncoming("freddyin", autosetPartition01: false)
                        .AttachPriorityPartition(0, 1, 2)
                        .AttachCommandUnity(typeof(IHello), new CommandHello())
                    .Revert()
                    .AddChannelOutgoing("freddyout", autosetPartition01: false)
                        .AttachPriorityPartition(1, 2)
                    ;

                var result = pipe.Unity.Resolve(typeof(IHello));
                Assert.IsNotNull(result);
                Assert.IsTrue(((ICommand)result).ResponseChannelId == "freddyin");

                pipe.Start();

                pipe.Stop();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            try
            {
                var pipe = new UnityWebApiMicroservicePipeline();

                pipe
                    .AddChannelIncoming("freddyin", autosetPartition01: false)
                        .AttachPriorityPartition(0, 1, 2)
                        .AttachCommandUnity(typeof(IHello), new CommandHello(), responseChannel: new Channel("freda", ChannelDirection.Incoming))
                    .Revert()
                    .AddChannelOutgoing("freddyout", autosetPartition01: false)
                        .AttachPriorityPartition(1, 2)
                    ;

                var result = pipe.Unity.Resolve(typeof(IHello));
                Assert.IsNotNull(result);
                Assert.IsTrue(((ICommand)result).ResponseChannelId == "freda");

                pipe.Start();

                pipe.Stop();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
