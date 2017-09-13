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
        /// <summary>
        /// Gets or sets the magic numbers.
        /// </summary>
        public virtual byte[] MagicNumbers { get; set; }
        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        public virtual Type EntityType { get; set; }
        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
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
        /// <summary>
        /// Holds the disposed state.
        /// </summary>
        protected bool mDisposed = false;
        /// <summary>
        /// The records the supported status of the known types.
        /// </summary>
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

        #region SupportsObjectTypeSerialization(Type entityType)
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

        /// <summary>
        /// Creates the serializer.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>The serializer.</returns>
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
        #region PayloadMagicNumbers()
        /// <summary>
        /// This is the collection of byte magic numbers the byte array will index with,
        /// </summary>
        /// <returns>A collection of 2 byte arrays.</returns>
        public virtual IEnumerable<byte[]> PayloadMagicNumbers()
        {
            yield return Identifier;
        } 
        #endregion

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
        /// <param name="start">The index point in the incoming byte array.</param>
        /// <param name="length">The count of the data in the byte array.</param>
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
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <returns>The deserialized entity.</returns>
        public virtual E Deserialize<E>(byte[] blob)
        {
            return Deserialize<E>(blob, 0, blob.Length);
        }
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <param name="start">The array start.</param>
        /// <param name="length">The array length.</param>
        /// <returns>The deserialized entity.</returns>
        public virtual E Deserialize<E>(byte[] blob, int start, int length)
        {
            return (E)Deserialize(blob, start, length);
        }
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <returns>The deserialized entity.</returns>
        public virtual object Deserialize(byte[] blob)
        {
            return Deserialize(blob, 0, blob.Length);
        }
        /// <summary>
        /// Deserializes the specified binary blob.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <param name="start">The array start.</param>
        /// <param name="length">The array length.</param>
        /// <returns>The deserialized entity.</returns>
        public virtual object Deserialize(byte[] blob, int start, int length)
        {
            S state = new S();
            DeserializeInternal(state, blob, start, length);
            return state.Entity;
        }
        #endregion
        #region Serialize public        
        /// <summary>
        /// Serializes the specified entity.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns>The binary blob.</returns>
        public virtual byte[] Serialize<E>(E entity)
        {
            return Serialize((object)entity);
        }
        /// <summary>
        /// Serializes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The binary blob.</returns>
        public abstract byte[] Serialize(object entity);

        #endregion

        /// <summary>
        /// Deserializes the entity in to the state object.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="blob">The incoming binary blob.</param>
        /// <param name="index">The byte array index.</param>
        /// <param name="count">The byte count.</param>
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

        /// <summary>
        /// Serializes the entity from the state object.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>Returns the byte array.</returns>
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

        /// <summary>
        /// Sets the state object type.
        /// </summary>
        /// <param name="sr">The reader.</param>
        /// <param name="state">The state.</param>
        protected virtual void ObjectTypeRead(BinaryReader sr, S state)
        {
            string typeName = sr.ReadString();
            state.EntityType = Type.GetType(typeName);
        }
        /// <summary>
        /// Writes the object type to the binary writer.
        /// </summary>
        /// <param name="sw">The binary writer.</param>
        /// <param name="state">The state.</param>
        protected virtual void ObjectTypeWrite(BinaryWriter sw, S state)
        {
            sw.Write(state.EntityType.AssemblyQualifiedName);
        }
        /// <summary>
        /// Reads the object from the binary reader and sets it in the state.
        /// </summary>
        /// <param name="sr">The binary reader.</param>
        /// <param name="state">The state.</param>
        protected abstract void ObjectRead(BinaryReader sr, S state);

        /// <summary>
        /// Writes the object in the state to the binary writer.
        /// </summary>
        /// <param name="sw">The binary writer.</param>
        /// <param name="state">The state.</param>
        protected abstract void ObjectWrite(BinaryWriter sw, S state);


    }
}
