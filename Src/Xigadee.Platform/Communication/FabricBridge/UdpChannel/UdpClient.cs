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
    /// <summary>
    /// This is the Udp client.
    /// </summary>
    public class UdpClient
    {
        public UdpClient()
        {
            mClient = new System.Net.Sockets.UdpClient();
        }

        private System.Net.Sockets.UdpClient mClient;

        public bool IsBroadcast { get; set; }

        public IPEndPoint EndPoint { get; set; }
    }
}
