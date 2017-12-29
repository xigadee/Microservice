using System;
using System.Net;
using System.Threading.Tasks;
using Xigadee;

namespace PiO
{
    /// <summary>
    /// This class holds the Lightwave message.
    /// </summary>
    public class LightwaveMessage
    {
        /// <summary>
        /// The serialization mime type.
        /// </summary>
        public const string MimeContentType = "udp/lightwave";

        public LightwaveMessage(SerializationHolder holder)
        {

        }
    }
    /// <summary>
    /// This is the binary deserializer for the incoming UDP packets.
    /// </summary>
    /// <seealso cref="Xigadee.SerializerBase" />
    public class LightwaveUdpDeserializer: SerializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LightwaveUdpDeserializer"/> class.
        /// </summary>
        public LightwaveUdpDeserializer()
        {
            ContentType = LightwaveMessage.MimeContentType;
        }

        /// <summary>
        /// Deserializes the specified holder.
        /// </summary>
        /// <param name="holder">The holder.</param>
        public override void Deserialize(SerializationHolder holder)
        {
            holder.Object = new LightwaveMessage(holder);
            holder.ObjectType = holder.Object.GetType();
        }

        /// <summary>
        /// Serialization is not supported.
        /// </summary>
        /// <param name="holder">The holder.</param>
        /// <exception cref="NotSupportedException"></exception>
        public override void Serialize(SerializationHolder holder)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Serialization is not supported.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>
        /// Returns false.
        /// </returns>
        public override bool SupportsContentTypeSerialization(Type entityType)
        {
            return false;
        }
    }
}
