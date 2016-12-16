using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;

namespace Test.Xigadee
{
    [Ignore]
    [TestClass]
    public class InheritedCommandUnitTest: CommandUnitTestBase<InheritedCommand1>
    {
        [TestMethod]
        public void TestStandard1()
        {
            DefaultTest();
        }

        [TestMethod]
        public void PipelineCommand1()
        {
            IPipeline pipeline = null;
            try
            {
                pipeline = Pipeline();

                pipeline.Start();

                int start = Environment.TickCount;

                var result1 = mCommandInit.Process<string, string>("internalIn", "simples1", "async",
                    "bunga", new RequestSettings() { WaitTime = TimeSpan.FromHours(1) }).Result;


                var end = ConversionHelper.DeltaAsTimeSpan(start);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            try
            {
                pipeline?.Stop();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class InheritedCommand1: InheritedClassBase
    {

        [CommandContract(messageType: "simples1", actionType: "async")]
        [return: PayloadOut]
        public override string DoSomething([PayloadIn] string item)
        {
            return base.DoSomething(item);
        }

    }

    public abstract class InheritedClassBase: CommandBase
    {
        protected InheritedClassBase() : base(null) { }


        [return: PayloadOut]
        public virtual string DoSomething([PayloadIn] string item)
        {
            return "momma";
        }
    }
}
