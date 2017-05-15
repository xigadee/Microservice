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
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for the various message derivations.
    /// </summary>
    public class MessageBase
    {
        #region Declarations
        private bool mLoaded = false;
        private object syncLoad = new object();
        #endregion // Declarations
        #region Constructors
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public MessageBase()
        {
        }
        #endregion // Constructors


        #region Loaded/CanLoad
        /// <summary>
        /// This property identifies whether the message can be loaded.
        /// </summary>
        public virtual bool CanLoad
        {
            get { lock (syncLoad) { return !mLoaded; } }
            protected set { lock (syncLoad) { mLoaded = !value; } }
        }
        /// <summary>
        /// This method returns true if the message has been loaded.
        /// </summary>
        public virtual bool Loaded
        {
            get { lock (syncLoad) { return mLoaded; } }
            protected set { lock (syncLoad) { mLoaded = value; } }
        }
        #endregion // Declarations

        #region Serialization Helpers
        public static void WriteVal(BinaryWriter w, byte[] data)
        {
            w.Write(data.Length);
            w.Write(data);
        }

        public static void ReadVal(BinaryReader r, out byte[] data)
        {
            int len = r.ReadInt32();
            data = r.ReadBytes(len);
        }

        public static void WriteVal(BinaryWriter w, Guid? data)
        {
            w.Write(data.HasValue);
            if (data.HasValue)
                WriteVal(w, data.Value);
        }

        public static void WriteVal(BinaryWriter w, Guid data)
        {
            w.Write(data.ToByteArray().Length);
            w.Write(data.ToByteArray());
        }

        public static void WriteVal(BinaryWriter w, DateTime data)
        {
            w.Write(data.Ticks);
        }

        public static void WriteVal(BinaryWriter w, DateTime? data)
        {
            w.Write(data.HasValue);
            if (data.HasValue)
                WriteVal(w, data.Value);
        }

        public static void ReadVal(BinaryReader r, out Guid? data)
        {
            if (!r.ReadBoolean())
            {
                data = null;
                return;
            }

            Guid outGuid;
            ReadVal(r, out outGuid);
            data = outGuid;
        }

        public static void ReadVal(BinaryReader r, out DateTime? data)
        {
            if (!r.ReadBoolean())
            {
                data = null;
                return;
            }

            DateTime outData;
            ReadVal(r, out outData);
            data = outData;
        }

        public static void ReadVal(BinaryReader r, out Guid data)
        {
            int len = r.ReadInt32();
            byte[] blob = r.ReadBytes(len);
            data = new Guid(blob);
        }

        public static void ReadVal(BinaryReader r, out DateTime data)
        {
            data = new DateTime(r.ReadInt64());
        }
        #endregion 
    }
}
