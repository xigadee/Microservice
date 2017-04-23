#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
    [Obsolete("Use JsonContractSerializer instead.", false)]
    public class DefaultDataContractSerializer : DefaultSerializerBase<DataContractSerializer, SerializerState>
    {
        public override byte[] Identifier
        {
            get
            {
                return new byte[] { 66, 215 };
            }
        }

        protected override DataContractSerializer CreateSerializer(Type entityType)
        {
            return new DataContractSerializer(entityType);
        }

        public override byte[] Serialize(object entity)
        {
            var state = new SerializerState();
            state.MagicNumbers = Identifier;
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
