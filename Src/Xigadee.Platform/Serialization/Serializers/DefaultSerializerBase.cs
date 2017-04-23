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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    #region Class -> SerializerState
    /// <summary>
    /// This is the state object used to pass the relevant parameters through the system
    /// </summary>
    public class SerializerState
    {
        public virtual byte[] MagicNumbers { get; set; }

        public virtual Type EntityType { get; set; }

        public object Entity { get; set; }
    } 
    #endregion

    /// <summary>
    /// This abstract class is used to define serializers used on the system to provide binary transport
    /// between the various parts of the system.
    /// </summary>
    public abstract class DefaultSerializerBase<A,S> : IPayloadSerializer
        where A : class
        where S : SerializerState, new()
    {
        #region Declarations
        protected bool mDisposed = false;

        ConcurrentDictionary<Type, bool> mSupported;

        /// <summary>
        /// This is the byte header for the serialization payload.
        /// </summary>
        public abstract byte[] Identifier { get; }
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public DefaultSerializerBase()
        {
            mSupported = new ConcurrentDictionary<Type, bool>();
        } 
        #endregion

        #region Dispose()
        /// <summary>
        /// This method is called when the logger is disposed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
        #region Dispose(bool disposing)
        /// <summary>
        /// This method disposes of the concurrent queue.
        /// </summary>
        /// <param name="disposing">Set to true if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed)
                return;

            if (disposing)
            {
                mDisposed = true;

            }
        }
        #endregion

        #region SupportsObjectTypeSerialization(ChannelId entityType)
        /// <summary>
        /// Returns true of the serializer supports this object channelId.
        /// </summary>
        /// <param name="entityType">The object channelId.</param>
        /// <returns>Returns true.</returns>
        public virtual bool SupportsObjectTypeSerialization(Type entityType)
        {
            A data;
            return ValidateType(entityType, out data);
        } 
        #endregion

        protected abstract A CreateSerializer(Type entityType);

        #region ValidateType(ChannelId entityType, out A data)
        /// <summary>
        /// This method validates the specific channelId.
        /// </summary>
        /// <param name="entityType">The entity channelId.</param>
        /// <param name="data">The serializer.</param>
        /// <returns>Returns true if the entity channelId is supported.</returns>
        protected bool ValidateType(Type entityType, out A data)
        {
            data = null;
            bool value = false;
            if (mSupported.TryGetValue(entityType, out value))
                return value;

            try
            {
                //See if we can make the serializer without it throwing an error.
                data = CreateSerializer(entityType);
                value = true;
            }
            catch (Exception)
            {

            }

            //We cache the result to save time later.
            mSupported.TryAdd(entityType, value);
            return value;
        }
        #endregion

        /// <summary>
        /// This is the collection of byte magic numbers the the byte array will index with,
        /// </summary>
        /// <returns>A collection of 2 byte arrays.</returns>
        public virtual IEnumerable<byte[]> PayloadMagicNumbers()
        {
            yield return Identifier;
        }

        #region SupportsPayloadDeserialization ...
        /// <summary>
        /// This method matches the incoming byte stream and identifies whether the serializer
        /// can deserialize on the basis of the index of the byte array.
        /// </summary>
        /// <param name="blob">The incoming byte array</param>
        /// <returns>Returns true if it is a match.</returns>
        public virtual bool SupportsPayloadDeserialization(byte[] blob)
        {
            return SupportsPayloadDeserialization(blob, 0, blob.Length);
        }

        /// <summary>
        /// This method matches the incoming byte stream and identifies whether the serializer
        /// can deserialize on the basis of the index of the byte array.
        /// </summary>
        /// <param name="blob">The incoming byte array</param>
        /// <param name="start">The index point in the incoming byte array.</param>
        /// <param name="length">The count of the data in the byte array.</param>
        /// <returns>Returns true if it is a match.</returns>
        public virtual bool SupportsPayloadDeserialization(byte[] blob, int start, int length)
        {
            var result = PayloadMagicNumbers()
                .Select(m => MatchMagicBytes(m, blob, start, length))
                .FirstOrDefault(r => r);

            return result;
        }

        /// <summary>
        /// This method matches the magic bytes with the index of the payload.
        /// </summary>
        /// <param name="magic">The magic byte array</param>
        /// <param name="blob">The incoming byte array</param>
        /// <param name="index">The index point in the incoming byte array.</param>
        /// <param name="count">The count of the data in the byte array.</param>
        /// <returns>Returns true if it is a match.</returns>
        protected bool MatchMagicBytes(byte[] magic, byte[] blob, int start, int length)
        {
            if (length < magic.Length | blob.Length < magic.Length)
                return false;

            for (int i = 0; i < magic.Length; i++)
            {
                if (magic[i] != blob[start + i])
                    return false;
            }

            return true; ;
        } 
        #endregion

        #region Deserialize public
        public virtual E Deserialize<E>(byte[] blob)
        {
            return Deserialize<E>(blob, 0, blob.Length);
        }

        public virtual E Deserialize<E>(byte[] blob, int start, int length)
        {
            return (E)Deserialize(blob, start, length);
        }

        public virtual object Deserialize(byte[] blob)
        {
            return Deserialize(blob, 0, blob.Length);
        }

        public virtual object Deserialize(byte[] blob, int start, int length)
        {
            S state = new S();
            DeserializeInternal(state, blob, start, length);
            return state.Entity;
        }
        #endregion
        #region Serialize public

        public virtual byte[] Serialize<E>(E entity)
        {
            return Serialize((object)entity);
        }

        public abstract byte[] Serialize(object entity);

        #endregion


        protected virtual void DeserializeInternal(S state, byte[] blob, int index, int count)
        {
            using (var stream = new MemoryStream(blob, index, count, false))
            {
                stream.Position = 0;
                //Remove the magic numbers.
                stream.ReadByte();
                stream.ReadByte();

                using (var zipStream = new GZipStream(stream, CompressionMode.Decompress, true))
                using (var sr = new BinaryReader(zipStream, Encoding.UTF8))
                {
                    ObjectTypeRead(sr, state);

                    ObjectRead(sr, state);        
                }

                stream.Close();
            }
        }

        public virtual byte[] SerializeInternal(S state)
        {
            byte[] outBlob;

            using (var stream = new MemoryStream())
            {
                stream.Write(state.MagicNumbers, 0, 2);
                using (var zipStream = new GZipStream(stream, CompressionMode.Compress, true))
                using (var sw = new BinaryWriter(zipStream, Encoding.UTF8))
                {
                    ObjectTypeWrite(sw, state);

                    ObjectWrite(sw, state);

                    stream.Flush();
                }

                stream.Position = 0;
                outBlob = stream.ToArray();

                return outBlob;
            }
        }


        protected virtual void ObjectTypeRead(BinaryReader sr, S state)
        {
            string typeName = sr.ReadString();
            state.EntityType = Type.GetType(typeName);
        }

        protected virtual void ObjectTypeWrite(BinaryWriter sw, S state)
        {
            sw.Write(state.EntityType.AssemblyQualifiedName);
        }

        protected abstract void ObjectRead(BinaryReader sr, S state);


        protected abstract void ObjectWrite(BinaryWriter sw, S state);


    }
}
