using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace PiO
{
    /// <summary>
    /// This class holds the Lightwave message.
    /// </summary>
    [DebuggerDisplay("{Trans}/{Type} {EnergyCurrent}/{EnergyToday}")]
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

            string json = null;
            try
            {
                json = Encoding.UTF8.GetString(blob, 2, blob.Length - 2);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return;
            }

            dynamic incoming = null;

            try
            {
                incoming = JsonConvert.DeserializeObject(json);
            }
            catch (Exception ex)
            {
                ErrorJson = json;
                ErrorMessage = ex.Message;
                return;
            }

            bool error = TrySet(() => Trans = incoming.trans);
            error |= TrySet(() => Time = (long)incoming.time);

            error |= TrySet(() => EnergyCurrent = incoming.cUse);
            error |= TrySet(() => EnergyToday = incoming.todUse);
            error |= TrySet(() => Type = incoming.type);
            error |= TrySet(() => Serial = incoming.serial);
            error |= TrySet(() => Prod = incoming.prod);
            error |= TrySet(() => Fn = incoming.fn);
            error |= TrySet(() => Packet = incoming.pkt);
            error |= TrySet(() => Mac = incoming.mac);

            if (error)
                ErrorJson = json;

            TrySet(() => Preamble = BitConverter.ToUInt16(blob, 0));
        }

        private bool TrySet(Action action)
        {
            try
            {
                action();
                return false;
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the byte preamble.
        /// </summary>
        public UInt16 Preamble { get; set; }
        /// <summary>
        /// Gets the end point of the remote party sending the message.
        /// </summary>
        public string EndPoint { get; set;}
        /// <summary>
        /// The incoming time stamp.
        /// </summary>
        public DateTime? IncomingUTC { get; set; }

        public string ErrorJson { get; set; }

        public string ErrorMessage { get; set; }

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
