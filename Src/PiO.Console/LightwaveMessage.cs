using System;
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
        /// <param name="holder">The holder containing the binary data.</param>
        public LightwaveMessage(SerializationHolder holder)
        {
            var json = Encoding.UTF8.GetString(holder.Blob);
            dynamic incoming = JsonConvert.DeserializeObject(json);
            Id = incoming.Id;
            TimeStamp = incoming.TimeStamp;
        }

        public int Id { get; }

        public DateTime? TimeStamp { get; }
    }

}
