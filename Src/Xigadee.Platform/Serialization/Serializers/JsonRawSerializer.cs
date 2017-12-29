using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Xigadee
{
    /// <summary>
    /// This is the raw JSON serializer.
    /// </summary>
    /// <seealso cref="Xigadee.SerializerBase" />
    public class JsonRawSerializer : SerializerBase
    {
        private readonly JsonSerializer mJsonSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRawSerializer"/> class.
        /// </summary>
        public JsonRawSerializer()
        {
            ContentType = "application/json";
            mJsonSerializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto };
        }

        public override void Deserialize(SerializationHolder holder)
        {
            //mJsonSerializer.Deserialize(
        }

        public override void Serialize(SerializationHolder holder)
        {
            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            using (var textWriter = new JsonTextWriter(streamWriter))
            {
                mJsonSerializer.Serialize(textWriter, holder.Object);
                streamWriter.Flush();
                stream.Position = 0;
                holder.Blob = stream.ToArray();
            }

            holder.ContentType = ContentType + $"; type=\"{holder.Object.GetType().ToString()}\"";
        }

        public override bool SupportsContentTypeSerialization(Type entityType)
        {
            return true;
        }
    }
}
