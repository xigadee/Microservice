using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Xigadee
{
    public class JsonContractSerializer : DefaultSerializerBase<JsonSerializer, SerializerState>
    {
        private readonly byte[] mMagicBytes = { 67, 216 };
        private readonly JsonSerializer mJsonSerializer = new JsonSerializer {TypeNameHandling = TypeNameHandling.Auto};

        protected override JsonSerializer CreateSerializer(Type entityType)
        {
            return mJsonSerializer;
        }

        public override IEnumerable<byte[]> PayloadMagicNumbers()
        {
            yield return mMagicBytes;
        }

        public override byte[] Serialize(object entity)
        {
            var state = new SerializerState {MagicNumbers = mMagicBytes, EntityType = entity.GetType(), Entity = entity};
            return SerializeInternal(state);
        }

        protected override void ObjectRead(BinaryReader sr, SerializerState state)
        {
            int length = sr.ReadInt32();
            var blob = sr.ReadBytes(length);

            var serializer = CreateSerializer(state.EntityType);
            using (var stream = new MemoryStream(blob))
            using (var streamReader = new StreamReader(stream))
            using (var textReader = new JsonTextReader(streamReader))
            {
                stream.Position = 0;
                state.Entity = serializer.Deserialize(textReader, state.EntityType);
            }
        }

        protected override void ObjectWrite(BinaryWriter sw, SerializerState state)
        {
            var serializer = CreateSerializer(state.EntityType);

            byte[] blob;
            
            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream))
            using (var textWriter = new JsonTextWriter(streamWriter))
            {
                serializer.Serialize(textWriter, state.Entity);
                streamWriter.Flush();                
                stream.Position = 0;
                blob = stream.ToArray();
            }

            sw.Write(blob.Length);
            sw.Write(blob, 0, blob.Length);
        }
    }
}
