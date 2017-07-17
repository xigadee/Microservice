using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This harness is used to test the TcpTls channel listener.
    /// </summary>
    public class TcpTlsChannelListenerHarness : ListenerHarness<TcpTlsChannelListener>
    {
        protected override TcpTlsChannelListener Create()
        {
            return new TcpTlsChannelListener();
        }

        protected override void Configure(TcpTlsChannelListener service)
        {
            base.Configure(service);
            service.EndPoint = Endpoint;
            service.SslProtocolLevel = SslProtocolLevel;
        }

        public virtual IPEndPoint Endpoint => new IPEndPoint(IPAddress.Loopback, 8088);

        public virtual SslProtocols SslProtocolLevel => SslProtocols.None;

        public override void Start()
        {
            base.Start();

            var wrapper = new MessageFilterWrapper(new ServiceMessageHeader(Service.ChannelId, "one", "two"));

            Service.Update(new[] { wrapper }.ToList());
        }
    }
}
