using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xigadee;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Xigadee
{
    [TestClass]
    public class TcpTlsConnectionTests
    {
        [Ignore]
        [TestMethod]
        public void Connector1()
        {
            var payload = TransmissionPayload.Create<CommunicationBridgeReroute.IContractFinal>();

            bool stop = false;

            var server = new TcpTlsServer(new IPEndPoint(IPAddress.Any, 9090), SslProtocols.None, null);
            var client = new TcpTlsClient(new IPEndPoint(IPAddress.Loopback, 9090), SslProtocols.None, null);

            server.Start();

            Thread toRun = new Thread(new ThreadStart(() => 
            {
                while (!stop)
                {
                    if (server.PollRequired)
                        Task.Run(async () => await server.Poll());

                    if (client.PollRequired)
                        Task.Run(async () => await client.Poll());

                    Thread.Sleep(10);
                }
            }));

            toRun.Start();

            client.Start();

            client.Write(payload).Wait();


            server.Poll().Wait();

            

            stop = true;
            if (toRun.IsAlive)
                toRun.Abort();
        }
    }
}
