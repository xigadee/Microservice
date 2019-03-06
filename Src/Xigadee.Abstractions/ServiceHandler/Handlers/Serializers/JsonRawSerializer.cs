using System;
using System.Collections.Concurrent;
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
        public JsonRawSerializer():base("application/json")
        {
            mJsonSerializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto};
        }

        protected ConcurrentDictionary<string, Type> mTypesResolvedCache = new ConcurrentDictionary<string, Type>();

        protected Type TypeResolve(string typeAsString, bool useCache = true)
        {
            Type type;

            if (useCache && mTypesResolvedCache.TryGetValue(typeAsString, out type))
                return type;

            type = TypeHelper.Resolve(typeAsString);

            return mTypesResolvedCache.AddOrUpdate(typeAsString, type, (s, t) => type);
        }

        public override void Deserialize(ServiceHandlerContext holder)
        {
            if (!holder.HasContentType)
                return;
            
            var type = TypeResolve(holder.ContentType.ObjectType);

            using (var stream = new MemoryStream(holder.Blob))
            using (var sReader = new StreamReader(stream))
            using (var textReader = new JsonTextReader(sReader))
            {
                var obj = mJsonSerializer.Deserialize(textReader, type);
                holder.SetObject(obj, true);
            }
        }

        public override void Serialize(ServiceHandlerContext holder)
        {
            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            using (var textWriter = new JsonTextWriter(streamWriter))
            {
                mJsonSerializer.Serialize(textWriter, holder.Object);
                streamWriter.Flush();
                stream.Position = 0;
                holder.SetBlob(stream.ToArray());
            }

            holder.ContentType = $"{Id}; type=\"{holder.Object.GetType().AssemblyQualifiedName}\"";
        }

        public override bool SupportsContentTypeSerialization(Type entityType)
        {
            return true;
        }
    }
}
