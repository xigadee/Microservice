using System.Net;

namespace Xigadee
{
    /// <summary>
    /// This class holds the incoming UDP binary data. This is used when the deserializers cannot pick up the message type.
    /// </summary>
    public class UdpBinaryContext
    {
        /// <summary>
        /// Gets or sets the binary payload.
        /// </summary>
        public byte[] Blob { get; set; }
        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        public IPEndPoint Endpoint { get; set; }
    }
}

