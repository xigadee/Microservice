using System;
using System.Net;
using System.Net.Sockets;

namespace Xigadee
{
    /// <summary>
    /// This class contains the base context properties.
    /// </summary>
    public class UdpContext
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UdpContext"/> class.
        /// </summary>
        /// <param name="direction">The context direction.</param>
        /// <param name="serializer">The system wide payload serializer.</param>
        public UdpContext(UdpContextDirection direction, IPayloadSerializationContainer serializer)
        {
            Direction = direction;
            PayloadSerializer = serializer ?? throw new ArgumentNullException("serializer", "serializer cannot be null.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpContext"/> class for an incoming message.
        /// </summary>
        /// <param name="serializer">The system wide payload serializer.</param>
        /// <param name="result">The incoming Udp message result.</param>
        public UdpContext(IPayloadSerializationContainer serializer, UdpReceiveResult result) : this(UdpContextDirection.Incoming, serializer)
        {
            Blob = result.Buffer;
            EndPoint = result.RemoteEndPoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpContext"/> class for an outgoing message.
        /// </summary>
        /// <param name="serializer">The system wide payload serializer.</param>
        /// <param name="message">The service message.</param>
        public UdpContext(IPayloadSerializationContainer serializer, ServiceMessage message) : this(UdpContextDirection.Outgoing, serializer)
        {
            Message = message;
        } 
        #endregion


        /// <summary>
        /// Gets the direction for the context, either incoming or outgoing.
        /// </summary>
        public UdpContextDirection Direction { get; }
        /// <summary>
        /// This is the system wide Payload serializer.
        /// </summary>
        IPayloadSerializationContainer PayloadSerializer { get; }

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        public IPEndPoint EndPoint { get; set; }
        /// <summary>
        /// Gets or sets the binary blob.
        /// </summary>
        public byte[] Blob { get; set; }

        /// <summary>
        /// Gets or sets the ServiceMessage.
        /// </summary>
        public ServiceMessage Message { get; set; }
    }

    /// <summary>
    /// This enumeration specifies the direction of the message.
    /// </summary>
    public enum UdpContextDirection
    {
        /// <summary>
        /// The context holds the received IPAddress and Blob from the Udp listener.
        /// </summary>
        Incoming,
        /// <summary>
        /// The context holds an outgoing ServiceMessage that needs to be translated in to an IPAddress and Blob.
        /// </summary>
        Outgoing
    }
}
