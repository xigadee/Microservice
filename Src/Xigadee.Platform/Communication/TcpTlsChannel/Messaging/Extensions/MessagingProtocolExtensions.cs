using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace Xigadee
{
    public static class MessagingProtocolExtensions
    {
        public static byte[] ToByte(this HttpRequestMessage message)
        {
            return null;
        }
        public static byte[] ToByte(this HttpResponseMessage message)
        {
            return null;
        }

        /// <summary>
        /// This extension method converts a HttpRequestMessage in to a byte array and writes it to the stream.
        /// </summary>
        /// <param name="message">The message to convert.</param>
        /// <param name="sm">The stream to write to.</param>
        public static void WriteToStream(this HttpRequestMessage message, Stream sm)
        {
            if (sm == null)
                throw new ArgumentNullException("sm");
            byte[] blob = message.ToByte();

            sm.Write(blob,0, blob.Length);
        }

        /// <summary>
        /// This extension method converts a HttpResponseMessage in to a byte array and writes it to the stream.
        /// </summary>
        /// <param name="message">The message to convert.</param>
        /// <param name="sm">The stream to write to.</param>
        public static void WriteToStream(this HttpResponseMessage message, Stream sm)
        {
            if (sm == null)
                throw new ArgumentNullException("sm");
            byte[] blob = message.ToByte();

            sm.Write(blob, 0, blob.Length);
        }
    }
}
