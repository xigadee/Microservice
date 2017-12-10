﻿using System;
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
        /// <summary>
        /// Gets the content-type parameter: application/json
        /// </summary>
        public override string ContentType { get; } = "application/json";

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
