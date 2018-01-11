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
    /// This class holds the Socket state when receiving and transmitting information.
    /// </summary>
    public class UdpHelperState
    {
        #region Declarations
        /// <summary>
        /// The maximum permitted size for a UDP message body.
        /// </summary>
        public const int UDPMAXSIZE = 65507;

        private HashSet<IPAddress> mIPAddressExclude; 
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpHelperState"/> class.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="socket">The socket.</param>
        public UdpHelperState(UdpHelperMode mode, Socket socket)
        {
            Mode = mode;
            Buffer = Mode != UdpHelperMode.Sender ? (new byte[UDPMAXSIZE]) : null;
            Socket = socket;
            mIPAddressExclude = new HashSet<IPAddress>();
        } 
        #endregion
        /// <summary>
        /// Gets the socket.
        /// </summary>
        public Socket Socket { get; }
        /// <summary>
        /// Gets the context mode.
        /// </summary>
        public UdpHelperMode Mode { get; }
        /// <summary>
        /// Gets the buffer.
        /// </summary>
        public byte[] Buffer { get; }

        #region TransmitOk(IPAddress address)
        /// <summary>
        /// Checks that an address is OK to transmit on the socket interface.
        /// </summary>
        /// <param name="address">The address to check.</param>
        /// <returns>Returns true if OK to transmit.</returns>
        public bool TransmitOk(IPAddress address)
        {
            return !mIPAddressExclude.Contains(address);
        }
        #endregion
        #region TransmitBlock(IPAddress address)
        /// <summary>
        /// Registers an address as blocked for transmission. This usually happens when there is no route to the address on a previous transmit attempt.
        /// </summary>
        /// <param name="address">The address.</param>
        public void TransmitBlock(IPAddress address)
        {
            if (!mIPAddressExclude.Contains(address))
                mIPAddressExclude.Add(address);
        }
        #endregion

        /// <summary>
        /// Sets the async function when receiving data on the socket.
        /// </summary>
        /// <param name="ar">The callback.</param>
        public void Receive(AsyncCallback ar)
        {
            EndPoint RemoteEndpoint = new IPEndPoint(0, 0);

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
        /// <summary>
        /// The maximum packet size for a UDP packet. 
        /// </summary>
        public const int PacketMaxSize = 508;

        #region Declarations
        Dictionary<IPEndPoint, UdpHelperState> mConnectionsListener, mConnectionsSender;
        ConcurrentQueue<Message> mIncomingPending;
        #endregion
        #region Constructor
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
        #endregion

        /// <summary>
        /// Gets the UDP configuration.
        /// </summary>
        public UdpConfig Config { get; }
        /// <summary>
        /// Gets the mode.
        /// </summary>
        public UdpHelperMode Mode { get; }

        /// <summary>
        /// Gets the available data.
        /// </summary>
        public bool Available => mIncomingPending.Count > 0;
        /// <summary>
        /// Gets the count of the pending incoming messages.
        /// </summary>
        public int Count => mIncomingPending.Count;

        #region TryDequeue(out Message message)
        /// <summary>
        /// Tries to dequeue a message from the incoming queue.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Returns true if a message has been successfully dequeued.</returns>
        public bool TryDequeue(out Message message)
        {
            message = null;

            return mIncomingPending.TryDequeue(out message);
        }
        #endregion

        #region StartInternal()
        /// <summary>
        /// This method starts the service. 
        /// </summary>
        protected override void StartInternal()
        {
            if (Mode == UdpHelperMode.Listener || Mode == UdpHelperMode.Bidirectional)
                switch (Config.Mode)
                {
                    case UdpMode.Unicast:
                        Config.Addresses.ForEach((a) => ListenerUnicastAdd(a, Config.Port));
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
                        Config.Addresses.ForEach((a) => SenderUnicastAdd(a, Config.Port, Config.RemoteEndPoint));
                        break;
                    case UdpMode.Broadcast:
                        Config.Addresses.ForEach((a) => SenderBroadcastAdd(a, Config.Port, Config.RemoteEndPoint));
                        break;
                    case UdpMode.Multicast:
                        throw new NotImplementedException();
                        break;
                }

        }
        #endregion
        #region StopInternal()
        /// <summary>
        /// This method stops the service. 
        /// </summary>
        protected override void StopInternal()
        {
            mConnectionsListener.ForEach((k) => k.Value.Socket.Close());
            mConnectionsListener.Clear();
            mConnectionsSender.ForEach((k) => k.Value.Socket.Close(1));
            mConnectionsSender.Clear();
        }
        #endregion

        #region SenderUnicastAdd(IPAddress address, int port, IPEndPoint destination)
        /// <summary>
        /// Adds a Udp sender with the specific destination.
        /// </summary>
        /// <param name="address">The local address.</param>
        /// <param name="port">The local port.</param>
        /// <param name="destination">The destination address and port.</param>
        protected virtual void SenderUnicastAdd(IPAddress address, int port, IPEndPoint destination)
        {
            var ep = new IPEndPoint(address, port);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ExclusiveAddressUse = Config.ExclusiveAddressUse;

            socket.Bind(ep);

            var state = new UdpHelperState(UdpHelperMode.Sender, socket);

            mConnectionsSender.Add(ep, state);
        }
        #endregion

        #region SenderBroadcastAdd(IPAddress address, int port, IPEndPoint destination)
        /// <summary>
        /// Adds a Udp sender with the specific destination.
        /// </summary>
        /// <param name="address">The local address.</param>
        /// <param name="port">The local port.</param>
        /// <param name="destination">The destination address and port.</param>
        protected virtual void SenderBroadcastAdd(IPAddress address, int port, IPEndPoint destination)
        {
            var ep = new IPEndPoint(address, port);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ExclusiveAddressUse = Config.ExclusiveAddressUse;

            // set the Don't Fragment flag.
            socket.DontFragment = true;
            // Enable broadcast.
            socket.EnableBroadcast = true;
            // Disable multicast loop-back.
            socket.MulticastLoopback = false;
            socket.Blocking = true;
            socket.Bind(ep);

            var state = new UdpHelperState(UdpHelperMode.Sender, socket);

            mConnectionsSender.Add(ep, state);
        }
        #endregion

        #region ListenerUnicastAdd(IPAddress address, int port)
        /// <summary>
        /// Adds a specific UDP listener for the address and port.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        protected virtual void ListenerUnicastAdd(IPAddress address, int port)
        {
            var ep = new IPEndPoint(address, port);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ExclusiveAddressUse = Config.ExclusiveAddressUse;

            socket.Bind(ep);

            var state = new UdpHelperState(UdpHelperMode.Listener, socket);

            mConnectionsListener.Add(ep, state);

            state.Receive(new AsyncCallback(ListenerUnicastEndReceiveFrom));
        }

        private void ListenerUnicastEndReceiveFrom(IAsyncResult ar)
        {
            try
            {
                EndPoint ep = new IPEndPoint(0, 0);
                var state = (UdpHelperState)ar.AsyncState;
                Socket socket = state.Socket;
                int read = socket.EndReceiveFrom(ar, ref ep);

                if (read > 0)
                {
                    var message = new Message() { RemoteEndPoint = (IPEndPoint)ep };
                    message.Buffer = new byte[read];
                    Buffer.BlockCopy(state.Buffer, 0, message.Buffer, 0, read);

                    mIncomingPending.Enqueue(message);

                    state.Receive(new AsyncCallback(ListenerUnicastEndReceiveFrom));
                }
            }
            catch (ObjectDisposedException odex)
            {
                //This is throw when the socket is closed and the Async wait is completed.
                //We should just ignore this.
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        #endregion

        //public void SenderAdd(IPEndPoint ep)
        //{
        //    if (mConnectionsListener.ContainsKey(ep))
        //        throw new ArgumentOutOfRangeException("ep", $"The EndPoint {ep.Address.ToString()}:{ep.Port} is already registered.");

        //    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ExclusiveAddressUse, false);

        //    //if (MulticastTtl.HasValue)
        //    //    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, MulticastTtl);

        //    //Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.
        //    //Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));
        //    //Socket.Bind(
        //}


        #region Send(byte[] blob, int length)
        /// <summary>
        /// Sends the specified BLOB to the configuration remote endpoint.
        /// </summary>
        /// <param name="blob">The BLOB.</param>
        /// <param name="length">The length.</param>
        /// <returns>Returns true if the message transmitted successfully on at least one interface.</returns>
        public bool Send(byte[] blob, int length)
        {
            return SendTo(blob, length, Config.RemoteEndPoint);
        } 
        #endregion
        #region SendTo(byte[] blob, int length, IPEndPoint remoteEndPoint)
        /// <summary>
        /// Sends the specified BLOB to the specified endpoint.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <param name="length">The byte length.</param>
        /// <param name="remoteEndPoint">The specified remote IP endpoint.</param>
        /// <returns>Returns true if the message transmitted successfully on at least one interface.</returns>
        public bool SendTo(byte[] blob, int length, IPEndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
                throw new ArgumentNullException("remoteEndPoint");

            bool success = false;
            mConnectionsSender.Values.ForEach((s) =>
            {
                try
                {
                    if (s.TransmitOk(remoteEndPoint.Address))
                    {
                        int b = s.Socket.SendTo(blob, length, SocketFlags.None, remoteEndPoint);
                        success = true;
                    }
                }
                catch (SocketException sex)
                {
                    if (sex.SocketErrorCode == SocketError.NetworkUnreachable)
                        s.TransmitBlock(remoteEndPoint.Address);
                    else
                        throw sex;
                }
                catch (Exception ex)
                {

                }
            });
            return success;
        }
        #endregion

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
