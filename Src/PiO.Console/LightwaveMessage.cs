using Xigadee;
using System.Text;
using Newtonsoft.Json;
namespace PiO
{
    /// <summary>
    /// This class holds the Lightwave message.
    /// </summary>
    public class LightwaveMessage
    {
        public LightwaveMessage(SerializationHolder holder)
        {
            var json = Encoding.UTF8.GetString(holder.Blob);
            dynamic incoming = JsonConvert.DeserializeObject(json);
            Id = incoming.Id;
            TimeStamp = incoming.TimeStamp;
        }

        public int Id { get; }

        public int TimeStamp { get; }
    }

}
