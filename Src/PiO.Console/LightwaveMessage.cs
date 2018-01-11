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
        public LightwaveMessage()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LightwaveMessage"/> class and sets the properties from the incoming JSON binary payload.
        /// </summary>
        /// <param name="blob">The UDP binary blob.</param>
        /// <param name="ep">The IP endpoint</param>
        public LightwaveMessage(byte[] blob, IPEndPoint ep)
        {
            EndPoint = ep.ToString();
            IncomingUTC = DateTime.UtcNow;

            var json = Encoding.UTF8.GetString(blob,2,blob.Length -2);
            dynamic incoming = JsonConvert.DeserializeObject(json);
            Trans = incoming.trans;
            Time = (long)incoming.time;

            EnergyCurrent = incoming.cUse;
            EnergyToday = incoming.todUse;
            Type = incoming.type;
            Serial = incoming.serial;
            Prod = incoming.prod;
            Fn = incoming.fn;
            Packet = incoming.pkt;
            Mac = incoming.mac;
        }
        /// <summary>
        /// Gets the end point of the remote party sending the message.
        /// </summary>
        public string EndPoint { get; set;}
        /// <summary>
        /// The incoming time stamp.
        /// </summary>
        public DateTime? IncomingUTC { get; set; }

        public int Trans { get; set; }

        public string Mac { get; set; }

        public long Time { get; set; }

        public int EnergyCurrent { get; set; }

        public int EnergyToday { get; set; }

        public string Type { get; set; }
        public string Serial { get; set; }
        public string Prod { get; set; }
        public string Fn { get; set; }
        public string Packet { get; set; }
    }

}
