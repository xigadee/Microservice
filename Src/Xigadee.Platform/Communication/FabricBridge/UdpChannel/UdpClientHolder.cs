using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Xigadee
{

    public class UdpClientHolder : ClientHolder<UdpClient, ServiceMessage>
    {
        public override Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            throw new NotImplementedException();
        }

        public override Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            throw new NotImplementedException();
        }
    }
}
