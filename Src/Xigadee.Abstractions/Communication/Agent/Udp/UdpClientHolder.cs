using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace Xigadee
{
    /// <summary>
    /// This class holds the Udp client and associated logic.
    /// </summary>
    public class UdpClientHolder : ClientHolderV2<MessagingServiceStatistics>
    {
        #region Declarations
        readonly Dictionary<IPEndPoint, State> mConnectionsListener;
        readonly Dictionary<IPEndPoint, State> mConnectionsSender;
        readonly ConcurrentQueue<Message> mIncomingPending;
        #endregion
        #region Constructor
        /// <summary>
        /// This is Udp client holder. This class receives and sends the UDP Datagrams.
        /// </summary>
        /// <param name="udp">The Udp configuration.</param>
        /// <param name="mode">The transmission mode.</param>
        public UdpClientHolder(UdpConfig udp
            , CommunicationAgentCapabilities mode
            , (ServiceMessageHeaderFragment address, int? priority) requestAddress
            , (ServiceMessageHeader adddress, int? priority) responseAddress
            , int? maxUdpMessagePayloadSize
            )
        {
            Config = udp;
            Mode = mode;

            mConnectionsListener = new Dictionary<IPEndPoint, State>();
            mConnectionsSender = new Dictionary<IPEndPoint, State>();

            mIncomingPending = new ConcurrentQueue<Message>();

            RequestAddress = requestAddress;
            ResponseAddress = responseAddress;
            MaxUdpMessagePayloadSize = maxUdpMessagePayloadSize;
        }
        #endregion

        #region Start()
        /// <summary>
        /// This method creates the socket configuration for the Udp protocol. 
        /// </summary>
        protected override void StartInternal()
        {
            if (Mode == CommunicationAgentCapabilities.Listener || Mode == CommunicationAgentCapabilities.Bidirectional)
                switch (Config.Mode)
                {
                    case UdpTransmissionMode.Unicast:
                        Config.Addresses.ForEach((a) => ListenerUnicastAdd(a, Config.Port));
                        break;
                    case UdpTransmissionMode.Broadcast:
                        throw new NotImplementedException();
                    //break;
                    case UdpTransmissionMode.Multicast:
                        throw new NotImplementedException();
                        //break;
                }

            if (Mode == CommunicationAgentCapabilities.Sender || Mode == CommunicationAgentCapabilities.Bidirectional)
                switch (Config.Mode)
                {
                    case UdpTransmissionMode.Unicast:
                        Config.Addresses.ForEach((a) => SenderUnicastAdd(a, Config.Port, Config.RemoteEndPoint));
                        break;
                    case UdpTransmissionMode.Broadcast:
                        Config.Addresses.ForEach((a) => SenderBroadcastAdd(a, Config.Port, Config.RemoteEndPoint));
                        break;
                    case UdpTransmissionMode.Multicast:
                        throw new NotImplementedException();
                        //break;
                }

            base.StartInternal();
        }
        #endregion
        #region Stop()
        /// <summary>
        /// This method closes all the Udp sockets.
        /// </summary>
        protected override void StopInternal()
        {
            base.StopInternal();

            mConnectionsListener.ForEach((k) => k.Value.Socket.Close());
            mConnectionsListener.Clear();
            mConnectionsSender.ForEach((k) => k.Value.Socket.Close(1));
            mConnectionsSender.Clear();
        } 
        #endregion

        /// <summary>
        /// Gets the UDP configuration.
        /// </summary>
        public UdpConfig Config { get; }

        public (ServiceMessageHeaderFragment address, int? priority) RequestAddress { get; }

        public (ServiceMessageHeader address, int? priority) ResponseAddress { get; }

        public int? MaxUdpMessagePayloadSize { get; }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        public CommunicationAgentCapabilities Mode { get; }

        /// <summary>
        /// The current length of dequeued messages.
        /// </summary>
        /// <returns></returns>
        public override long? QueueLengthCurrent() => mIncomingPending.Count;

        #region MessagesPull(int? count, int? wait, string mappingChannel = null)
        /// <summary>
        /// This method pulls fabric messages and converts them in to generic payload messages for the Microservice to process.
        /// </summary>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="wait">The maximum wait in milliseconds</param>
        /// <param name="mappingChannel">This is the incoming mapping channel for subscription based client where the subscription maps
        /// to a new incoming channel on the same topic.</param>
        /// <returns>
        /// Returns a list of transmission for processing.
        /// </returns>
        public override async Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null)
        {
            int? timeOut = null;
            int countMax = count ?? 10;
            int errorCount = 0;
            Guid? batchId = null;

            if (wait.HasValue)
                timeOut = Environment.TickCount + wait.Value;

            var batch = new List<TransmissionPayload>();

            if (BoundaryLoggingActive)
                batchId = Collector?.BoundaryBatchPoll(count ?? -1, mIncomingPending.Count
                    , mappingChannel ?? ChannelId, Priority);

            try
            {
                Message result = null;

                while (!mIncomingPending.IsEmpty
                    && countMax > 0
                    && (!timeOut.HasValue || timeOut.Value > Environment.TickCount)
                    && mIncomingPending.TryDequeue(out result)
                    )
                {
                    try
                    {
                        var holder = (ServiceHandlerContext)result.Buffer;
                        holder.Metadata = result.RemoteEndPoint;

                        holder.ContentType = ServiceHandlerIds.Serializer;
                        //holder.ContentEncoding = ContentEncoding;

                        IServiceHandlerSerialization serializer;
                        if (!ServiceHandlers.Serialization.TryGet(ServiceHandlerIds.Serializer, out serializer))
                            throw new Exception();

                        if (!serializer.TryDeserialize(holder))
                        {
                            holder.SetObject(new UdpHelper.Message { Buffer = holder.Blob, RemoteEndPoint = (IPEndPoint)holder.Metadata });
                        }

                        var sMessage = new ServiceMessage((MappingChannelId ?? ChannelId, RequestAddress.address), ResponseAddress.address);

                        sMessage.ResponseChannelPriority = ResponseAddress.priority ?? 1;

                        //        //        sMessage.ChannelPriority = RequestAddressPriority ?? client.Priority;
                        //        //        sMessage.Holder = holder;

                        //        //        return sMessage;

                        //holder.
                        ////Unpack the message in the holder.
                        //var sm = MessageUnpack(holder);

                        //batch.Add(new TransmissionPayload(sm));
                    }
                    catch (Exception ex)
                    {
                        Collector?.LogException("UdpClientHolder/MessagesPull deserialization error.", ex);
                        errorCount++;
                    }

                    countMax--;
                }
            }
            catch (Exception ex)
            {
                LogException("Messaging Exception (Pull)", ex);
            }

            LastTickCount = Environment.TickCount;

            return batch;
        }
        #endregion

        #region Transmit(TransmissionPayload payload, int retry = 0)
        /// <summary>
        /// This method is used to Transmit the payload. You should override this method to insert your own transmission logic.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        /// <param name="retry">This parameter specifies the number of retries that should be attempted if transmission fails. By default this value is 0.</param>
        public override async Task Transmit(TransmissionPayload payload, int retry = 0)
        {
            try
            {
                IServiceHandlerSerialization serializer;

                if (!ServiceHandlers.Serialization.TryGet(ServiceHandlerIds.Serializer, out serializer))
                    throw new Exception();

                if (!serializer.SupportsSerialization(payload.Message.Holder))
                    throw new Exception();

                if (!serializer.TrySerialize(payload.Message.Holder))
                    throw new Exception();

                if (payload.Message.Holder.Blob != null)
                    Send(payload.Message.Holder.Blob, payload.Message.Holder.Blob.Length);
            }
            catch (Exception ex)
            {
                Collector?.LogException("UdpClientHolder/Transmit", ex);
            }
        }
        #endregion

        #region Class -> State
        /// <summary>
        /// This class holds the Socket state when receiving and transmitting information.
        /// </summary>
        public class State
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
            /// Initializes a new instance of the <see cref="State"/> class.
            /// </summary>
            /// <param name="mode">The mode.</param>
            /// <param name="socket">The socket.</param>
            public State(CommunicationAgentCapabilities mode, Socket socket)
            {
                Mode = mode;
                Buffer = Mode != CommunicationAgentCapabilities.Sender ? (new byte[UDPMAXSIZE]) : null;
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
            public CommunicationAgentCapabilities Mode { get; }
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
        #endregion
        #region Class -> Message
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

            var state = new State(CommunicationAgentCapabilities.Listener, socket);

            mConnectionsListener.Add(ep, state);

            state.Receive(new AsyncCallback(ListenerUnicastEndReceiveFrom));
        }

        private void ListenerUnicastEndReceiveFrom(IAsyncResult ar)
        {
            try
            {
                EndPoint ep = new IPEndPoint(0, 0);
                var state = (State)ar.AsyncState;
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

            var state = new State(CommunicationAgentCapabilities.Sender, socket);

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

            var state = new State(CommunicationAgentCapabilities.Sender, socket);

            mConnectionsSender.Add(ep, state);
        }
        #endregion

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

    }
}
