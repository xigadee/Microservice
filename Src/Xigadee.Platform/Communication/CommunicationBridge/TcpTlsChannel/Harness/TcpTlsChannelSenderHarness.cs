using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    public class TcpTlsChannelSenderHarness : SenderHarness<TcpTlsChannelSender>
    {
        protected override TcpTlsChannelSender Create()
        {
            return new TcpTlsChannelSender();
        }

        protected override void Configure(TcpTlsChannelSender service)
        {
            base.Configure(service);
            service.EndPoint = Endpoint;
            //service.SslProtocolLevel = SslProtocolLevel;
        }

        public virtual IPEndPoint Endpoint => new IPEndPoint(IPAddress.Loopback, 8088);

        public virtual SslProtocols SslProtocolLevel => SslProtocols.None;

        public override void Start()
        {
            base.Start();

            var wrapper = new MessageFilterWrapper(new ServiceMessageHeader(Service.ChannelId, "one", "two"));

            //Service.Update(new[] { wrapper }.ToList());

        }
    }
}
