using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Xigadee
{
    /// <summary>
    /// This is the raw Bson serializer.
    /// </summary>
    /// <seealso cref="Xigadee.SerializerBase" />
    public class BsonRawSerializer : SerializerBase
    {
        /// <summary>
        /// Gets the content-type parameter: application/bson
        /// </summary>
        public override string ContentType { get; } = "application/bson";

        public override object Deserialize(byte[] blob, int start, int length)
        {
            throw new NotImplementedException();
        }

        public override byte[] Serialize(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
