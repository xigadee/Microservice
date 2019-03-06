using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Xigadee
{
    /// <summary>
    /// This class is used to notify of an incoming type resolution by the serializer.
    /// </summary>
    public class ContentTypeEventArgs : EventArgs
    {
        public string IncomingType { get; set; }
        public Type ResolvedType { get; set; }
    }

    /// <summary>
    /// This is the raw JSON serializer.
    /// </summary>
    /// <seealso cref="Xigadee.SerializerBase" />
    public class JsonRawSerializer : SerializerBase
    {
        protected readonly JsonSerializer mJsonSerializer;

        protected ConcurrentDictionary<string, Type> mTypesResolvedCache = new ConcurrentDictionary<string, Type>();

        public event EventHandler<ContentTypeEventArgs> ObjectTypeResolved;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRawSerializer"/> class.
        /// </summary>
        public JsonRawSerializer():base("application/json")
        {
            mJsonSerializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.Auto};
        }


        protected Type TypeResolve(string typeAsString, bool useCache = true)
        {
            Type type;

            if (useCache && mTypesResolvedCache.TryGetValue(typeAsString, out type))
                return type;

            type = Type.GetType(typeAsString) ?? TypeHelper.Resolve(typeAsString);

            if (type == null)
                throw new ArgumentOutOfRangeException("");
            else
                //Allow resolution to be inspected and rejected if necessary.
                ObjectTypeResolved?.Invoke(this, new ContentTypeEventArgs() { IncomingType = typeAsString, ResolvedType = type });

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
