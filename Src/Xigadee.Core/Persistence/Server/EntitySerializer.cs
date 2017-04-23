using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    
        public EntitySerializer(Func<E, string> serializer, Func<string, E> deserializer)
        {
            Serializer = serializer;
            Deserializer = deserializer;
        }

        public EntitySerializer(Func<E, XElement> serializer, Func<XElement, E> deserializer)
        {
            Serializer = e => serializer(e).ToString();
            Deserializer = s => deserializer(XElement.Parse(s));
        }
    }
}
