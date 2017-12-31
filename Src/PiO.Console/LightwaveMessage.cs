using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Xigadee;

namespace PiO
{
    /// <summary>
    /// This class holds the Lightwave message.
    /// </summary>
    public class LightwaveMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightwaveMessage"/> class and sets the properties from the incoming JSON binary payload.
        /// </summary>
        /// <param name="blob">The UDP binary blob.</param>
        /// <param name="ep">The IP endpoint</param>
        public LightwaveMessage(byte[] blob, IPEndPoint ep)
        {
            EndPoint = ep;
            var json = Encoding.UTF8.GetString(blob);
            dynamic incoming = JsonConvert.DeserializeObject(json);
            TempId = incoming.Id;
            TempTimeStamp = incoming.TimeStamp;
        }
        /// <summary>
        /// Gets the end point of the remote party sending the message.
        /// </summary>
        public IPEndPoint EndPoint { get; }

        public int TempId { get; }

        public DateTime? TempTimeStamp { get; }
    }

}
