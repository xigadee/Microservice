using System.Net;
using Xigadee;

namespace Test.Xigadee
{
    public class MicroserviceClient:MicroserviceBase
    {
        public MicroserviceClient()
        {
            ServicePointManager.DefaultConnectionLimit = 50000;
        }
    }

    public class MicroserviceServer : MicroserviceBase
    {
        public MicroserviceServer()
        {
            ServicePointManager.DefaultConnectionLimit = 50000;
        }
    }

    [Contract("testa", "Quango", "Bongo")]
    public interface ITestADoSomeThing : IMessageContract
    {

    }

    [Contract("testb", "Quango", "Gongo")]
    public interface ITestBDoSomeThing2 : IMessageContract
    {

    }
}
