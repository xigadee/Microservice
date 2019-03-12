using System;
using System.Xml.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the serializer / deserializer for an entity.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public class EntitySerializer<E>
    {
        /// <summary>
        /// Holds the entity serializer
        /// </summary>
        public Func<E, string> Serializer { get; set; }

        /// <summary>
        /// Holds the entity deserializer
        /// </summary>
        public Func<string, E> Deserializer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySerializer{E}"/> class.
        /// </summary>
        /// <param name="serializer">The string serializer.</param>
        /// <param name="deserializer">The string deserializer.</param>
        public EntitySerializer(Func<E, string> serializer, Func<string, E> deserializer)
        {
            Serializer = serializer;
            Deserializer = deserializer;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySerializer{E}"/> class.
        /// </summary>
        /// <param name="serializer">The XML serializer.</param>
        /// <param name="deserializer">The XML deserializer.</param>
        public EntitySerializer(Func<E, XElement> serializer, Func<XElement, E> deserializer)
        {
            Serializer = e => serializer(e).ToString();
            Deserializer = s => deserializer(XElement.Parse(s));
        }
    }
}
