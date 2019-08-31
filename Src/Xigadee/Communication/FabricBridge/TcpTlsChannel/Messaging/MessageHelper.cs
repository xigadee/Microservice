using System;
using System.IO;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This helper class adds extension methods to the Message based classes.
    /// </summary>
    public static class MessageHelper
    {
        /// <summary>
        /// This method is used to write a byte overread from the stream
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="buffer">The byte arrary.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The byte count to write.</param>
        /// <returns>Returns a tuple containing the number of bytes read, and the new array offset.</returns>
        public static (int read, (byte[] buffer, int offset, int count)? overread) WriteFromOverRead(
            this IMessage message, byte[] buffer, int offset, int count)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (buffer == null)
                throw new ArgumentNullException("buffer");

            int totalByte = 0;

            while (count > 0 && message.CanWrite)
            {
                int consumed = message.Write(buffer, offset, count);
                offset += consumed;
                count -= consumed;
                totalByte += consumed;
            }

            if (count == 0)
                return (totalByte, null);

            return (totalByte, (buffer, offset, count));
        }
        /// <summary>
        /// This method reads from the stream and writes to the message.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="toRead">The stream to be read from.</param>
        /// <param name="overread">This tuple contains any overread of data from an earlier read to a stream.</param>
        /// <returns>Returns a tuple containing the number of bytes read, and the new array offset.</returns>
        public static (int read, (byte[] buffer, int offset, int count)? overread) WriteFromStream(
            this IMessage message, Stream toRead
            , (byte[] buffer, int offset, int count)? overread = null)
        {
            const int bufferSize = 1024;

            if (message == null)
                throw new ArgumentNullException("message");

            if (toRead == null)
                throw new ArgumentNullException("toRead");

            if (!message.CanWrite || !toRead.CanRead)
                return (0, overread);

            int count = 0;

            if (overread.HasValue)
            {
                //Check that we have consumed all the overread data?
                var result = message.WriteFromOverRead(overread.Value.buffer, overread.Value.offset, overread.Value.count);
                if (result.overread.HasValue)
                    //No then return as there is no point consuming more data from the stream.
                    return result;
                count += result.read;
            }

            overread = (new byte[bufferSize], 0, bufferSize);

            while (message.CanWrite && toRead.CanRead)
            {
                int actual = toRead.Read(overread.Value.buffer, overread.Value.offset, overread.Value.count);
                if (actual <= 0)
                    break;

                int readIn = message.Write(overread.Value.buffer, overread.Value.offset, actual);

                count += readIn;

                if (readIn < actual)
                    return (count, (overread.Value.buffer, readIn, actual - readIn));
            }

            return (count, null);
        }
    }
}
