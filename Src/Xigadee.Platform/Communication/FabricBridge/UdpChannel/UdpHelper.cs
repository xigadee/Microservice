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

    public class UdpHelperState
    {
        public const int UDPMAXSIZE = 65507;

        public Socket Socket { get; set; }

        public UdpHelperMode Mode { get; set; }

        public byte[] Buffer { get; set; } = new byte[UDPMAXSIZE];

        public EndPoint RemoteEndpoint;

        public void Receive(AsyncCallback ar)
        {
            RemoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

            Socket.BeginReceiveFrom(Buffer, 0, Buffer.Length, SocketFlags.None
                , ref RemoteEndpoint
                , ar, this);
        }
    }



    /// <summary>
    /// This is the Udp Helper class.
    /// </summary>
    public class UdpHelper:ServiceBase<UdpHelperStatistics>
    {
        Dictionary<IPEndPoint, UdpHelperState> mConnectionsListener, mConnectionsSender;
        ConcurrentQueue<Message> mIncomingPending;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpHelper"/> class.
        /// </summary>
        public UdpHelper(UdpConfig udp, UdpHelperMode mode)
        {
            Config = udp;
            Mode = mode;

            mConnectionsListener = new Dictionary<IPEndPoint, UdpHelperState>();
            mConnectionsSender = new Dictionary<IPEndPoint, UdpHelperState>();

            mIncomingPending = new ConcurrentQueue<Message>();
        }

        public bool TryDequeue(out Message message)
        {
            message = null;

            return mIncomingPending.TryDequeue(out message);
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

            if (Mode == UdpHelperMode.Sender || Mode == UdpHelperMode.Bidirectional)
                switch (Config.Mode)
                {
                    case UdpMode.Unicast:
                        Config.Addresses.ForEach((a) => SenderAddUnicast(a, Config.Port));
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
            mConnectionsListener.ForEach((k) => k.Value.Socket.Close());
            mConnectionsSender.ForEach((k) => k.Value.Socket.Close(1));
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
                    || (mConnectionsListener.Values.Select((s) => s.Socket.Available > 0).FirstOrDefault());
            }
        }
        #endregion

        protected virtual void SenderAddUnicast(IPAddress address, int port)
        {

        }

        protected virtual void ListenerAddUnicast(IPAddress address, int port)
        {
            var ep = new IPEndPoint(address, port);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ExclusiveAddressUse = Config.ExclusiveAddressUse;

            socket.Bind(ep);

            var state = new UdpHelperState { Socket = socket, Mode = UdpHelperMode.Listener };

            mConnectionsListener.Add(ep, state);

            state.Receive(new AsyncCallback(ListenReceive));

        }

        private void ListenReceive(IAsyncResult ar)
        {
            var state = (UdpHelperState)ar.AsyncState;
            Socket socket = state.Socket;
            int read = socket.EndReceive(ar);

            if (read> 0)
                state.Receive(new AsyncCallback(ListenReceive));


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
}
