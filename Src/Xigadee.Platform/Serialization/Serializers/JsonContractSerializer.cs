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

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Xigadee
{
    /// <summary>
    /// This class is used for serialization support, using the standard Newtonsoft Json settings.
    /// </summary>
    public class JsonContractSerializer : DefaultSerializerBase<JsonSerializer, SerializerState>
    {
        private readonly JsonSerializer mJsonSerializer = new JsonSerializer {TypeNameHandling = TypeNameHandling.Auto};

        /// <summary>
        /// This is the specific magic number for this serializer: 67,216
        /// </summary>
        public override byte[] Identifier
        {
            get
            {
                return new byte[]{ 67, 216 };
            }
        }

        protected override JsonSerializer CreateSerializer(Type entityType)
        {
            return mJsonSerializer;
        }

        /// <summary>
        /// This method serializes the entity to a byte stream.
        /// </summary>
        /// <param name="entity">The entity to serialize</param>
        /// <returns>The compressed byte stream with the identifier prepended</returns>
        public override byte[] Serialize(object entity)
        {
            var state = new SerializerState {MagicNumbers = Identifier, EntityType = entity.GetType(), Entity = entity};
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
