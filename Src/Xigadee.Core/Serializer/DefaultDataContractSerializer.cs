#region using

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the default serializer that uses the standard DataContractSerializer
    /// </summary>
    public class DefaultDataContractSerializer : DefaultSerializerBase<DataContractSerializer, SerializerState>
    {
        private readonly byte[] mMagicBytes = new byte[] { 66, 215 };

        public override IEnumerable<byte[]> PayloadMagicNumbers()
        {
            yield return mMagicBytes;
        }

        protected override DataContractSerializer CreateSerializer(Type entityType)
        {
            return new DataContractSerializer(entityType);
        }

        public override byte[] Serialize(object entity)
        {
            var state = new SerializerState();
            state.MagicNumbers = mMagicBytes;
            state.EntityType = entity.GetType();
            state.Entity = entity;

            return SerializeInternal(state);
        }

        protected override void ObjectRead(BinaryReader sr, SerializerState state)
        {
            int length = sr.ReadInt32();
            var blob = sr.ReadBytes(length);

            var serializer = CreateSerializer(state.EntityType);
            using (var stream = new MemoryStream(blob))
            {
                stream.Position = 0;
                state.Entity = serializer.ReadObject(stream);
            }

        }

        protected override void ObjectWrite(BinaryWriter sw, SerializerState state)
        {
            var serializer = CreateSerializer(state.EntityType);

            byte[] blob;
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, state.Entity);
                stream.Position = 0;
                blob = stream.ToArray();
            }

            sw.Write(blob.Length);
            sw.Write(blob, 0, blob.Length);
            //throw new NotImplementedException();
        }
    }
}
