using System;
using System.IO;

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
