using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Xigadee
{
    public class UdpHelperStatistics: StatusBase
    {

    }

    /// <summary>
    /// This enumeration specifies how the helper processes messages, i.e. incoming, outgoing or both.
    /// </summary>
    public enum UdpHelperMode
    {
        /// <summary>
        /// The helper is in listener mode.
        /// </summary>
        Listener,
        /// <summary>
        /// The helper is in sender mode.
        /// </summary>
        Sender,
        /// <summary>
        /// The helper is bidirectional
        /// </summary>
        Bidirectional
    }

    /// <summary>
    /// This is the Udp Helper class.
    /// </summary>
    public class UdpHelper:ServiceBase<UdpHelperStatistics>
    {
        Dictionary<IPEndPoint, Socket> mConnectionsListener, mConnectionsSender;
        ConcurrentQueue<Message> mIncomingPending;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpHelper"/> class.
        /// </summary>
        public UdpHelper(UdpConfig udp, UdpHelperMode mode)
        {
            Config = udp;
            Mode = mode;

            mConnectionsListener = new Dictionary<IPEndPoint, Socket>();
            mConnectionsSender = new Dictionary<IPEndPoint, Socket>();

            mIncomingPending = new ConcurrentQueue<Message>();
        }

        /// <summary>
        /// This method starts the service. 
        /// </summary>
        protected override void StartInternal()
        {
            if (Mode == UdpHelperMode.Listener || Mode == UdpHelperMode.Bidirectional)
                switch (Config.Mode)
                {
                    case UdpMode.Unicast:
                        Config.Addresses.ForEach((a) => ListenerAddUnicast(a, Config.Port));
                        break;
                    case UdpMode.Broadcast:
                        throw new NotImplementedException();
                        break;
                    case UdpMode.Multicast:
                        throw new NotImplementedException();
                        break;
                }
        }

        /// <summary>
        /// This method stops the service. 
        /// </summary>
        protected override void StopInternal()
        {
            mConnectionsListener.ForEach((k) => k.Value.Close());
            mConnectionsSender.ForEach((k) => k.Value.Close(1));
        }

        /// <summary>
        /// Gets the UDP configuration.
        /// </summary>
        public UdpConfig Config { get; }
        /// <summary>
        /// Gets the mode.
        /// </summary>
        public UdpHelperMode Mode { get; }


        #region Available
        /// <summary>
        /// Gets the available data.
        /// </summary>
        public bool Available
        {
            get
            {
                return (mIncomingPending.Count > 0)
                    || (mConnectionsListener.Values.Select((s) => s.Available > 0).FirstOrDefault());
            }
        } 
        #endregion

        protected virtual void ListenerAddUnicast(IPAddress address, int port)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ExclusiveAddressUse, false);

            var cb = new AsyncCallback(ListenReceive);

            mConnectionsListener.Add(new IPEndPoint(address, port), socket);
        }

        private void ListenReceive(IAsyncResult result)
        {
            
        }

        public void SenderAdd(IPEndPoint ep)
        {
            if (mConnectionsListener.ContainsKey(ep))
                throw new ArgumentOutOfRangeException("ep", $"The EndPoint {ep.Address.ToString()}:{ep.Port} is already registered.");

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ExclusiveAddressUse, false);

            //if (MulticastTtl.HasValue)
            //    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, MulticastTtl);

            //Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.
            //Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
            //Socket.Bind(
        }


        public async Task<Message> ReceiveAsync()
        {
            //var e = new SocketAsyncEventArgs();
            //e.Completed += ReceiveCompleted;
            //Socket.ReceiveAsync(e);

            //Socket.
            //var success = await Socket.ReceiveAsync(e);

            return null;
        }

        private void ReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SendAsync(byte[] blob, int length, IPEndPoint ep = null)
        {
            return 0;
        }

        void UdpTransmitHelper(string mcastGroup, int port, int ttl, int rep)
        {
            IPAddress ip;
            try
            {
                Console.WriteLine("MCAST Send on Group: {0} Port: {1} TTL: {2}", mcastGroup, port, ttl);
                ip = IPAddress.Parse(mcastGroup);

                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip));
                s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, ttl);

                byte[] b = new byte[10];
                for (int x = 0; x < b.Length; x++) b[x] = (byte)(x + 65);

                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(mcastGroup), port);

                Console.WriteLine("Connecting...");

                s.Connect(ipep);

                for (int x = 0; x < rep; x++)
                {
                    Console.WriteLine("Sending ABCDEFGHIJ...");
                    s.Send(b, b.Length, SocketFlags.None);
                }

                Console.WriteLine("Closing Connection...");
                s.Close();
            }
            catch (System.Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        void UdpReceiveHelper(string mcastGroup, int port)
        {
            IPAddress ip = IPAddress.Parse(mcastGroup);

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            s.Bind(ipep);

            while (true)
            {
                byte[] b = new byte[10];
                Console.WriteLine("Waiting for data..");
                s.Receive(b);
                string str = System.Text.Encoding.ASCII.GetString(b, 0, b.Length);
                Console.WriteLine("RX: " + str.Trim());
            }
            //s.Close();
        }


        /// <summary>
        /// This class holds the incoming UDP binary data. This is used when the deserializers cannot pick up the message type.
        /// </summary>
        public class Message
        {
            /// <summary>
            /// Gets or sets the binary payload.
            /// </summary>
            public byte[] Buffer { get; set; }
            /// <summary>
            /// Gets or sets the endpoint.
            /// </summary>
            public IPEndPoint RemoteEndPoint { get; set; }
        }
    }
}
